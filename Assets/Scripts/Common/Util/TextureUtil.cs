using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Common.Util
{
    public static class TextureUtil
    {
        public static Texture2D AnticlockwiseRotate90(Texture2D target)
        {
            int width = target.width;  //图片原本的宽度
            int height = target.height;  //图片原本的高度
            Texture2D newTexture = new Texture2D(height, width);
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    Color color = target.GetPixel(i, j);
                    newTexture.SetPixel(height - j - 1, i, color);
                }
            }
            newTexture.Apply();
            return newTexture;
        }
    }
}