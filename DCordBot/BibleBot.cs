using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using HtmlAgilityPack;
using static DCordBot.CustomPreconditions;


namespace DCordBot
{

    public class BibleBot : CommandHandler
    {
        private const string summary = "How to use the Bible Module (ex): !Bible 1 John 4:8-10;KJV";

        [Command("Bible")]
        [Summary(summary)]
        [RequireCoolDown]
        public async Task ExecuteCommandAsync([Remainder] string input)
        {
            string webURL = "https://biblia.com/bible/";
            string[] args = input.Split(';');
            webURL += args[1] + "/";
            webURL += args[0];

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(webURL);
            HtmlNode targetNode = document.DocumentNode.SelectSingleNode("//button[@class='copy-button no-button']");
            if (targetNode != null)
            {
                Embedder embedder = new Embedder();
                embedder.SetTitle(args[0]);
                string desc = HttpUtility.HtmlDecode(targetNode.GetAttributeValue("data-copytext", ""));
                embedder.SetDescription(desc);
                await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
            }
        }
    }
}
