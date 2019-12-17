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
            string commandName = commandInfo.commandName.ToLower();
            switch(commandName)
            {
                case "!kick":
                    {
                        await ResponseKick(message, (SocketGuildUser)message.Author);
                    }break;
                case "!ban":
                    {
                        await ResponseBan(message, (SocketGuildUser)message.Author);
                    }
                    break;
                case "!unban":
                    {
                        await ResponseUnBan(message, (SocketGuildUser)message.Author);
                    }
                    break;
                default:
                    return;
            }
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
        public static async Task ResponseBan(SocketMessage message, SocketGuildUser admin)
        {
            var role = (admin as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Hospitalized Inmates" || x.Name == "Professional Spanker");
            if (!admin.Roles.Contains(role))
            {
                return;
            }

            var usersToBan = message.MentionedUsers;
            foreach (var user in usersToBan)
            {
                if (user.IsBot && message.Author.Username != "DeffJay")
                {
                    await message.Channel.SendMessageAsync("Why would you ban a bot?!");
                    continue;
                }

                if(user.Username == "DeffJay")
                {
                    await message.Channel.SendMessageAsync("I will not let you ban my master! Counter Ban!");
                    await ((SocketGuildUser)message.Author).BanAsync();
                    return;
                }

                var chnl = message.Channel as SocketGuildChannel;

                await message.Channel.SendMessageAsync("User is Banned!");
                await chnl.Guild.AddBanAsync(user, 1);
            }
        }

        private static async Task ResponseUnBan(SocketMessage message, SocketGuildUser admin)
        {
            var role = (admin as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Hospitalized Inmates");
            if (!admin.Roles.Contains(role))
                return;

            if (message.Author.Username != "DeffJay")
                return;

            var usersToUnBan = message.MentionedUsers;
            foreach(var user in usersToUnBan)
            {
                await ((IGuild)message.Channel).RemoveBanAsync(user);
            }
        }
    }
}
