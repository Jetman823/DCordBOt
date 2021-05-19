using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    public class AdminCommands : CommandHandler
    {
        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task ResponseKick([Remainder] string name)
        {
            SocketGuildUser sender = Context.Message.Author as SocketGuildUser;

            var usersToKick = Context.Message.MentionedUsers;
            foreach (var user in usersToKick)
            {
                SocketGuildUser player = user as SocketGuildUser;
                if (player.GuildPermissions.Administrator && !sender.GuildPermissions.Administrator)
                    continue;

                await (((SocketGuildUser)(user)).KickAsync());
            }
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task ResponseBan([Remainder] string name)
        {
            SocketGuildUser sender = Context.Message.Author as SocketGuildUser;

            var usersToBan = Context.Message.MentionedUsers;
            foreach (var user in usersToBan)
            {
                SocketGuildUser player = user as SocketGuildUser;
                if (player.GuildPermissions.Administrator && !sender.GuildPermissions.Administrator)
                    continue;

                var chnl = Context.Message.Channel as SocketGuildChannel;

                await Context.Message.Channel.SendMessageAsync($"{user.Username} is Banned!");
                await chnl.Guild.AddBanAsync(user, 1);
            }
        }

        [Command("unban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task ResponseUnBan([Remainder] string name)
        {
            SocketGuildUser sender = Context.Message.Author as SocketGuildUser;

            var usersToUnBan = Context.Message.MentionedUsers;
            foreach(var user in usersToUnBan)
            {
                await ((IGuild)Context.Message.Channel).RemoveBanAsync(user);
            }
        }
        [Command("ClearChat")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ResponseClearChat([Remainder] string messageCount)
        {
            int count = Convert.ToInt32(messageCount);
            var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task MuteUser([Remainder] string name)
        {
            foreach(SocketUser user in Context.Message.MentionedUsers)
            {
                await (user as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));
            }
        }

        [Command("unmute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task UnMuteUser([Remainder] string name)
        {
            foreach (SocketUser user in Context.Message.MentionedUsers)
            {
                await (user as SocketGuildUser).RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));
            }
        }
    }
}
