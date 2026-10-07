#!/usr/bin/env python3
"""Snapshot bundle content independently of replaceable JAR signatures."""
import argparse
import hashlib
import json
import re
import zipfile

parser = argparse.ArgumentParser()
parser.add_argument("bundle")
parser.add_argument("--unsigned-output")
args = parser.parse_args()
signature = re.compile(r"META-INF/(?:MANIFEST\.MF|[^/]+\.(?:SF|RSA|DSA|EC)|SIG-[^/]+)$", re.I)
with zipfile.ZipFile(args.bundle) as source:
    names = source.namelist()
    if len(names) != len(set(names)):
        raise ValueError("Duplicate bundle entries")
    if any(n.startswith("/") or ".." in n.split("/") for n in names):
        raise ValueError("Unsafe bundle entry path")
    if any(i.flag_bits & 1 for i in source.infolist()):
        raise ValueError("Encrypted bundle entries")
    payload = {n: hashlib.sha256(source.read(n)).hexdigest()
               for n in sorted(names) if not signature.fullmatch(n)}
    if "base/manifest/AndroidManifest.xml" not in payload:
        raise ValueError("Missing Android base manifest")
    if args.unsigned_output:
        with zipfile.ZipFile(args.unsigned_output, "w") as target:
            for entry in source.infolist():
                if not signature.fullmatch(entry.filename):
                    target.writestr(entry, source.read(entry.filename))
print(json.dumps(payload, sort_keys=True, separators=(",", ":")))
