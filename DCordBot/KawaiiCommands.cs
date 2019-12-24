using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class Image
{
    [JsonProperty("_type")]
    public int _type { get; set; }
    [JsonProperty("_filePath")]
    public string _filePath { get; set; }
}

public class ImageList
{
    [JsonProperty("Images")]
    public List<Image> Images { get; set; }
}

namespace DCordBot
{
    public class KawaiiCommands : CommandHandler
    {
        private static ImageList kawaiiImages;

        public KawaiiCommands()
        {
            kawaiiImages = new ImageList();

            using (StreamReader r = new StreamReader("kawaiiImages.json"))
            {
                string json = r.ReadToEnd();
                kawaiiImages = JsonConvert.DeserializeObject<ImageList>(json);
            }
            return;
        }

        [Command("hug")]
        public async Task ResponseHug([Remainder]string players)
        {
            var targets = Context.Message.MentionedUsers;
            string response = $"{Context.Message.Author.Username} hugged ";
            foreach (SocketUser user in targets)
            {
                response += " " + user.Username;
            }
            response += "!";

            Random random = new Random();
            List<Image> images = await GetkawaiiImages(0);
            int index = random.Next(0, images.Count);
            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(images[index]._filePath);
            await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("kiss")]
        public async Task ResponseKiss([Remainder]string players)
        {
            var targets = Context.Message.MentionedUsers;
            string response = $"{Context.Message.Author.Username} kissed ";
            foreach (SocketUser user in targets)
            {
                response += " " + user.Username;
            }
            response += "!";

            Random random = new Random();
            List<Image> images = await GetkawaiiImages(1);
            int index = random.Next(0, images.Count);
            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(images[index]._filePath);
            await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("lick")]
        public async Task ResponseLick([Remainder]string players)
        {
            var targets = Context.Message.MentionedUsers;
            string response = $"{Context.Message.Author.Username} licked ";
            foreach (SocketUser user in targets)
            {
                response += " " + user.Username;
            }
            response += "!";

            Random random = new Random();
            List<Image> images = await GetkawaiiImages(2);
            int index = random.Next(0, images.Count);
            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(images[index]._filePath);
            await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("slap")]
        public async Task ResponseSlap([Remainder]string players)
        {
            var targets = Context.Message.MentionedUsers;
            string response = $"{Context.Message.Author.Username} slapped ";
            foreach (SocketUser user in targets)
            {
                response += " " + user.Username;
            }
            response += "!";

            Random random = new Random();
            List<Image> images = await GetkawaiiImages(3);
            int index = random.Next(0, images.Count);
            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(images[index]._filePath);
            await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("glare")]
        public async Task ResponseGlare([Remainder]string players)
        {
            var targets = Context.Message.MentionedUsers;
            string response = $"{Context.Message.Author.Username} glared at ";
            foreach (SocketUser user in targets)
            {
                response += " " + user.Username;
            }
            response += "!";

            Random random = new Random();
            List<Image> images = await GetkawaiiImages(4);
            int index = random.Next(0, images.Count);
            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(images[index]._filePath);
            await Context.Message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        public Task<List<Image>> GetkawaiiImages(int imageType)
        {
            return Task.Run(()=> kawaiiImages.Images.FindAll(x => x._type == imageType));
        }
    }
}
