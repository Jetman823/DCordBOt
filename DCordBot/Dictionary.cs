using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Json;
using System.Threading.Tasks;
using static DCordBot.CustomPreconditions;

public struct WordsApi
{
    [JsonProperty("shortdef")]
    public string[] Shortdef { get; set; }
}

namespace DCordBot
{
    public class Dictionary : CommandHandler
    {
        [Command("define")]
        [Summary("test")]
        [RequireCoolDown(30)]
        public async Task DefineASync([Remainder] string word)
        {
            string apicall = "https://dictionaryapi.com/api/v3/references/collegiate/json/";
            apicall += word + "?key=4976810c-d633-4532-a50b-5811814e1c20";

            using (var webClient = new System.Net.WebClient())
            {
                Uri test = new Uri(apicall);
                string json = await webClient.DownloadStringTaskAsync(test);
                JsonArray obj = new JsonArray(json);

                List<WordsApi> array = JsonConvert.DeserializeObject<List<WordsApi>>(json);
                WordsApi wordDef = array[0];

                Embedder embedder = new Embedder();
                embedder.SetTitle(word);
                foreach(string value in wordDef.Shortdef)
                {
                    embedder.Description += "\n" + value;
                }
                embedder.AddThumbNailUrl("https://dictionaryapi.com/images/info/branding-guidelines/MWLogo_LightBG_120x120_2x.png");

                await Context.Channel.SendMessageAsync("", false, embedder.Build());
            }
        }
    }
}
