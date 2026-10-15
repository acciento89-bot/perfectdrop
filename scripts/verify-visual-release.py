#!/usr/bin/env python3
"""Fail closed before TestFlight; evidence must describe this exact Unity source."""
import argparse, hashlib, json, re, sys, zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
REQUIRED = ("concept_similarity", "character_or_tower", "world_variants", "menu_identity", "submenu_navigation", "readability")
VIEWS = ("menu", "gameplay", "submenus", "variants")

def fingerprint(root):
    digest = hashlib.sha256()
    for directory in ("Assets", "Packages", "ProjectSettings"):
        for path in sorted((root / directory).rglob("*")):
            if path.is_file() and path.name != ".DS_Store":
                digest.update(path.relative_to(root).as_posix().encode() + b"\0")
                digest.update(hashlib.sha256(path.read_bytes()).digest())
    return digest.hexdigest()

def validate(root, report):
    if report.get("status") != "passed":
        raise ValueError("Visual review has not passed. No TestFlight upload is permitted.")
    if report.get("sourceSha256") != fingerprint(root):
        raise ValueError("Visual review is stale or belongs to different Unity source.")
    if not report.get("reviewer") or not report.get("reviewedAt"):
        raise ValueError("Missing actual visual review record.")
    criteria = report.get("criteria", {})
    if any(criteria.get(key) != "passed" for key in REQUIRED):
        raise ValueError("At least one required visual criterion has not passed.")
    evidence = report.get("evidence", {})
    for kind in ("concept",) + VIEWS:
        items = evidence.get(kind, [])
        if not isinstance(items, list) or not items:
            raise ValueError("Missing visual evidence: " + kind)
        for item in items:
            path = Path(item.get("path", ""))
            if not path.is_absolute(): path = root / path
            if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest() != item.get("sha256"):
                raise ValueError("Missing or modified visual evidence: " + kind)
            if kind != "concept" and item.get("origin") != "Unity-render":
                raise ValueError("Gameplay evidence must be a genuine Unity render.")

def png_info(data):
    """Validate complete non-interlaced RGB/RGBA PNG chunks and decompressed rows."""
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("Icon evidence is not a PNG.")
    pos = 8; header = None; compressed = []; ended = False
    while pos < len(data):
        if pos + 12 > len(data):
            raise ValueError("Truncated icon PNG chunk.")
        size = int.from_bytes(data[pos:pos+4], "big"); tag = data[pos+4:pos+8]; end = pos+12+size
        if end > len(data):
            raise ValueError("Truncated icon PNG payload.")
        payload = data[pos+8:pos+8+size]
        if zlib.crc32(tag+payload) & 0xffffffff != int.from_bytes(data[pos+8+size:end], "big"):
            raise ValueError("Corrupt icon PNG CRC.")
        if header is None and tag != b"IHDR":
            raise ValueError("Icon PNG has no leading image header.")
        if tag == b"IHDR":
            if header is not None or size != 13:
                raise ValueError("Invalid icon PNG header.")
            width, height = int.from_bytes(payload[:4], "big"), int.from_bytes(payload[4:8], "big")
            depth, kind, compression, filters, interlace = payload[8:13]
            if not (0 < width <= 4096 and 0 < height <= 4096 and depth in (8,16) and kind in (2,6) and compression == filters == interlace == 0):
                raise ValueError("Use a non-interlaced RGB/RGBA icon PNG, at most 4096px.")
            header = (width, height, depth, kind)
        elif tag == b"IDAT":
            compressed.append(payload)
        elif tag == b"IEND":
            if size != 0 or end != len(data):
                raise ValueError("Invalid icon PNG end marker.")
            ended = True; break
        pos = end
    if not ended or not header or not compressed:
        raise ValueError("Incomplete icon PNG.")
    width, height, depth, kind = header
    stride = 1 + width * (3 if kind == 2 else 4) * depth // 8; expected = height * stride
    try:
        decoder = zlib.decompressobj(); rows = decoder.decompress(b"".join(compressed), expected+1)
    except zlib.error as error:
        raise ValueError("Invalid icon PNG image data.") from error
    if len(rows) != expected or not decoder.eof or decoder.unused_data or decoder.unconsumed_tail or any(rows[y*stride] > 4 for y in range(height)):
        raise ValueError("Invalid or truncated icon PNG scanlines.")
    return header


