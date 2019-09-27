/*  主要用于资源导入时批量处理资源，比如设置assetbundle的tag等    */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Common.PreImprot.Editor
{
    /*
    public class PreImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var str in importedAssets)
            {
                //Debug.Log(str);
                if (str.EndsWith(".cs") || str.EndsWith(".js") || str.EndsWith(".asset"))
                    continue;
                if (!str.Contains(".")) continue;
                Debug.LogFormat("str:{0}", str);
                //设置atlas ab tag
                if (str.Contains(@"Assets/Res/Atlas") && (str.EndsWith(".png") || str.EndsWith(".tga")))
                {
                    AssetImporter assetImport = AssetImporter.GetAtPath(str);
                    if (str.Contains(@"Assets/Res/Atlas/always"))//always目录下的不打包ab
                    {
                        assetImport.assetBundleName = string.Empty;
                    }
                    else
                    {
                        string fileName = str.Substring(str.LastIndexOf("/"));
                        string assetBundleName = str.Replace(@"Assets/_Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(fileName, string.Empty);
                        assetBundleName = assetBundleName.Replace("Atlas", "sprite");
                        //Debug.Log(assetBundleName+"   "+ fileName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = Logic.ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                }

                //设置ab tag
                if (str.Contains(@"_Res/Resources/models") || str.Contains(@"_Res/Materials") || str.Contains(@"_Res/Resources/font") || str.Contains(@"_Res/Resources/ui"))
                {
                    AssetImporter assetImport = AssetImporter.GetAtPath(str);
                    if (str.Contains(@"_Res/Resources/ui/download") || str.Contains(@"_Res/Resources/ui/password"))//download ui不打包ab
                    {
                        assetImport.assetBundleName = string.Empty;
                    }
                    else
                    {
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debug.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/_Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = Logic.ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                }
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }*/
}
