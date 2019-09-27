using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace com.baidu.ai
{
    public static class AccessToken
    {
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 返回token示例
        public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "Pjm8ch6B3wrx454CatDrG4Ul";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "h5Z43YmnyZGwU96D07CHdmx5hfHGR6e4";

        public static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Debug.Log(result);
            return result;
        }
    }

    public class TokenJson
    {
        public string refresh_token;
        public int expires_in;
        public string session_key;
        public string access_token;
        public string scope;
        public string session_secret;
    }
}
