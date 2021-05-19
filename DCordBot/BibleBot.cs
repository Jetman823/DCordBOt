using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using HtmlAgilityPack;
using System.Timers;
using static DCordBot.CustomPreconditions;
using Discord.WebSocket;
using System.Collections.Generic;

namespace DCordBot
{

    public class BibleBot : CommandHandler
    {
        private const string summary = "How to use the Bible Module (ex): !Bible 1 John 4:8-10;KJV";
        public Timer timeElapsed = null;

        public BibleBot()
        {
            //timeElapsed = new Timer();
            //timeElapsed.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //timeElapsed.Interval = 10000;
            //timeElapsed.Start();
        }

        //static void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    IReadOnlyCollection<SocketTextChannel> channels = DCordBot.Program.client.Guilds.ElementAt(0).Channels as IReadOnlyCollection<SocketTextChannel>;
        //    foreach(SocketTextChannel channel in channels)
        //    {
        //        channel.SendMessageAsync("Jesus Loves You!");
        //    }
        //}


        [Command("Bible")]
        [Summary(summary)]
        [RequireCoolDown(0)]
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


        [Command("Jesus")]
        public async Task JesusAsync()
        {
            string[] embedPaths = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            if (embedPaths.Contains("DCordBot.JesusImages.Context.gif"))
            {
                embedPaths = embedPaths.Where(x => x != "DCordBot.JesusImages.Context.gif").ToArray();
            }
            Random rand = new Random();
            int randImage = rand.Next(1, embedPaths.Count());
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embedPaths[randImage]);
            await Context.Message.Channel.SendFileAsync(stream, embedPaths[randImage]);
        }

        [Command("Context")]
        public async Task ContextAsync()
        {
            Stream contextStream = GetContext();

            await Context.Message.Channel.SendFileAsync(contextStream, "Context.gif");
        }

        public static Stream GetContext()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("DCordBot.JesusImages.Context.gif");
        }
    }
}
