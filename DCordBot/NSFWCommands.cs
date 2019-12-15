using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    class NSFWCommands : CommandModule
    {
        public override async Task Response(SocketMessage message,CommandInfo command)
        {
            switch(command.commandName.ToLower())
            {
                case "!yandere":
                    {
                        await ResponseYandere(message);
                    }
                    break;
                case "!gelbooru":
                    {
                        await ResponseGelBooru(message);
                    }
                    break;
                case "!danbooru":
                    {
                        await ResponseDanBooru(message);
                    }break;
                default:
                    return;
            }
        }

        private async Task ResponseYandere(SocketMessage message)
        {
            ITextChannel textChannel = (ITextChannel)message.Channel;
            if (!((ITextChannel)message.Channel).IsNsfw)
            {
                await message.Channel.SendMessageAsync("This only works if the channel is NSFW!");
                return;
            }

            Random random = new Random();
            int RandomImage = random.Next(0, NFSW.yandImages.Count);
            ImageData randImage = NFSW.yandImages.ElementAt(RandomImage);

            var embed = new EmbedBuilder
            {
                Color = Color.Red
            };
            embed.WithImageUrl(randImage.file_url);

            await message.Channel.SendMessageAsync("", false, embed.Build());

        }

        private async Task ResponseGelBooru(SocketMessage message)
        {
            ITextChannel textChannel = (ITextChannel)message.Channel;
            if (!((ITextChannel)message.Channel).IsNsfw)
            {
                await message.Channel.SendMessageAsync("This only works if the channel is NSFW!");
                return;
            }

            var embed = new EmbedBuilder
            {
                Color = Color.Red
            };
            Random random = new Random();
            int RandomImage = random.Next(0, NFSW.gelBorImages.Count);
            ImageData randImage = NFSW.gelBorImages.ElementAt(RandomImage);
            embed.WithImageUrl(randImage.file_url);

            await message.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ResponseDanBooru(SocketMessage message)
        {
            ITextChannel textChannel = (ITextChannel)message.Channel;
            if (!((ITextChannel)message.Channel).IsNsfw)
            {
                await message.Channel.SendMessageAsync("This only works if the channel is NSFW!");
                return;
            }

            var embed = new EmbedBuilder
            {
                Color = Color.Red
            };
            Random random = new Random();
            int RandomImage = random.Next(0, NFSW.danBorImages.Count);
            ImageData randImage = NFSW.danBorImages.ElementAt(RandomImage);
            embed.WithImageUrl(randImage.file_url);

            await message.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
