#!/usr/bin/env python3
"""Source-only release identity/guard checks. Does not compile or run Unity."""
import json, os, pathlib, re, subprocess, unittest
ROOT = pathlib.Path(__file__).resolve().parents[1]
META = json.loads((ROOT / "docs/android-release-metadata.json").read_text())
EDITOR = ROOT / "Assets/Editor/AndroidReleaseBuild.cs"

class ReleaseSourceTests(unittest.TestCase):
    def test_android_code_exceeds_saved_all_track_maximum(self):
        self.assertEqual(META["androidVersionCode"], META["liveMaxCode"] + 1)
        source = (ROOT / "ProjectSettings/ProjectSettings.asset").read_text()
        self.assertEqual(re.findall(r"^  AndroidBundleVersionCode: (\d+)$", source, re.M), [str(META["androidVersionCode"])])

    def test_shared_ios_version_build_and_package_are_preserved(self):
        source = (ROOT / "ProjectSettings/ProjectSettings.asset").read_text()
        self.assertEqual(re.findall(r"^  bundleVersion: (.*)$", source, re.M), [META["sharedIOSVersionToPreserve"]])
        block = re.search(r"^  buildNumber:\n((?:    .*\n)+)", source, re.M).group(1)
        self.assertIn("    iPhone: " + META["iosBuildToPreserve"], block)
        self.assertIn("    Android: " + META["package"], source)

    def test_android_scoped_entrypoint_has_exact_release_metadata(self):
        self.assertTrue(EDITOR.is_file(), "Scoped Android release entrypoint missing")
        source = EDITOR.read_text()
        self.assertIn('const string VersionName = "' + META["androidVersionName"] + '";', source)
        self.assertIn('const int VersionCode = ' + str(META["androidVersionCode"]) + ';', source)
        self.assertIn('const string PackageName = "' + META["package"] + '";', source)
        self.assertIn('EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android', source)

    def test_source_contains_external_fresh_output_and_restoration_guards(self):
        self.assertTrue(EDITOR.is_file(), "Scoped Android release entrypoint missing")
        source = EDITOR.read_text()
        for required in ['Path.IsPathRooted(output)', 'File.Exists(output)', 'Directory.Exists(output)',
                         'Path.GetExtension(output) != ".aab"', 'output.StartsWith(project + Path.DirectorySeparatorChar',
                         'Argument("-versionName")', 'Argument("-buildNumber")', 'finally',
                         'File.WriteAllBytes(settingsPath, originalSettings)',
                         'PlayerSettings.bundleVersion = previousVersion', 'PlayerSettings.iOS.buildNumber = previousIosBuild',
                         'PlayerSettings.Android.useCustomKeystore = false', 'ProviderIdentityAssetsRestored']:
            self.assertIn(required, source)
        self.assertNotIn('keyaliasPass', source)
        self.assertNotIn('keystorePass', source)

    def test_real_selected_signing_guards_reject_wrong_version_and_ref(self):
        path = ROOT / ".github/workflows/sign-android-bundle.yml"
        if not path.exists():
            self.assertEqual(META["app"], "Runenpfad")
            return
        text = path.read_text()
        guards = [line.strip() for line in text.splitlines() if line.strip().startswith('[[ "') or line.strip().startswith('test "$GITHUB_REF"') or line.strip().startswith('test "$VERSION_NAME"') or line.strip().startswith('test "$BUILD_NUMBER"')]
        code = "set -euo pipefail\n" + "\n".join(guards)
        env = {**os.environ, "SOURCE_ASSET":"42", "SOURCE_HASH":"a"*64, "SOURCE_COMMIT":"b"*40,
               "VERSION_NAME":META["androidVersionName"], "BUILD_NUMBER":str(META["androidVersionCode"]), "GITHUB_REF":"refs/heads/main"}
        self.assertEqual(subprocess.run(["bash"], input=code, text=True, env=env, capture_output=True).returncode, 0)
        for delta in [{"VERSION_NAME":"9.9.9"}, {"BUILD_NUMBER":str(META["androidVersionCode"]+1)}, {"GITHUB_REF":"refs/heads/feature"}]:
            with self.subTest(delta=delta):
                self.assertNotEqual(subprocess.run(["bash"], input=code, text=True, env={**env, **delta}, capture_output=True).returncode, 0)

if __name__ == "__main__":
    unittest.main()
