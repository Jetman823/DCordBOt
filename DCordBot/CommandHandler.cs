using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    public class CommandHandler : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync([Remainder]string command)
        {
            string message = "";
            if (command == null)
            {
                foreach (var commandinfo in Program.commands.Commands)
                {
                    message += commandinfo.Name + ":\n" + commandinfo.Summary + "\n";
                }
            }
            else
            {
                CommandInfo comm = Program.commands.Commands.FirstOrDefault(x => x.Name.ToLower() == command.ToLower());
                message += comm.Name + ":\n" + comm.Summary + "\n";
            }
            await Context.Message.Channel.SendMessageAsync(message);
        }
    }
}
