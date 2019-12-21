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
    class KawaiiCommands : CommandModule
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
        override public async Task Response(SocketMessage message, CommandInfo command)
        {
            if(command.commandType == 3)
            {
                await ResponseKawaii(message,command);
            }
            return;
        }

        private async Task ResponseKawaii(SocketMessage message,CommandInfo command)
        {               
            if (message.MentionedUsers.Count < 1)
                return;

            string response = command.desc;
            response = response.Replace("$1", message.Author.Username);

            foreach (SocketUser user in message.MentionedUsers)
            {
                response += " " + user.Username;
            }
            response += "!";
            Random random = new Random();
            List<Image> image = kawaiiImages.Images.FindAll(x => x._type == command.kawaiiType);
            int index = random.Next(0, image.Count);

            Embedder embedder = new Embedder();
            embedder.SetTitle(response);
            embedder.AddImageUrl(image[index]._filePath);
            await message.Channel.SendMessageAsync("", false, embedder.Build());
        }
    }
}
