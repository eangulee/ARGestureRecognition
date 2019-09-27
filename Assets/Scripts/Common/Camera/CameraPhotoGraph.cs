using Logic.ResMgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Cameras
{
    public class CameraPhotoGraph
    {
        public static void SaveRenderTextureToPNG(Camera camera)
        {
            RenderTexture rt = camera.targetTexture;
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            System.DateTime now = new System.DateTime();
            now = System.DateTime.Now;
            string filename = string.Format("easy_match_image{0}{1}{2}{3}.png", now.Day, now.Hour, now.Minute, now.Second);
            ResUtil.Save2Local(filename, bytes);
            ResUtil.Save2PhotoAlbum(filename);
            png = null;
            RenderTexture.active = prev;
        }

        public static string SaveRenderTextureToPNG(string fileName)
        {
            Texture2D png = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false, false);
            png.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            string filename = string.Format("{0}.png", fileName);
            ResUtil.DelRes(filename);
            ResUtil.Save2Local(filename, bytes);
            png = null;
            return filename;
        }

        public static byte[] SaveRenderTextureToPNGBytes()
        {
            Texture2D png = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false, false);
            png.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            return bytes;
        }
    }
}