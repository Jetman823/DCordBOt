using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    public class NSFWCommands : CommandHandler
    {
        [Command("yandere")]
        [Summary("!yandere <tagstoearch> add a space between each tag")]
        [RequireNsfw]
        public async Task ResponseYandere([Remainder] string tags = "")
        {
            ISocketMessageChannel channel = Context.Message.Channel;
            List<ImageData> images = new List<ImageData>();
            Random random = new Random();
            ImageData? randImage = null;
            if (tags.Length > 0)
            {
                foreach(ImageData image in NFSW.yandImages)
                {
                    bool result = Extensions.Anyof(image.Tags, tags);
                    if (result == true)
                        images.Add(image);
                }

                if (images.Count == 0)
                {
                    await channel.SendMessageAsync("sorry, couldn't find an image with those tags");
                }
                int RandomImage = random.Next(0, images.Count());
                randImage = NFSW.yandImages.ElementAt(RandomImage);
            }
            else
            {
                int RandomImage = random.Next(0, NFSW.yandImages.Count);
                randImage = NFSW.yandImages.ElementAt(RandomImage);
            }

            if (randImage == null)
                return;

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(randImage?.file_url);

            await channel.SendMessageAsync("", false, embedder.Build());

        }

        [Command("gelbooru")]
        [RequireNsfw]
        private async Task ResponseGelBooru([Remainder]string tags = "")
        {
            ISocketMessageChannel channel = Context.Message.Channel;
            List<ImageData> images = new List<ImageData>();
            Random random = new Random();
            ImageData? randImage = null;
            if (tags.Length > 0)
            {


                foreach (ImageData image in NFSW.gelBorImages)
                {
                    bool result = Extensions.Anyof(image.Tags, tags);
                    if (result == true)
                        images.Add(image);
                }

                if (images.Count == 0)
                {
                    await channel.SendMessageAsync("sorry, couldn't find an image with those tags");
                }
                int RandomImage = random.Next(0, images.Count());
                randImage = NFSW.gelBorImages.ElementAt(RandomImage);
            }
            else
            {
                int RandomImage = random.Next(0, NFSW.gelBorImages.Count);
                randImage = NFSW.gelBorImages.ElementAt(RandomImage);
            }

            if (randImage == null)
                return;

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(randImage?.file_url);

            await channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("danbooru")]
        [RequireNsfw]
        private async Task ResponseDanBooru([Remainder] string tags = "")
        {
            ISocketMessageChannel channel = Context.Message.Channel;
            List<ImageData> images = new List<ImageData>();
            Random random = new Random();
            ImageData? randImage = null;
            if (tags.Length > 0)
            {


                foreach (ImageData image in NFSW.danBorImages)
                {
                    bool result = Extensions.Anyof(image.Tags, tags);
                    if (result == true)
                        images.Add(image);
                }

                if (images.Count == 0)
                {
                    await channel.SendMessageAsync("sorry, couldn't find an image with those tags");
                }
                int RandomImage = random.Next(0, images.Count());
                randImage = NFSW.danBorImages.ElementAt(RandomImage);
            }
            else
            {
                int RandomImage = random.Next(0, NFSW.danBorImages.Count);
                randImage = NFSW.danBorImages.ElementAt(RandomImage);
            }

            if (randImage == null)
                return;

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(randImage?.file_url);

            await channel.SendMessageAsync("", false, embedder.Build());
        }
    }
}
