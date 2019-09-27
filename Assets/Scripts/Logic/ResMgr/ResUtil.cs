/*
 * @Author:lily
 * @Create Time:2017-08-15 10:44:13
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Logic.ResMgr
{
    public static class ResUtil
    {
        private const string LOCAL_WWW_PREFIX = "file:///";

        public static List<string> RecursiveGetFiles(string path)
        {
            List<string> result = new List<string>();
            if (!System.IO.Directory.Exists(path))
            {
                if (System.IO.File.Exists(path))
                    result.Add(path);
                return result;
            }
            result.AddRange(System.IO.Directory.GetFiles(path));
            foreach (string dir in System.IO.Directory.GetDirectories(path))
            {
                result.AddRange(RecursiveGetFiles(dir));
            }
            return result;
        }

        public static string GetFileNameByPath(string fullPath)
        {
            int index = fullPath.LastIndexOf("/");
            return fullPath.Substring(index + 1);
        }

        public static string GetDirectoryByPath(string path)
        {
            return path.Substring(0, path.LastIndexOf("/"));
        }

        private static string GetFullPath(string basePath, string subPath)
        {
            return string.Format(@"{0}/{1}/{2}", basePath, ResConf.eResPlatform, subPath);
        }

        //本地ab路径
        public static string GetSandBoxPath(string subPath)
        {
            return GetFullPath(Application.persistentDataPath, subPath);
        }

        public static string GetStreamingAssetsPath(string subPath)
        {
            string path = string.Format(@"{0}/{1}", Application.streamingAssetsPath, subPath);
#if UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_ANDROID

#elif UNITY_IOS
//			path = "file://" + path;
#endif
            return path;
        }

        public static string GetLocalWWWPath(string subPath)
        {
            return string.Format(@"{0}{1}", LOCAL_WWW_PREFIX, subPath);
        }

        public static void CopyFiles(string sourceDir, string destDir, string searchPattern = "*.txt", SearchOption option = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
            int len = sourceDir.Length;

            if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
            {
                --len;
            }

            for (int i = 0; i < files.Length; i++)
            {
                string str = files[i].Remove(0, len);
                string dest = destDir + "/" + str;
                string dir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(files[i], dest, true);
            }
        }

        public static byte[] GetBytesFromLocal(string subPath)
        {
            string path = GetSandBoxPath(subPath);
            if (!File.Exists(path))
                return null;

            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }

        public static void Save2Local(string subPath, byte[] bytes)
        {
            try
            {
                string path = GetSandBoxPath(subPath);
                string dir = ResUtil.GetDirectoryByPath(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                fs.SetLength(0);
                if (bytes != null)
                    fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
                Debug.LogFormat("Save2Local:{0}", path);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Save2Local:{0} error {1}!", subPath, e.ToString());
            }
        }

        public static bool DelRes(string subPath)
        {
            try
            {
                string path = GetSandBoxPath(subPath);
                if (File.Exists(path))
                    File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("DelAllLocalAbRes:" + e);
            }
            return false;
        }

        /// <summary>
        /// 保存照片到系统相册
        /// </summary>
        /// <param name="path"></param>
        public static void Save2PhotoAlbum(string subPath)
        {
#if UNITY_IOS && !UNITY_EDITOR 
            string path = GetSandBoxPath(subPath);
            IOSAlbumManager.instance.SaveImageToPhotosAlbum(path);
#endif
        }
    }
}
