using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

public struct Images
{
    public string filePath;
    public char    imageType;
};

namespace DCordBot
{
    class KawaiiCommands : CommandModule
    {
        private static List<Images> kawaiiImages;

        public KawaiiCommands()
        {
            kawaiiImages = new List<Images>();
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true
            };
            XmlReader reader = XmlReader.Create("KawaiiImages.xml", settings);
            doc.Load(reader);
            foreach (XmlNode childNode in doc.FirstChild)
            {
                if (childNode.Name == "IMAGE")
                {
                    Images imageInfo = new Images
                    {
                        imageType = Convert.ToChar(childNode.Attributes["type"].Value),
                        filePath = childNode.Attributes["filePath"].Value
                    };
                    kawaiiImages.Add(imageInfo);
                }
            }
        }
        override public async Task Response(SocketMessage message, CommandInfo command)
        {
            switch (command.commandName.ToLower())
            {
                case "!hug":
                    {
                        await ResponseHug(message);
                    }
                    break;
                case "!kiss":
                    {
                        await ResponseKiss(message);
                    }
                    break;
                case "!lick":
                    {
                        await ResponseLick(message);
                    }
                    break;
                case "!slap":
                    {
                        await ResponseSlap(message);
                    }
                    break;
                case "!glare":
                    {
                        await ResponseGlare(message);
                    }
                    break;
                default:
                    return;
            }
        }

        private async Task ResponseHug(SocketMessage message)
        {
            string response = message.Author.Username + " Hugs ";
            if (message.MentionedUsers.Count < 1)
                return;
            foreach(SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }
            var embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = response
            };
            Random random = new Random();
            List<Images> image = kawaiiImages.FindAll(x => x.imageType == '0');
            int index = random.Next(0, image.Count);
            embed.WithImageUrl(image[index].filePath);
            await message.Channel.SendMessageAsync("",false,embed.Build());
        }

        private async Task ResponseKiss(SocketMessage message)
        {
            string response = message.Author.Username + " Kisses ";
            if (message.MentionedUsers.Count < 1)
                return;
            foreach (SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }
            var embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = response
            };
            Random random = new Random();
            List<Images> image = kawaiiImages.FindAll(x => x.imageType == '1');
            int index = random.Next(0, image.Count);
            embed.WithImageUrl(image[index].filePath);
            await message.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ResponseLick(SocketMessage message)
        {
            string response = message.Author.Username + " Licks ";
            if (message.MentionedUsers.Count < 1)
                return;
            foreach (SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }
            var embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = response
            };

            Random random = new Random();
            List<Images> image = kawaiiImages.FindAll(x => x.imageType == '2');
            int index = random.Next(0, image.Count);
            embed.WithImageUrl(image[index].filePath);
            await message.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ResponseSlap(SocketMessage message)
        {
            string response = message.Author.Username + " Slapped ";
            if (message.MentionedUsers.Count < 1)
                return;

            foreach(SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }

            var embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = response
            };

            Random random = new Random();
            List<Images> image = kawaiiImages.FindAll(x => x.imageType == '3');
            int index = random.Next(0, image.Count);
            embed.WithImageUrl(image[index].filePath);
            await message.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ResponseGlare(SocketMessage message)
        {
            string response = message.Author.Username + " Glared at ";
            if (message.MentionedUsers.Count < 1)
                return;

            foreach(SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }

            var embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = response
            };
            Random random = new Random();
            List<Images> image = kawaiiImages.FindAll(x => x.imageType == '4');
            int index = random.Next(0, image.Count);
            embed.WithImageUrl(image[index].filePath);

            await message.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
