using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Logic.ResMgr;
using Common.Util;

public class ProjectBuilder
{
    [MenuItem("Tools/Export Android Project", false, 100)]
    public static void ExportAndroidProject()
    {
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        AssetDatabase.SaveAssets();
        string datapath = Application.dataPath;
        string root = datapath.Replace("Assets", string.Empty);
        DirectoryInfo dir = new DirectoryInfo(root);
        string path = Path.Combine(dir.Parent.FullName, "android");
        Debug.LogFormat(path);
        if (System.IO.Directory.Exists(path))
            System.IO.Directory.Delete(path, true);
        System.IO.Directory.CreateDirectory(path);
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.ConnectWithProfiler | BuildOptions.AcceptExternalModificationsToPlayer);
    }

    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    [MenuItem("Tools/Export XCode Project", false, 100)]
    public static void ExportXCodeProject()
    {
        string datapath = Application.dataPath;
        string path = datapath.Replace(@"Assets", string.Empty) + "xcode";
        Debug.LogFormat("path:{0}", path);
        if (System.IO.Directory.Exists(path))
            System.IO.Directory.Delete(path, true);
        System.IO.Directory.CreateDirectory(path);
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.ConnectWithProfiler | BuildOptions.AcceptExternalModificationsToPlayer);
    }

    private static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene == null) continue;
            if (scene.enabled)
                names.Add(scene.path);
        }
        return names.ToArray();
    }

    #region build ab
    /*
    static ManifestInfo manifest = null, modelManifest = null;
    public static string abPath
    {
        get
        {
            string result = string.Empty;
            DirectoryInfo dir = new DirectoryInfo("Application.dataPath");
            result = dir.Parent.Parent + "/ab/";
            result = result.Replace('/', System.IO.Path.DirectorySeparatorChar);
            result = result.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            return result;
        }
    }

    public static void BuildAssetBundleForAndroid(string appVersion, string version, bool abs, bool config, bool bin, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.android;
        BuildTarget buildTarget = BuildTarget.Android;
        Build(appVersion, version, abs, config, bin, eResPlatform, buildTarget, debug);
    }

    public static void BuildAssetBundleForIOS(string appVersion, string version, bool abs, bool config, bool bin, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.iOS;
        BuildTarget buildTarget = BuildTarget.iOS;
        Build(appVersion, version, abs, config, bin, eResPlatform, buildTarget, debug);
    }

    public static void BuildAssetBundleForPC(string appVersion, string version, bool abs, bool config, bool bin, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.standalonewindows;
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        Build(appVersion, version, abs, config, bin, eResPlatform, buildTarget, debug);
    }

    private static void Build(string appVersion, string version, bool abs, bool config, bool bin, EResPlatform eResPlatform, BuildTarget buildTarget, bool debug = false)
    {
        if (string.IsNullOrEmpty(version)) return;
        string path = abPath + eResPlatform.ToString() + "/" + appVersion + "/" + version + "/";
        if (abs && Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget))
            {
                if (abs)
                    BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget);
                if (config)
                    GenerateLuaAndConfig(path);
                if (bin)
                    GenerateBinFiles(path);
                manifest = new ManifestInfo();
#if LOAD_MODEL_REAL_TIME
                modelManifest = new ManifestInfo();
#endif
                if (WriteManifest(BuildTarget.Android, path, appVersion, version, abs) && UpdateVersionFile(buildTarget, appVersion, version, debug))
                {
                    Debugger.Log("{0}打包成功!{1} ", eResPlatform.ToString(), path);
                }
            }
        }
        else
        {
            if (abs)
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget);
            if (config)
                GenerateLuaAndConfig(path);
            if (bin)
                GenerateBinFiles(path);
            manifest = new ManifestInfo();
#if LOAD_MODEL_REAL_TIME
            modelManifest = new ManifestInfo();
#endif
            if (WriteManifest(buildTarget, path, appVersion, version, abs) && UpdateVersionFile(buildTarget, appVersion, version, debug))
            {
                Debugger.Log("{0}打包成功!{1} ", eResPlatform.ToString(), path);
            }
        }
    }

    public static void BuildEmptyForAndroid(string appVersion, string version, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.android;
        BuildTarget buildTarget = BuildTarget.Android;
        BuildEmpty(appVersion, version, eResPlatform, buildTarget, debug);
    }

    public static void BuildEmptyForIOS(string appVersion, string version, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.iOS;
        BuildTarget buildTarget = BuildTarget.iOS;
        BuildEmpty(appVersion, version, eResPlatform, buildTarget, debug);
    }

    public static void BuildEmptyForPC(string appVersion, string version, bool debug = false)
    {
        EResPlatform eResPlatform = EResPlatform.standalonewindows;
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        BuildEmpty(appVersion, version, eResPlatform, buildTarget, debug);
    }

    private static void BuildEmpty(string appVersion, string version, EResPlatform eResPlatform, BuildTarget buildTarget, bool debug = false)
    {
        if (string.IsNullOrEmpty(version)) return;
        string path = abPath + eResPlatform.ToString() + "/" + appVersion + "/" + version + "/";
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        manifest = new ManifestInfo();
#if LOAD_MODEL_REAL_TIME
        modelManifest = new ManifestInfo();
#endif
        if (WriteEmptyManifest(buildTarget, path, appVersion, version) && UpdateVersionFile(buildTarget, appVersion, version, debug))
        {
            Debugger.Log("{0}打包成功!{1} ", eResPlatform.ToString(), path);
        }
    }

    public static void GenerateBinFiles(string path)
    {
        if (!System.IO.Directory.Exists("Assets/StreamingAssets"))
        {
            Debugger.Log("StreamingAssets directory is not exist !");
            return;
        }
        DirectoryInfo originalDI = new DirectoryInfo("Assets/StreamingAssets");
        foreach (FileInfo fi in originalDI.GetFiles("*.bin", SearchOption.AllDirectories))
        {
            string name = fi.FullName;
            //Debugger.Log(fi.Extension);
            name = name.Substring(name.IndexOf(@"StreamingAssets") + 16);
            name = path + "/" + name;
            name = name.Replace('/', System.IO.Path.DirectorySeparatorChar);
            name = name.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            //Debugger.Log(name);
            string fileDir = name.Substring(0, name.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            //Debugger.Log(fileDir);
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            fi.CopyTo(name, true);
        }

        foreach (FileInfo fi in originalDI.GetFiles("*.mp3", SearchOption.AllDirectories))
        {
            string name = fi.FullName;
            //Debugger.Log(fi.Extension);
            name = name.Substring(name.IndexOf(@"StreamingAssets") + 16);
            name = path + "/" + name;
            name = name.Replace('/', System.IO.Path.DirectorySeparatorChar);
            name = name.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            //Debugger.Log(name);
            string fileDir = name.Substring(0, name.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            //Debugger.Log(fileDir);
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            fi.CopyTo(name, true);
        }
    }

    public static void GenerateLuaAndConfig(string path)
    {
        if (!System.IO.Directory.Exists("Assets/XLua/Resources"))
        {
            Debugger.LogError("XLua Resources directory is not exist !");
        }
        else
        {
            DirectoryInfo originalDI = new DirectoryInfo("Assets/XLua/Resources");
            foreach (FileInfo fi in originalDI.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                string targetPath = fi.FullName;
                //Debugger.Log(fi.Extension);
                targetPath = targetPath.Substring(targetPath.IndexOf(@"Resources") + 10);
                targetPath = targetPath.Replace(".txt", string.Empty);//处理lua文件，因为lua文件的命名为*.lua.txt
                targetPath = path + "/" + targetPath;
                targetPath = targetPath.Replace('/', System.IO.Path.DirectorySeparatorChar);
                targetPath = targetPath.Replace('\\', System.IO.Path.DirectorySeparatorChar);
                //Debugger.Log(name);
                string fileDir = targetPath.Substring(0, targetPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                //Debugger.Log(fileDir);
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                string content = File.ReadAllText(fi.FullName, System.Text.UTF8Encoding.UTF8);
                byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(content);
                byte[] encryptBytes = EncryptUtil.AESEncrypt(ApplicationConfig.key, bytes);
                byte[] prefix = ApplicationConfig.prefixBytes;
                byte[] result = new byte[encryptBytes.Length + prefix.Length];
                System.Array.Copy(prefix, result, prefix.Length);
                System.Array.Copy(encryptBytes, 0, result, prefix.Length, encryptBytes.Length);
                File.WriteAllBytes(targetPath, result);
            }
        }
        if (!System.IO.Directory.Exists("Assets/_Res/Resources/config"))
        {
            Debugger.LogError("config directory is not exist !");
        }
        else
        {
            DirectoryInfo originalDI = new DirectoryInfo("Assets/_Res/Resources/config");
            foreach (FileInfo fi in originalDI.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (fi.FullName.Contains(".meta"))
                    continue;
                string targetPath = fi.FullName;
                //Debugger.Log(name);
                targetPath = targetPath.Substring(targetPath.IndexOf(@"Resources") + 10);
                targetPath = path + "/" + targetPath;
                targetPath = targetPath.Replace('/', System.IO.Path.DirectorySeparatorChar);
                targetPath = targetPath.Replace('\\', System.IO.Path.DirectorySeparatorChar);
                //Debugger.Log(name);
                string fileDir = targetPath.Substring(0, targetPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                //Debugger.Log(fileDir);
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                byte[] bytes = File.ReadAllBytes(fi.FullName);
                byte[] encryptBytes = EncryptUtil.AESEncrypt(ApplicationConfig.key, bytes);
                byte[] prefix = ApplicationConfig.prefixBytes;
                byte[] result = new byte[encryptBytes.Length + prefix.Length];
                System.Array.Copy(prefix, result, prefix.Length);
                System.Array.Copy(encryptBytes, 0, result, prefix.Length, encryptBytes.Length);
                File.WriteAllBytes(targetPath, result);
            }
        }
    }

    private static bool WriteEmptyManifest(BuildTarget bt, string path, string appVersion, string version)
    {
        string platformPath = string.Empty;
        switch (bt)
        {
            case BuildTarget.Android:
                platformPath = EResPlatform.android.ToString();
                break;
            case BuildTarget.iOS:
                platformPath = EResPlatform.iOS.ToString();
                break;
            case BuildTarget.StandaloneWindows64:
                platformPath = EResPlatform.standalonewindows.ToString();
                break;
        }

        manifest.version = version;
        manifest.appVersion = appVersion;
#if LOAD_MODEL_REAL_TIME
        modelManifest.version = version;
        modelManifest.appVersion = appVersion;
#endif
        byte[] bytes = ResUtil.GetBytesFromManifest(manifest);
        File.WriteAllBytes(path + ResUtil.MANIFESTNAME, bytes);
        Debug.Log(manifest.ToString());
#if LOAD_MODEL_REAL_TIME
        bytes = ResUtil.GetBytesFromManifest(modelManifest);
        File.WriteAllBytes(path + ResUtil.MODELMANIFESTNAME, bytes);
        Debug.Log(modelManifest.ToString());
#endif
        return true;
    }

    private static bool WriteManifest(BuildTarget bt, string path, string appVersion, string version, bool abs)
    {
        string platformPath = string.Empty;
        switch (bt)
        {
            case BuildTarget.Android:
                platformPath = EResPlatform.android.ToString();
                break;
            case BuildTarget.iOS:
                platformPath = EResPlatform.iOS.ToString();
                break;
            case BuildTarget.StandaloneWindows64:
                platformPath = EResPlatform.standalonewindows.ToString();
                break;
        }
        AssetInfo assetInfo = null;
        DirectoryInfo dir = new DirectoryInfo(path);
        //遍历ab文件
        foreach (var ab in dir.GetFiles("*" + ResUtil.ASSET_BUNDLE_SUFFIX, SearchOption.AllDirectories))
        {
            string fileName = ab.FullName.Replace(ResUtil.ASSET_BUNDLE_SUFFIX, string.Empty);
            //Debugger.LogError(abDir.FullName);
            fileName = fileName.Substring(fileName.IndexOf(dir.FullName) + dir.FullName.Length);
            fileName = fileName.Replace(@"\", @"/");
            //Debugger.LogError(fileName);
            assetInfo = new AssetInfo(fileName);
            assetInfo.filePathList = new List<string>() { assetInfo.subPath };//只打包一个文件
            assetInfo.createDate = ab.CreationTime.Ticks;
            byte[] datas = File.ReadAllBytes(ab.FullName);
            string md5 = EncryptUtil.Bytes2MD5(datas);
            assetInfo.md5 = md5;
            assetInfo.Length = ab.Length;
            assetInfo.suffix = ResUtil.ASSET_BUNDLE_SUFFIX;
#if LOAD_MODEL_REAL_TIME            
            if (ab.FullName.Contains(version + @"\model") && !ab.FullName.Contains(@"model\materials"))
                modelManifest.assetDic.Add(assetInfo.subPath, assetInfo);
            else
                manifest.assetDic.Add(assetInfo.subPath, assetInfo);
#else
            manifest.assetDic.Add(assetInfo.subPath, assetInfo);
#endif
        }
        //遍历lua和config文件，以及bin文件
        foreach (var f in dir.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (f.FullName.Contains(ResUtil.MANIFESTNAME) || f.FullName.Contains(ResUtil.MODELMANIFESTNAME)) continue;
            if (!f.FullName.EndsWith(".lua") && !f.FullName.EndsWith(".txt") && !f.FullName.EndsWith(".csv") && !f.FullName.EndsWith(".bin") && !f.FullName.EndsWith(".mp3"))
                continue;
            //Debugger.LogError(ab.FullName);
            string suffix = f.Extension;
            string fileName = f.FullName.Replace(suffix, string.Empty);
            //Debugger.LogError(abDir.FullName);
            fileName = fileName.Substring(fileName.IndexOf(dir.FullName) + dir.FullName.Length);
            fileName = fileName.Replace(@"\", @"/");
            //Debugger.LogError(fileName);
            assetInfo = new AssetInfo(fileName);
            assetInfo.filePathList = new List<string>() { assetInfo.subPath };//只打包一个文件
            assetInfo.createDate = f.CreationTime.Ticks;
            byte[] datas = File.ReadAllBytes(f.FullName);
            string md5 = EncryptUtil.Bytes2MD5(datas);
            assetInfo.md5 = md5;
            assetInfo.Length = f.Length;
            assetInfo.suffix = suffix;
            manifest.assetDic.Add(assetInfo.subPath, assetInfo);
        }
        byte[] bytes = null;
        if (abs)
        {
            //assetbundle manifest
            assetInfo = new AssetInfo(version);
            assetInfo.filePathList = new List<string>() { assetInfo.subPath };
            //Debugger.Log(abPath+ResConf.eResPlatform.ToString());
            FileInfo assetbundleManifest = new FileInfo(path + version);
            bytes = File.ReadAllBytes(assetbundleManifest.FullName);
            string encodeMD5 = EncryptUtil.Bytes2MD5(bytes);
            assetInfo.md5 = encodeMD5;
            assetInfo.createDate = assetbundleManifest.CreationTime.Ticks;
            assetInfo.Length = assetbundleManifest.Length;
            manifest.assetDic.Add(assetInfo.subPath, assetInfo);
        }

        manifest.version = version;
        manifest.appVersion = appVersion;
#if LOAD_MODEL_REAL_TIME
        modelManifest.version = version;
        modelManifest.appVersion = appVersion;
#endif
        bytes = ResUtil.GetBytesFromManifest(manifest);
        File.WriteAllBytes(path + ResUtil.MANIFESTNAME, bytes);
        Debug.Log(manifest.ToString());
#if LOAD_MODEL_REAL_TIME
        bytes = ResUtil.GetBytesFromManifest(modelManifest);
        File.WriteAllBytes(path + ResUtil.MODELMANIFESTNAME, bytes);
        Debug.Log(modelManifest.ToString());
#endif
        return true;
    }

    private static bool UpdateVersionFile(BuildTarget bt, string appVersion, string version, bool debug)
    {
        string platformPath = string.Empty;
        switch (bt)
        {
            case BuildTarget.Android:
                platformPath = EResPlatform.android.ToString();
                break;
            case BuildTarget.iOS:
                platformPath = EResPlatform.iOS.ToString();
                break;
            case BuildTarget.StandaloneWindows64:
                platformPath = EResPlatform.standalonewindows.ToString();
                break;
        }
        string fullPath = abPath + platformPath + "/" + "version.txt";
        string str = "app:1.0.0.0\r\n" + platformPath + ":1\r\n" + "debug:0\r\n";
        File.WriteAllText(fullPath, str);
        string[] contents = CSVUtil.SplitLines(File.ReadAllText(fullPath));
        string result = string.Empty;
        for (int i = 0, length = contents.Length; i < length; i++)
        {
            string content = contents[i];
            if (content.Contains("app"))
                result += "app:" + appVersion + "\r\n";
            else if (content.Contains(platformPath))
                result += platformPath + ":" + version + "\r\n";
            else if (content.Contains("debug"))
                result += "debug" + ":" + (debug ? 1 : 0);
        }
        Debugger.Log("versionInfo:{0}", result);
        File.WriteAllText(fullPath, result);
        return true;
    }
    */
    #endregion
}
