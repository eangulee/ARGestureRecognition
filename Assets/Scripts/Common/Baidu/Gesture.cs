using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace com.baidu.ai
{
    public class Gesture
    {
        public static Dictionary<string, string> gestureDescriptionDic = new Dictionary<string, string>();

        static Gesture()
        {
            InitGestureDescriptionDic();
        }

        // 手势识别
        public static string gesture(string token, String fileName)
        {
            //string token = "[调用鉴权接口获取的token]";
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/gesture?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(fileName);
            String str = "image=" + UnityWebRequest.EscapeURL(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = reader.ReadToEnd();
            Debug.Log("手势识别:");
            Debug.Log(result);
            return result;
        }

        public static String getFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }

        // 手势识别
        public static string gesture(string token, byte[] bytes)
        {
            //string token = "[调用鉴权接口获取的token]";
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/gesture?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getByteBase64(bytes);
            String str = "image=" + UnityWebRequest.EscapeURL(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = reader.ReadToEnd();
            Debug.Log("手势识别:");
            Debug.Log(result);
            return result;
        }

        public static String getByteBase64(byte[] bytes)
        {
            string baser64 = Convert.ToBase64String(bytes);
            return baser64;
        }

        private static void InitGestureDescriptionDic()
        {
            gestureDescriptionDic.Add(GestureType.One.ToString(), "数字1（原食指）");
            gestureDescriptionDic.Add(GestureType.Five.ToString(), "数字5（原掌心向前）");
            gestureDescriptionDic.Add(GestureType.Fist.ToString(), "拳头");
            gestureDescriptionDic.Add(GestureType.OK.ToString(), "OK");
            gestureDescriptionDic.Add(GestureType.Prayer.ToString(), "祈祷");
            gestureDescriptionDic.Add(GestureType.Congratulation.ToString(), "作揖");
            gestureDescriptionDic.Add(GestureType.Honour.ToString(), "作别");
            gestureDescriptionDic.Add(GestureType.Heart_single.ToString(), "单手比心");
            gestureDescriptionDic.Add(GestureType.Thumb_up.ToString(), "点赞");
            gestureDescriptionDic.Add(GestureType.Thumb_down.ToString(), "Diss");
            gestureDescriptionDic.Add(GestureType.ILY.ToString(), "我爱你");
            gestureDescriptionDic.Add(GestureType.Palm_up.ToString(), "掌心向上");
            gestureDescriptionDic.Add(GestureType.Heart_1.ToString(), "双手比心1");
            gestureDescriptionDic.Add(GestureType.Heart_2.ToString(), "双手比心2");
            gestureDescriptionDic.Add(GestureType.Heart_3.ToString(), "双手比心3");
            gestureDescriptionDic.Add(GestureType.Two.ToString(), "数字2");
            gestureDescriptionDic.Add(GestureType.Three.ToString(), "数字3");
            gestureDescriptionDic.Add(GestureType.Four.ToString(), "数字4");
            gestureDescriptionDic.Add(GestureType.Six.ToString(), "数字6");
            gestureDescriptionDic.Add(GestureType.Seven.ToString(), "数字7");
            gestureDescriptionDic.Add(GestureType.Eight.ToString(), "数字8");
            gestureDescriptionDic.Add(GestureType.Nine.ToString(), "数字9");
            gestureDescriptionDic.Add(GestureType.Rock.ToString(), "Rock");
            gestureDescriptionDic.Add(GestureType.Insult.ToString(), "竖中指");
            gestureDescriptionDic.Add(GestureType.Face.ToString(), "面部");
            gestureDescriptionDic.Add(GestureType.Other.ToString(), "其他");
        }

        public static string GetDescription(string gestureType)
        {
            string result = string.Empty;
            gestureDescriptionDic.TryGetValue(gestureType, out result);
            return result;
        }
    }

    public class GestureJson
    {
        public long log_id;
        public int result_num;
        public List<ResultJson> result;
    }

    [Serializable]
    public class ResultJson
    {
        public float probability;
        public int top;
        public int width;
        public int left;
        public int height;
        public string classname;

        public override string ToString()
        {
            return string.Format("[probability:{0},top:{1},width:{2},left:{3},height:{4},classname:{5}]", probability, top, width, left, left, height, classname);
        }
    }

    public enum GestureType
    {
        None = 0,
        One = 1,//数字1（原食指）		
        Five = 2,//数字5（原掌心向前）		
        Fist = 3,//拳头		
        OK = 4,//OK		
        Prayer = 5,//祈祷		
        Congratulation = 6,//作揖		
        Honour = 7,//作别		
        Heart_single = 8,//单手比心	Heart_single	
        Thumb_up = 9,//点赞	Thumb_up	
        Thumb_down = 10,//Diss	Thumb_down	
        ILY = 11,//我爱你	ILY
        Palm_up = 12,//掌心向上		
        Heart_1 = 13,//	双手比心1	
        Heart_2 = 14,//	双手比心2	
        Heart_3 = 15,//	双手比心3	
        Two = 16,//数字2		
        Three = 17,//数字3		
        Four = 18,//数字4		
        Six = 19,//数字6		
        Seven = 20,//数字7		
        Eight = 21,//数字8	Eight	
        Nine = 22,//数字9		
        Rock = 23,//Rock		
        Insult = 24,//竖中指
        Face = 25,//面部
        Other = 26,//其他
    }
}
