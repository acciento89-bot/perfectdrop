#!/bin/bash
set -euo pipefail

if [ "${CONFIRM_TESTFLIGHT_UPLOAD:-}" != "YES" ]; then
  echo "Refusing upload. Set CONFIRM_TESTFLIGHT_UPLOAD=YES only after the RC QA gates pass." >&2
  exit 2
fi

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
python3 "$ROOT/scripts/verify-visual-release.py"

XCODE_PROJECT="${1:-/private/tmp/PerfectDrop-iOS}"
ARCHIVE_PATH="${2:-/private/tmp/PerfectDrop-TestFlight.xcarchive}"
EXPORT_PLIST="$ROOT/scripts/testflight-export-options.plist"
TEAM_ID="TKG684N5GL"

if [ ! -f "$XCODE_PROJECT/Unity-iPhone.xcodeproj/project.pbxproj" ]; then
  echo "Missing Unity Xcode project at: $XCODE_PROJECT" >&2
  exit 3
fi
python3 "$ROOT/scripts/verify-ios-candidate.py" "$XCODE_PROJECT"
if [ -e "$ARCHIVE_PATH" ]; then
  echo "Refusing to overwrite an existing archive: $ARCHIVE_PATH" >&2
  exit 4
fi
PROJECT_ARGS=(-project "$XCODE_PROJECT/Unity-iPhone.xcodeproj")
if [ -d "$XCODE_PROJECT/Unity-iPhone.xcworkspace" ]; then
  PROJECT_ARGS=(-workspace "$XCODE_PROJECT/Unity-iPhone.xcworkspace")
fi

: "${DEVELOPER_DIR:=/Applications/Xcode-27.1-RC.app/Contents/Developer}"
export DEVELOPER_DIR

xcodebuild \
  "${PROJECT_ARGS[@]}" \
  -scheme Unity-iPhone \
  -configuration Release \
  -destination 'generic/platform=iOS' \
  -archivePath "$ARCHIVE_PATH" \
  -derivedDataPath "${ARCHIVE_PATH}.derived-data" \
  -allowProvisioningUpdates \
  DEVELOPMENT_TEAM="$TEAM_ID" \
  CODE_SIGN_STYLE=Automatic \
  archive

xcodebuild \
  -exportArchive \
  -archivePath "$ARCHIVE_PATH" \
  -exportOptionsPlist "$EXPORT_PLIST" \
  -allowProvisioningUpdates
