using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;
using System.Json;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

public struct ImageData
{
    [JsonProperty("source")]
#pragma warning disable IDE1006 // Naming Styles
    public string source { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    [JsonProperty("file_url")]
    public string file_url { get; set; }
}

namespace DCordBot
{
    class NFSW
    {
        public static List<ImageData> yandImages = new List<ImageData>();
        public static List<ImageData> gelBorImages = new List<ImageData>();
        public static List<ImageData> danBorImages = new List<ImageData>();

        public bool Load()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://yande.re/post.json");

                JsonArray obj = new JsonArray(json);

                dynamic array = JsonConvert.DeserializeObject<List<ImageData>>(json);
                foreach (var item in array)
                {
                    ImageData data = new ImageData();
                    data = item;
                    yandImages.Add(data);
                }
            }

            using (var webclient = new System.Net.WebClient())
            {
                var json = webclient.DownloadString("https://gelbooru.com/index.php?page=dapi&s=post&q=index&json=1&tags=");

                    JsonArray obj = new JsonArray(json);

                dynamic array = JsonConvert.DeserializeObject<List<ImageData>>(json);
                foreach (var item in array)
                {
                    ImageData data = new ImageData();
                    data = item;
                    gelBorImages.Add(data);
                }
            }

            using (var webclient = new System.Net.WebClient())
            {
                var json = webclient.DownloadString("https://danbooru.donmai.us/posts.json?tags=");
                JsonArray obj = new JsonArray(json);
                dynamic array = JsonConvert.DeserializeObject<List<ImageData>>(json);

                foreach(var item in array)
                {
                    ImageData data = new ImageData();
                    data = item;
                    danBorImages.Add(data);
                }
            }
                return true;
        }
    }
}
