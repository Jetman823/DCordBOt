using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace DCordBot
{
    public class Music : CommandHandler
    {

        [Command("playsong", RunMode = RunMode.Async)]
        [Summary("playsong songname")]
        public async Task Play([Remainder] string query)
        {
            return;
        }
    }
}