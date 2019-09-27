///*
// * @Author:lily
// * @Create Time:2017-07-23 17:00:43
// */
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditor.Callbacks;
//using UnityEditor.iOS.Xcode;
//using System.IO;

//public class ExportSettings
//{

//    [MenuItem("Tools/CopyDatas", false, 200)]
//    public static void CopyDatas()
//    {
//#if UNITY_IOS
//        string root = Application.dataPath.Substring(0, Application.dataPath.IndexOf("SrcCode"));
//        string path = Path.Combine(root, "xcode");
//        OnPostprocessBuild(BuildTarget.iOS, path);
//#elif UNITY_ANDROID
////		string root = Application.dataPath.Substring (0, Application.dataPath.IndexOf ("SrcCode"));
////		string path = Path.Combine (root, "androidProject");
////		OnPostprocessBuild (BuildTarget.iOS, path);
//#endif
//    }

//    [MenuItem("Tools/FindRemovesInNative", false, 200)]
//    public static void FindRemovesInNative()
//    {
//        string root = Application.dataPath.Substring(0, Application.dataPath.IndexOf("SrcCode"));
//        Debug.Log(root);
//        string newPath = Path.Combine(root, "xcodeProject");
//        string path = Path.Combine(root, "xcode");
//        List<string> diffs = GetDiffFiles(Path.Combine(newPath, "Classes/Native"), Path.Combine(path, "Classes/Native"));
//        Debug.Log("remove file list-----------------------------------------------------------------");
//        foreach (var file in diffs)
//        {
//            string fileName = file.Replace(newPath + "/", string.Empty);
//            Debug.LogFormat("fileName:{0}", fileName);
//        }
//        Debug.Log("-------------------------------------------------------------------------------");
//    }

//    // 简单函数(http://goo.gl/fzyig8作为参考)。
//    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
//    {
//        if (Directory.Exists(dstPath))
//            Directory.Delete(dstPath, true);
//        if (File.Exists(dstPath))
//            File.Delete(dstPath);

//        Directory.CreateDirectory(dstPath);

//        foreach (var file in Directory.GetFiles(srcPath))
//            File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

//        foreach (var dir in Directory.GetDirectories(srcPath))
//            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
//    }

//    internal static List<string> GetDiffFiles(string originalDir, string targetDir, string searchPattern = "*.cpp")
//    {
//        List<string> originals = new List<string>(Directory.GetFiles(originalDir, searchPattern));
//        List<string> targets = new List<string>(Directory.GetFiles(targetDir, searchPattern));
//        List<string> newTargets = new List<string>();
//        List<string> diffs = new List<string>();
//        foreach (var file in targets)
//        {
//            newTargets.Add(file.Replace(targetDir, originalDir));
//        }

//        foreach (var file in originals)
//        {
//            if (!newTargets.Contains(file))
//                diffs.Add(file);
//        }
//        return diffs;
//    }

//    //[PostProcessBuild]
//    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
//    {
//        if (buildTarget == BuildTarget.iOS)
//        {
//            //string newPath = path.Replace ("xcode", "xcodeProject");
//            string root = Application.dataPath.Substring(0, Application.dataPath.IndexOf("SrcCode"));
//            string newPath = Path.Combine(root, "xcodeProject");
//            Debug.LogFormat("build target:{0} ,path:{1} ,newPath:{2}", buildTarget.ToString(), path, newPath);
//            string projPath = PBXProject.GetPBXProjectPath(newPath);
//            Debug.LogFormat("projPath:{0}", projPath);
//            PBXProject proj = new PBXProject();

//            proj.ReadFromString(File.ReadAllText(projPath));
//            string target = proj.TargetGuidByName("Unity-iPhone");
//            Debug.LogFormat("target:{0}", target);

//            List<string> diffs = GetDiffFiles(Path.Combine(newPath, "Classes/Native"), Path.Combine(path, "Classes/Native"));
//            Debug.Log("diff file list-----------------------------------------------------------------");
//            foreach (var file in diffs)
//            {
//                string fileName = file.Replace(newPath + "/", string.Empty);
//                if (proj.ContainsFileByProjectPath(fileName))
//                {
//                    string guid = proj.FindFileGuidByProjectPath(fileName);
//                    Debug.LogFormat("fileName:{0},guid:{1}", fileName, guid);
//                    proj.RemoveFile(guid);
//                    proj.RemoveFileFromBuild(target, guid);
//                }
//            }
//            Debug.Log("-------------------------------------------------------------------------------");
//            CopyAndReplaceDirectory(Path.Combine(path, "Data"), Path.Combine(newPath, "Data"));
//            CopyAndReplaceDirectory(Path.Combine(path, "Classes/Native"), Path.Combine(newPath, "Classes/Native"));

//            /*
//			// 追加系统的框架
//			proj.AddFrameworkToProject(target, "AssetsLibrary.framework", false);

//			// 追加自定义的框架
//			CopyAndReplaceDirectory("Assets/Lib/mylib.framework", Path.Combine(path, "Frameworks/mylib.framework"));
//			proj.AddFileToBuild(target, proj.AddFile("Frameworks/mylib.framework", "Frameworks/mylib.framework", PBXSourceTree.Source));
//			*/

//            // 追加文件
//            //			var fileName = "my_file.xml";
//            //			var filePath = Path.Combine("Assets/Lib", fileName);
//            //			File.Copy(filePath, Path.Combine(path, fileName));
//            //			proj.AddFileToBuild(target, proj.AddFile(fileName, fileName, PBXSourceTree.Source));

//            string nativePath = Path.Combine(newPath, "Classes/Native/");
//            int count = 0;
//            foreach (var file in Directory.GetFiles(nativePath, "*.cpp"))
//            {
//                string fileName = file.Substring(file.LastIndexOf("/") + 1);
//                fileName = Path.Combine("Classes/Native/", fileName);
//                string guid = proj.AddFile(fileName, fileName, PBXSourceTree.Source);
//                Debug.LogFormat("fileName:{0},Guid:{1}", fileName, guid);
//                proj.AddFileToBuild(target, guid);
//                count++;
//            }
//            Debug.LogFormat("count:{0}", count);

//            /*
//			// Yosemiteでipaが書き出せないエラーに対応するための設定
//			// 用于对应于在yosemite上ipa未写入的错误的设定。
//			proj.SetBuildProperty(target, "CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");

//			// 设定、追加工作搜索路径
//			proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
//			proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
//			*/
//        }
//        else if (buildTarget == BuildTarget.Android)
//        {
//            string root = Application.dataPath.Substring(0, Application.dataPath.IndexOf("SrcCode"));
//            string newPath = Path.Combine(root, "androidProject");
//            Debug.Log(newPath);
//            //			PlayerSettings.productName
//            CopyAndReplaceDirectory(Path.Combine(path, PlayerSettings.productName + "/assets"), Path.Combine(newPath, "SDKRobotProj/assets"));
//            Debug.LogFormat("copy data from {0} to {1} sucess!", path, newPath);
//        }
//    }
//}
