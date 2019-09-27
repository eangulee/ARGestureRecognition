using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Logic.ResMgr;
using Common.Util;
using System.Text;
namespace Common.Tools.Editor
{
    [ExecuteInEditMode]
    public class PlayerBuilder : UnityEditor.EditorWindow
    {
        [MenuItem("Tools/发布打包", false, 100)]
        public static void OpenAssetBundlesBuilder()
        {
            EditorWindow.GetWindow<PlayerBuilder>();
            appVersion = EditorPrefs.GetString("appVersion", "1.0.0.0");
            abVersion = EditorPrefs.GetString("abVersion", "1");
            res = EditorPrefs.GetBool("res", true);
            config = EditorPrefs.GetBool("config", true);
            bin = EditorPrefs.GetBool("bin", false);
#if UNITY_ANDROID
            android = true;
#elif UNITY_IOS
            ios = true;
#endif
        }
        private static bool empty = false, android = false, ios = false, pc = false, iosPlayer = false, androidPlayer = true, androidProject = true, debug = false;
        private static bool res = true, config = true, bin = false;
        private static string appVersion, abVersion, abLastVersion;
        void OnGUI()
        {
            #region 打包资源
            /*
            appVersion = EditorGUILayout.TextField("app版本号：", appVersion, GUILayout.Width(200));
            abVersion = EditorGUILayout.TextField("资源版本号：", abVersion, GUILayout.Width(200));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            empty = EditorGUILayout.Toggle("无更新:", empty);
            if (!empty)
            {
                res = EditorGUILayout.Toggle("ab文件:", res);
                config = EditorGUILayout.Toggle("Lua文件和配置文件:", config);
                bin = EditorGUILayout.Toggle("bin文件:", bin);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            android = EditorGUILayout.Toggle("安卓:", android);
            ios = EditorGUILayout.Toggle("IOS:", ios);
            pc = EditorGUILayout.Toggle("pc:", pc);
            EditorGUILayout.Space();
            debug = EditorGUILayout.Toggle("debug:", debug);
            if (GUILayout.Button("打包资源"))
            {
                if (abLastVersion == abVersion)
                {
                    Debugger.Log("版本号相同!");
                }
                if (empty)
                {
                    if (android)
                        ProjectBuilder.BuildEmptyForAndroid(appVersion, abVersion, debug);
                    if (ios)
                        ProjectBuilder.BuildEmptyForIOS(appVersion, abVersion, debug);
                    if (pc)
                        ProjectBuilder.BuildEmptyForPC(appVersion, abVersion, debug);
                }
                else
                {
                    if (android)
                        ProjectBuilder.BuildAssetBundleForAndroid(appVersion, abVersion, res, config, bin, debug);
                    if (ios)
                        ProjectBuilder.BuildAssetBundleForIOS(appVersion, abVersion, res, config, bin, debug);
                    if (pc)
                        ProjectBuilder.BuildAssetBundleForPC(appVersion, abVersion, res, config, bin, debug);
                    EditorPrefs.SetString("appVersion", appVersion);
                    EditorPrefs.SetString("abVersion", abVersion);
                    EditorPrefs.SetBool("res", res);
                    EditorPrefs.SetBool("config", config);
                    EditorPrefs.SetBool("bin", bin);
                }
                this.Close();
            }*/
            #endregion

            #region 发布
            androidPlayer = EditorGUILayout.Toggle("安卓:", androidPlayer);
            iosPlayer = EditorGUILayout.Toggle("IOS:", iosPlayer);
            if (GUILayout.Button("导出工程"))
            {
                if (androidPlayer)
                {
                    ProjectBuilder.ExportAndroidProject();
                }
                if (iosPlayer)
                    ProjectBuilder.ExportXCodeProject();
                this.Close();
            }
            #endregion
        }
    }
}