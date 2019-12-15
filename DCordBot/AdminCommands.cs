using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    class AdminCommands : CommandModule
    {
        public override async Task Response(SocketMessage message,CommandInfo commandInfo)
        {
            if(message.Content.Contains("!kick"))
                await ResponseKick(message,(SocketGuildUser)message.Author);
            if(message.Content.Contains("!ban"))
                await ResponseBan(message,(SocketGuildUser)message.Author);
        }

        private static async Task ResponseKick(SocketMessage message, SocketGuildUser admin)
        {
            var role = (admin as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Staff");
            if (!admin.Roles.Contains(role))
            {
                return;
            }

            var usersToKick = message.MentionedUsers;
            foreach (var user in usersToKick)
            {


                if (user.IsBot)
                    continue;

                foreach (var targetRole in ((SocketGuildUser)(user)).Roles)
                {
                    if (targetRole.Name.Contains("Staff") || targetRole.Name.Contains("Admin"))
                        continue;
                }

                await (((SocketGuildUser)(user)).KickAsync());
            }
        }

        /// <summary>
        /// Admins can ban staff, but not other admins or bot
        /// </summary>
        /// <param name="message"></param>
        /// <param name="admin"></param>
        /// <returns></returns>
        private static async Task ResponseBan(SocketMessage message, SocketGuildUser admin)
        {
            var role = (admin as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (!admin.Roles.Contains(role))
            {
                return;
            }

            var usersToBan = message.MentionedUsers;
            foreach (var user in usersToBan)
            {
                if (user.IsBot)
                {
                    await message.Channel.SendMessageAsync("Why would you ban a bot?!");
                    continue;
                }

                foreach (var targetRole in ((SocketGuildUser)(user)).Roles)
                {
                    if (targetRole.Name.Contains("Admin"))
                        continue;
                }

                await (((SocketGuildUser)(user)).BanAsync());
            }
        }
    }
}
