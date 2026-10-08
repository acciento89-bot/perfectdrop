#if UNITY_EDITOR
using System.IO;
using System;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class CommerceBuildHooks
    {
        public static void ValidateExistingIosExport()
        {
            var args=Environment.GetCommandLineArgs();var index=Array.IndexOf(args,"-iosExport");
            if(index<0 || index+1>=args.Length)throw new InvalidOperationException("Missing -iosExport path.");
            ValidateIosAds(args[index+1]);
        }
        public static void ValidateIosAds(string output)
        {
            var xml=new XmlDocument();xml.XmlResolver=null;xml.Load(Path.Combine(output,"Info.plist"));
            var id=xml.SelectSingleNode("/plist/dict/key[.='GADApplicationIdentifier']/following-sibling::string[1]")?.InnerText;
            if(string.IsNullOrEmpty(id) || !id.StartsWith("ca-app-pub-") || !id.Contains("~"))
                throw new InvalidOperationException("iOS AdMob application ID is missing from exported Info.plist; native app would abort at startup.");
            if(xml.SelectSingleNode("/plist/dict/key[.='SKAdNetworkItems']/following-sibling::array[1]")==null)
                throw new InvalidOperationException("Google SKAdNetwork declaration missing from exported Info.plist.");
        }
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
