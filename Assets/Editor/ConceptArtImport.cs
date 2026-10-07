#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class ConceptArtImport : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if(!assetPath.StartsWith("Assets/Resources/Art/CloudCity"))return;
        var importer=(TextureImporter)assetImporter;
        importer.npotScale=TextureImporterNPOTScale.None;
        importer.wrapMode=TextureWrapMode.Clamp;
        importer.mipmapEnabled=false;
        importer.maxTextureSize=2048;
        importer.textureCompression=TextureImporterCompression.Compressed;
        importer.compressionQuality=75;
    }
}
#endif
