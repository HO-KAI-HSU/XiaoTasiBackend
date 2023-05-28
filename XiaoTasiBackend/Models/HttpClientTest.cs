using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace XiaoTasiBackend.Models
{
    public class HttpClientTest
    {
        public string httpClientGet(string url)
        {
            var client = new HttpClient();
            //client.BaseAddress = new Uri(url);

            var response = client.GetAsync(url).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            //TODO:反序列化
            //var serializer = new JavaScriptSerializer();
            //string list = serializer.Deserialize<string>(body);
            return body;
        }

        public string httpClientPost(string url, string phone)
        {
            var client = new HttpClient();
            //client.BaseAddress = new Uri(url);
            // 準備寫入的 data
            PostData postData = new PostData() { phone = phone };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果

            HttpResponseMessage response = client.PostAsync(url, contentPost).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            return body;
        }
    }

    internal class PostData
    {
        public PostData()
        {
        }

        public string phone { get; set; }
    }
}
