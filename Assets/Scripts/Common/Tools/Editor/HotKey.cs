using Common.Util;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HotKey
{
    [MenuItem("Tools/QuickHide &s")]
    public static void QuickHide()
    {
        GameObject[] obj = Selection.gameObjects;
        if (obj != null && obj.Length > 0)
        {
            foreach (var o in obj)
            {
                bool active = o.activeInHierarchy;
                active = !active;
                o.SetActive(active);
            }
        }
    }

    [MenuItem("Tools/PlayVideo")]
    public static void PlayVideo()
    {
        GameObject[] obj = Selection.gameObjects;
        for (int i = 0,length = obj.Length; i < length; i++)
        {
            VideoPlayer videoPlayer = obj[i].GetComponentInChildren<VideoPlayer>();
            if (videoPlayer)
            {
                if (videoPlayer.isPlaying)
                    videoPlayer.Stop();
                else
                    videoPlayer.Play();
            }
        }
    }

    [MenuItem("Tools/Clear Cache")]
    public static void ClearCache()
    {
        PlayerPrefs.DeleteAll();
        //EditorPrefs.DeleteAll();
    }

    [MenuItem("Tools/GC")]
    public static void GC()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }


    [MenuItem("Tools/Faster")]
    public static void Faster()
    {
        Time.timeScale = Time.timeScale * 1.2f;
    }

    [MenuItem("Tools/Normal")]
    public static void Normal()
    {
        Time.timeScale = 1f;
    }

    [MenuItem("Tools/Test &t")]
    public static void Test()
    {
        GameObject[] obj = Selection.gameObjects;
        for (int i = 0, length = obj.Length; i < length; i++)
        {
            MVideoPlayer mVideoPlayer = obj[i].GetComponentInChildren<MVideoPlayer>();
            //if (mVideoPlayer)
            //{
            //if (mVideoPlayer.isPlaying)
            //    mVideoPlayer.Stop();
            //else
            //    mVideoPlayer.Play();
            //}
            mVideoPlayer.Play(0,MVideoPlayer.MPlayType.EasyIn);
            //Debug.Log("frame:"+mVideoPlayer.frame+" ,frame count:"+mVideoPlayer.frameCount+"  ,time:"+mVideoPlayer.time+" ,length:"+mVideoPlayer.length);
            //mVideoPlayer.RegisterEvent(50f, new HotKey().Callback);
        }
    }

    private void Callback(MVideoPlayer mVideoPlayer)
    {
        Debug.Log("Callback:"+ mVideoPlayer.time+"   "+mVideoPlayer.frame);
    }

    [MenuItem("Tools/Add BaseVew", false, 300)]
    public static void AddBaseVew()
    {
        DirectoryInfo di = new DirectoryInfo("Assets/Res/Resources/ui");
        foreach (var f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
        {
            string path = f.FullName.Substring(f.FullName.IndexOf("Assets"));
            Debug.LogFormat("path:{0}", path);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go.name.EndsWith("_view"))
            {
                Logic.UI.BaseView baseView = go.GetComponent<Logic.UI.BaseView>();
                if (!baseView)
                    go.AddComponent<Logic.UI.BaseView>();
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Tools/SaveAll #&s", false, 300)]
    public static void SaveAll()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Tools/InitText", false, 300)]
    public static void InitText()
    {
        GameObject[] obj = Selection.gameObjects;
        if (obj != null && obj.Length > 0)
        {
            foreach (var o in obj)
            {
                Text[] txts = o.GetComponentsInChildren<Text>();
                foreach (var txt in txts)
                {
                    txt.text = string.Empty;
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    [MenuItem("Tools/替换选中UI字体")]
    public static void ReplaceFonts()
    {
        UnityEngine.Object[] objs = Selection.objects;
        List<string> list = new List<string>();
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0, length = objs.Length; i < length; i++)
            {
                if (objs[i] is GameObject)
                {
                    string path = AssetDatabase.GetAssetPath(objs[i]);
                    if (path.Contains("Resources/ui"))
                        ReplaceFonts(path);
                }
            }
        }
    }

    [MenuItem("Tools/替换所有UI字体")]
    public static void ReplaceAllUIFonts()
    {
        DirectoryInfo uiDirectoryInfo = new DirectoryInfo("Assets/Res/Resources/ui");
        foreach (FileInfo fileInfo in uiDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
        {
#if UNITY_EDITOR_WIN
            string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets\"));
#elif UNITY_EDITOR_OSX
            string prefabPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf(@"Assets/"));
#endif
            ReplaceFonts(prefabPath);
        }
    }

    private static void ReplaceFonts(string path)
    {
        GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
        List<Text> fontTextList = new List<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            fontTextList.Add(texts[i]);
        }

        if (fontTextList.Count > 0)
        {
            Debug.LogError("-------------------------------------------------------------------------------- [" + gameObject.name + "] --------------------------------------------------------------------------------");
            Transform parent = null;
            for (int i = 0; i < fontTextList.Count; i++)
            {
                string textPath = string.Empty;
                parent = fontTextList[i].transform.parent;
                while (parent != null)
                {
                    textPath = parent.name + "/" + textPath;
                    parent = parent.parent;
                }
                if (fontTextList[i].font != null)
                    textPath += fontTextList[i].name + " <====================> [" + fontTextList[i].font.name + "]";

                int fontSize = fontTextList[i].fontSize;
                Font sourceHanSans = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Res/Resources/font/SourceHanSansCN-Regular.otf");
                fontTextList[i].font = sourceHanSans;
                fontTextList[i].fontSize = fontSize;
                fontTextList[i].fontStyle = FontStyle.Normal;
                Debug.LogWarning(textPath);
            }
        }
        EditorUtility.SetDirty(gameObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/查找中文文件", false, 400)]
    static void FindChineseFile()
    {
        List<string> filePaths = new List<string>();
        string dir = Application.dataPath;
        DirectoryInfo di = new DirectoryInfo(dir);
        foreach (FileInfo f in di.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (StringUtil.ContainsChinese(f.FullName))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                filePaths.Add(assetsPath);
            }
        }
        foreach (var path in filePaths)
        {
            Debug.Log(path);
        }
        filePaths.Clear();
    }

    [MenuItem("Tools/查找命名包含空格", false, 400)]
    public static void FindWhiteSpaceCharacterInName()
    {
        FindWhiteSpaceCharacterCountInName();
    }

    public static int FindWhiteSpaceCharacterCountInName()
    {
        string path = Application.dataPath;
        DirectoryInfo dir = new DirectoryInfo(path);
        int i = 0;
        foreach (var f in dir.GetFiles("*.*", SearchOption.AllDirectories))
        {
            string name = f.Name;
            if (name.EndsWith(".meta"))
                continue;
            if (StringUtil.ContainsWhiteSpace(name))
            {
                string assetPath = f.FullName.Replace(path, "Assets/");
                Debug.Log(assetPath);
                i++;
            }
        }
        Debug.LogFormat("find space character in name total count:{0}", i);
        return i;
    }



    [MenuItem("Tools/Del Empty Dir", false, 400)]
    public static void DelEmptyDir()
    {
        string path = Application.dataPath;
        DelEmptyDir(new DirectoryInfo(path));
        AssetDatabase.Refresh();
    }

    private static void DelEmptyDir(DirectoryInfo dir)
    {
        foreach (var d in dir.GetDirectories())
        {
            DelEmptyDir(d);
        }
        if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
        {
            dir.Delete();
            Debug.LogFormat("del empty dir {0}", dir.FullName);
        }
    }    
}
