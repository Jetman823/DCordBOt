using System.Threading.Tasks;
using Discord.Commands;

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
