#!/usr/bin/env python3
"""Fail closed before TestFlight; evidence must describe this exact Unity source."""
import argparse, hashlib, json, sys
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

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--fingerprint", action="store_true", help="Print candidate source digest; does not approve it.")
    args = parser.parse_args()
    if args.fingerprint:
        print(fingerprint(ROOT)); return 0
    try:
        report = json.loads((ROOT / "docs/QA/visual-release-gate.json").read_text())
        validate(ROOT, report)
    except (ValueError, OSError, KeyError, TypeError) as error:
        print("TESTFLIGHT BLOCKED: " + str(error), file=sys.stderr); return 2
    print("Visual evidence matches the reviewed Unity source. Other release checks still apply.")
    return 0

if __name__ == "__main__":
    sys.exit(main())
