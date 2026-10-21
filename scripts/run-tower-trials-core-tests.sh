#!/bin/bash
# Root-owned execution only. No Unity Editor/player/device process is launched.
set -euo pipefail
cd -- "$(dirname -- "$0")/.."
if [ "$#" -ne 1 ]; then echo 'Provide an explicit external output .exe path.' >&2; exit 2; fi
trial_toolchain='/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting/MonoBleedingEdge'
"$trial_toolchain/bin/mono" "$trial_toolchain/lib/mono/4.5/csc.exe" -nologo -langversion:latest -target:exe "-out:$1" Assets/Scripts/Gameplay/TowerTrials.cs Assets/Scripts/Core/TowerTrialsArchive.cs tests/tower-trials/Program.cs
"$trial_toolchain/bin/mono" "$1"
