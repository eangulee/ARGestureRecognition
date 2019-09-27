using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ByteExpand
{
    public static string ToCustomString(this byte[] bytes)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int temp = 0;        
        for (int i = 0, length = bytes.Length; i < length; i++)
        {
            temp += bytes[i];
            sb.Append(string.Format("{0:X2}", bytes[i]));
            if (i != length - 1)
                sb.Append(",");
        }
        //Debug.Log("value : {0}", temp);
        return sb.ToString();
    }

    public static string ToCustomString(this List<byte> bytes)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int temp = 0;
        for (int i = 0, length = bytes.Count; i < length; i++)
        {
            temp += bytes[i];
            sb.Append(string.Format("{0:X2}", bytes[i]));
            if (i != length - 1)
                sb.Append(",");
        }
        //Debug.Log("value : {0}", temp);
        return sb.ToString();
    }
}
