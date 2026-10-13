#if UNITY_EDITOR
using UnityEditor;
public sealed class DropConceptAssetImports:AssetPostprocessor
{
 public override uint GetVersion()=>3;
 void OnPreprocessTexture()
 {
  if(!assetPath.StartsWith("Assets/Resources/Art/ConceptV2/"))return;
  var texture=(TextureImporter)assetImporter;texture.textureType=TextureImporterType.Default;texture.sRGBTexture=true;texture.alphaIsTransparency=(assetPath.EndsWith("GoldAction.png")||assetPath.EndsWith("GoldCoin.png"));texture.mipmapEnabled=!texture.alphaIsTransparency;texture.isReadable=false;texture.filterMode=UnityEngine.FilterMode.Trilinear;texture.wrapMode=assetPath.Contains("Albedo")?UnityEngine.TextureWrapMode.Repeat:UnityEngine.TextureWrapMode.Clamp;texture.maxTextureSize=texture.alphaIsTransparency?4096:1024;texture.textureCompression=TextureImporterCompression.CompressedHQ;
 }
}
#endif
