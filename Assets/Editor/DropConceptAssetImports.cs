#if UNITY_EDITOR
using UnityEditor;
public sealed class DropConceptAssetImports:AssetPostprocessor
{
 public override uint GetVersion()=>4;
 void OnPreprocessTexture()
 {
  if(assetPath.StartsWith("Assets/Resources/Art/ConceptV3/")){var card=(TextureImporter)assetImporter;card.textureType=TextureImporterType.Default;card.sRGBTexture=true;card.alphaIsTransparency=true;card.mipmapEnabled=false;card.isReadable=false;card.filterMode=UnityEngine.FilterMode.Bilinear;card.wrapMode=UnityEngine.TextureWrapMode.Clamp;card.maxTextureSize=2048;card.textureCompression=TextureImporterCompression.CompressedHQ;return;}
  if(!assetPath.StartsWith("Assets/Resources/Art/ConceptV2/"))return;
  var texture=(TextureImporter)assetImporter;texture.textureType=TextureImporterType.Default;texture.sRGBTexture=true;texture.alphaIsTransparency=(assetPath.EndsWith("GoldAction.png")||assetPath.EndsWith("GoldCoin.png"));texture.mipmapEnabled=!texture.alphaIsTransparency;texture.isReadable=false;texture.filterMode=UnityEngine.FilterMode.Trilinear;texture.wrapMode=assetPath.Contains("Albedo")?UnityEngine.TextureWrapMode.Repeat:UnityEngine.TextureWrapMode.Clamp;texture.maxTextureSize=texture.alphaIsTransparency?4096:1024;texture.textureCompression=TextureImporterCompression.CompressedHQ;
 }
}
#endif
