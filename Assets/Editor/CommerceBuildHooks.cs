#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class CommerceBuildHooks
    {
        // EDM generates the Podfile at 40 and installs at 50. Use the official
        // CocoaPods CDN instead of cloning the multi-gigabyte Specs Git history.
        [PostProcessBuild(45)]
        private static void UsePodCdn(BuildTarget target,string output)
        {
            if(target!=BuildTarget.iOS)return;
            var path=Path.Combine(output,"Podfile");if(!File.Exists(path))return;
            var text=File.ReadAllText(path).Replace("source 'https://github.com/CocoaPods/Specs'\n","");
            if(!text.Contains("source 'https://cdn.cocoapods.org/'"))text="source 'https://cdn.cocoapods.org/'\n"+text;
            if(!text.Contains("post_install do |installer|"))text+=@"
post_install do |installer|
  installer.pods_project.targets.each do |target|
    target.build_configurations.each do |config|
      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '15.0'
    end
  end
end
";
            File.WriteAllText(path,text);
        }
    }
}
#endif
