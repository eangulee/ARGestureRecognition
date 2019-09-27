/*
 * @Author:lily
 * @Create Time:2017-08-15 10:44:13
 */
using Logic.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.ResMgr
{
    public sealed class ResPath
    {
        public static string GetFullPath(string basePath, string subPath)
        {
            return string.Format("{0}/{1}", basePath, subPath);
        }

        public static string GetStreamingAssetsPath(string subPath)
        {
            string path = GetFullPath(Application.streamingAssetsPath, subPath);
#if UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_ANDROID

#elif UNITY_IOS
            path = "file://" + path;
#endif
            return path;
        }
    }
}
