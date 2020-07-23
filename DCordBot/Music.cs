using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
namespace DCordBot
{
    public class Music : CommandHandler
    {
        [Command("playsong")]
        [Summary("playsong songname")]
        public async Task PlaySongAsync([Remainder] string songName)
        {
            return;
        }
    }
}