def validate_store_icon(root):
    """A human/agent must compare the actual icon and current gameplay before upload."""
    icon_report = json.loads((root / "docs/QA/store-icon-review.json").read_text())
    if icon_report.get("status") != "passed":
        raise ValueError("Store icon review has not passed: " + icon_report.get("reason", "not reviewed"))
    source = fingerprint(root)
    if icon_report.get("sourceSha256") != source:
        raise ValueError("Store icon review is stale for this Unity source.")
    if not all(icon_report.get(key) for key in ("reviewer", "reviewedAt", "decision")):
        raise ValueError("Store icon lacks an explicit content-accuracy review.")
    if icon_report.get("variantConsistencyChecked") is not True:
        raise ValueError("Native small/large/store icon consistency has not been checked.")
    evidence = icon_report.get("evidence", {})
    for key, origin in (("sourceIcon", "project-icon"), ("nativeIcon", "native-build-icon")):
        item = evidence.get(key) or {}
        path = Path(item.get("path", ""))
        if not path.is_absolute(): path = root / path
        if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest() != item.get("sha256") or item.get("origin") != origin:
            raise ValueError("Missing or modified icon evidence: " + key)
        if key == "sourceIcon":
            try:
                path.resolve().relative_to((root / "Assets").resolve())
            except ValueError:
                raise ValueError("Reviewed source icon is outside this project's Assets.")
            metadata = path.with_name(path.name + ".meta").read_text()
            guid = re.search(r"(?m)^guid: ([0-9a-fA-F]{32})$", metadata)
            settings = (root / "ProjectSettings/ProjectSettings.asset").read_text()
            assigned = settings.partition("m_BuildTargetIcons:")[2].partition("m_BuildTargetPlatformIcons:")[0]
            default = re.search(r"(?ms)^  - m_BuildTarget: *\n(.*?)(?=^  - m_BuildTarget: |\Z)", assigned)
            if not guid or not default or guid.group(1) not in default.group(1):
                raise ValueError("Reviewed source icon is not assigned as the project icon.")
            platform = settings.partition("m_BuildTargetPlatformIcons:")[2]
            platform = re.split(r"(?m)^  (?!-)[A-Za-z_][^\n]*:", platform, maxsplit=1)[0]
            for section in (assigned, platform):
                for target, block in re.findall(r"(?ms)^  - m_BuildTarget: (iPhone|iOS) *\n(.*?)(?=^  - m_BuildTarget: |\Z)", section):
                    overrides = re.findall(r"\bguid: *([0-9a-fA-F]{32})\b", block)
                    if any(value not in (guid.group(1), "0"*32) for value in overrides):
                        raise ValueError("Native iPhone icon override differs from the reviewed design.")
        width, height, depth, kind = png_info(path.read_bytes())
        if width != height or width < 1024:
            raise ValueError("Icon evidence must be square and at least 1024 pixels: " + key)
        if key == "nativeIcon" and (width != 1024 or depth != 8 or kind != 2 or item.get("sourceSha256") != source):
            raise ValueError("Native store icon must be a source-bound 1024px RGB PNG.")
    frames = evidence.get("gameplay", [])
    if not frames:
        raise ValueError("Icon review has no actual gameplay comparison.")
    for frame in frames:
        path = Path(frame.get("path", ""))
        if not path.is_absolute(): path = root / path
        if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest() != frame.get("sha256") or frame.get("origin") != "Unity-render":
            raise ValueError("Icon gameplay comparison is missing, changed or not a Unity render.")

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--fingerprint", action="store_true", help="Print candidate source digest; does not approve it.")
    args = parser.parse_args()
    if args.fingerprint:
        print(fingerprint(ROOT)); return 0
    try:
        report = json.loads((ROOT / "docs/QA/visual-release-gate.json").read_text())
        validate_store_icon(ROOT)
        validate(ROOT, report)
    except (ValueError, OSError, KeyError, TypeError) as error:
        print("TESTFLIGHT BLOCKED: " + str(error), file=sys.stderr); return 2
    print("Visual and icon evidence match the reviewed Unity source. Other release checks still apply.")
    return 0

if __name__ == "__main__":
    sys.exit(main())
