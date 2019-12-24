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
        public async Task ResponseYandere([Remainder] string tags)
        {
            ISocketMessageChannel channel = Context.Message.Channel;
            List<ImageData> images = null;
            Random random = new Random();
            ImageData? randImage = null;
            if (tags.Length > 0)
            {


                images = NFSW.yandImages.FindAll(x => x.Tags.All(tags.Contains));
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
        private async Task ResponseGelBooru()
        {
            ISocketMessageChannel channel = Context.Message.Channel;

            Random random = new Random();
            int RandomImage = random.Next(0, NFSW.gelBorImages.Count);
            ImageData randImage = NFSW.gelBorImages.ElementAt(RandomImage);

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(randImage.file_url);

            await channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("danbooru")]
        [RequireNsfw]
        private async Task ResponseDanBooru()
        {
            ISocketMessageChannel channel = Context.Message.Channel;

            Random random = new Random();
            int RandomImage = random.Next(0, NFSW.danBorImages.Count);
            ImageData randImage = NFSW.danBorImages.ElementAt(RandomImage);

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(randImage.file_url);

            await channel.SendMessageAsync("", false, embedder.Build());
        }
    }
}
