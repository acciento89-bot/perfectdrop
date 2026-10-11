#!/usr/bin/env python3
"""Build a fresh iOS export, preserving a verifiable source/payload receipt."""
import argparse
import datetime
import importlib.util
import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
spec = importlib.util.spec_from_file_location("ios_receipt", ROOT / "scripts/verify-ios-candidate.py")
receipt = importlib.util.module_from_spec(spec)
spec.loader.exec_module(receipt)
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("export", type=Path)
parser.add_argument("--simulator", action="store_true")
parser.add_argument("--unity", default="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity")
args = parser.parse_args()
export = args.export.resolve()
if export.exists():
    raise SystemExit("Refusing to overwrite an existing candidate export.")
source = receipt.gate.fingerprint(ROOT)
settings = ROOT / "ProjectSettings/ProjectSettings.asset"
before = settings.read_bytes()
log = export.parent / (export.name + "-unity-build.log")
export.parent.mkdir(parents=True, exist_ok=True)
command = [args.unity, "-batchmode", "-quit", "-buildTarget", "iOS",
           "-projectPath", str(ROOT), "-executeMethod",
           "BuildAutomation.BuildIOSSimulatorQa" if args.simulator else "BuildAutomation.BuildIOS",
           "-buildOutput", str(export), "-logFile", str(log)]
result = subprocess.run(command, cwd=ROOT)
now = settings.read_bytes()
# Only Unity's known serialization whitespace is harmless. SDK/version changes
# or any other semantic change invalidate the frozen candidate.
if now.replace(b"m_BuildTarget: \n", b"m_BuildTarget:\n").rstrip() == before.replace(b"m_BuildTarget: \n", b"m_BuildTarget:\n").rstrip():
    settings.write_bytes(before)
if result.returncode:
    raise SystemExit("Unity export failed; see retained log: " + str(log))
if receipt.gate.fingerprint(ROOT) != source:
    raise SystemExit("Unity source changed during export; no trusted receipt written.")
if not (export / "Unity-iPhone.xcodeproj/project.pbxproj").is_file():
    raise SystemExit("Successful export did not produce the expected Xcode project.")
record = {"sourceSha256": source, "exportPath": str(export),
          "configuration": "Development simulator" if args.simulator else "Release",
          "unityExitCode": result.returncode, "builtAt": datetime.datetime.now(datetime.timezone.utc).isoformat(),
          "log": str(log), "command": command, "files": receipt.payload(export)}
(export / receipt.RECEIPT).write_text(json.dumps(record, indent=2) + "\n")
print("Fresh native export receipt written for source " + source, flush=True)
