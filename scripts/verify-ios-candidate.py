#!/usr/bin/env python3
"""Bind a successful fresh Unity export to the exact reviewed source and payload."""
import argparse
import hashlib
import importlib.util
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RECEIPT = "perfectdrop-source-receipt.json"
# Xcode/Unity generates these compiler manifests/traces during the native
# build. They are absent from a fresh export and are not source/asset inputs.
GENERATED_OUTPUTS = {"Il2CppOutputProject/Source/il2cppOutput/" + name for name in
                     ("compile-data.json", "il2cpp-compile.traceevents", "profile.json")}
spec = importlib.util.spec_from_file_location("visual_gate", ROOT / "scripts/verify-visual-release.py")
gate = importlib.util.module_from_spec(spec)
spec.loader.exec_module(gate)


def digest_file(path):
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def payload(export):
    # User-specific Xcode state and build caches are not native source inputs.
    return {path.relative_to(export).as_posix(): digest_file(path)
            for path in sorted(export.rglob("*"))
            if path.is_file() and path.name not in (RECEIPT, ".DS_Store")
            and path.relative_to(export).as_posix() not in GENERATED_OUTPUTS
            and not {"xcuserdata", "DerivedData"}.intersection(path.relative_to(export).parts)}


def validate_export(export):
    report = json.loads((ROOT / "docs/QA/visual-release-gate.json").read_text())
    gate.validate(ROOT, report)
    receipt = json.loads((export / RECEIPT).read_text())
    if receipt["sourceSha256"] != report["sourceSha256"]:
        raise ValueError("Native export differs from the visually reviewed source.")
    if Path(receipt["exportPath"]).resolve() != export.resolve():
        raise ValueError("Native receipt belongs to a different export path.")
    if receipt.get("configuration") != "Release" or receipt.get("unityExitCode") != 0:
        raise ValueError("Native receipt is not a successful Release export.")
    if receipt["files"] != payload(export):
        raise ValueError("Native export payload changed after its successful build.")
    return receipt


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("export", type=Path)
    args = parser.parse_args()
    try:
        receipt = validate_export(args.export)
    except (OSError, ValueError, KeyError, TypeError) as error:
        print("TESTFLIGHT BLOCKED: " + str(error), file=sys.stderr)
        sys.exit(2)
    print("Release export matches reviewed source and native payload: " + receipt["sourceSha256"])
