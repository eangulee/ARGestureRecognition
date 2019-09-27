/*  主要用于贴图资源导入时批量处理   */
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
namespace Common.PreImprot.Editor
{
    public class PreImportTexture : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("Res/Atlas"))
            {
                string atlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
                TextureImporter textureImporter = assetImporter as TextureImporter;
                //string assetName = assetPath.Substring(assetPath.LastIndexOf("/") + 1);
                //assetName = assetName.Replace(".png", string.Empty);
                //if (atlasName == "Atlas")
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spritePackingTag = atlasName;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.filterMode = FilterMode.Bilinear;
                    textureImporter.maxTextureSize = 2048;
                    textureImporter.isReadable = false;

                    textureImporter.ClearPlatformTextureSettings("android");
                    textureImporter.ClearPlatformTextureSettings("iPhone");
                    textureImporter.ClearPlatformTextureSettings("Standalone");

#if UNITY_ANDROID
                    TextureImporterPlatformSettings androidTextureImporterPlatformSettings = new TextureImporterPlatformSettings();
                    androidTextureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
                    androidTextureImporterPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    androidTextureImporterPlatformSettings.overridden = true;
                    textureImporter.SetPlatformTextureSettings(androidTextureImporterPlatformSettings);
#endif
#if UNITY_IOS
                    TextureImporterPlatformSettings iOSTextureImporterPlatformSettings = new TextureImporterPlatformSettings();
                    iOSTextureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
                    iOSTextureImporterPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    iOSTextureImporterPlatformSettings.overridden = true;
                    textureImporter.SetPlatformTextureSettings(iOSTextureImporterPlatformSettings);
#endif
                }
            }

            if (assetPath.Contains("Res/Resources/textures"))
            {
                return;
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Trilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.anisoLevel = 0;

                textureImporter.ClearPlatformTextureSettings("iPhone");
                textureImporter.ClearPlatformTextureSettings("Android");
                textureImporter.ClearPlatformTextureSettings("Standalone");

#if UNITY_ANDROID
                TextureImporterPlatformSettings androidTextureImporterPlatformSettings = new TextureImporterPlatformSettings();
                androidTextureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
                androidTextureImporterPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                androidTextureImporterPlatformSettings.overridden = true;
                textureImporter.SetPlatformTextureSettings(androidTextureImporterPlatformSettings);
#endif
#if UNITY_IOS
                    TextureImporterPlatformSettings iOSTextureImporterPlatformSettings = new TextureImporterPlatformSettings();
                    iOSTextureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
                    iOSTextureImporterPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    iOSTextureImporterPlatformSettings.overridden = true;
                    textureImporter.SetPlatformTextureSettings(iOSTextureImporterPlatformSettings);
#endif
            }

            if (assetPath.Contains("Res/Resources/texture_720"))// 处理720图片
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.anisoLevel = 0;

                textureImporter.ClearPlatformTextureSettings("iPhone");
                textureImporter.ClearPlatformTextureSettings("Android");
                textureImporter.ClearPlatformTextureSettings("Standalone");

                TextureImporterPlatformSettings androidTextureImporterPlatformSettings = new TextureImporterPlatformSettings();
                androidTextureImporterPlatformSettings.format = TextureImporterFormat.ETC_RGB4Crunched;
                androidTextureImporterPlatformSettings.textureCompression = TextureImporterCompression.CompressedHQ;
                androidTextureImporterPlatformSettings.overridden = true;
                androidTextureImporterPlatformSettings.crunchedCompression = true;
                androidTextureImporterPlatformSettings.maxTextureSize = 4096;
                androidTextureImporterPlatformSettings.compressionQuality = 100;
                textureImporter.SetPlatformTextureSettings(androidTextureImporterPlatformSettings);

            }
            AssetDatabase.SaveAssets();
        }
    }
}