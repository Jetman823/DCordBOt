using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    class CustomPreconditions
    {
        public class RequireCoolDown : PreconditionAttribute
        {
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,CommandInfo command, IServiceProvider services)
            {
                var messages = await context.Channel.GetMessagesAsync().FlattenAsync();
                foreach (IUserMessage message in messages)
                {
                    if (message.Timestamp == context.Message.Timestamp)
                        continue;
                    if (message.Content.Contains(command.Name) && message.Timestamp.TimeOfDay.TotalSeconds + 30 > context.Message.Timestamp.TimeOfDay.TotalSeconds)
                    {
                        return PreconditionResult.FromError("This command can only be executed every 30 seconds.");
                    }
                }
                return PreconditionResult.FromSuccess();
            }
        }
        public class CheckMarriageStatus : PreconditionAttribute
        {
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                SocketMessage message = (SocketMessage)context.Message;
                SqlBuilder builder = new SqlBuilder("spProposeToUser", CommandType.StoredProcedure);
                builder.AddParameter("@Sender", SqlDbType.BigInt, Convert.ToInt64(context.User.Id));
                builder.AddParameter("@Target", SqlDbType.BigInt, Convert.ToInt64(message.MentionedUsers.ElementAt(0).Id));
                builder.AddParameter("@ServerID", SqlDbType.BigInt, Convert.ToInt64(((SocketGuildChannel)context.Message.Channel).Id));
                builder.AddParameter("@RESULT", SqlDbType.TinyInt, 0, ParameterDirection.ReturnValue);

                await builder.ExecuteNonQueryAsync();
                byte result = Convert.ToByte(builder.GetParameter("@RESULT"));

                if (result == 1)
                {
                    return PreconditionResult.FromError("You're already married! You must divorce your current spouse first.");
                }
                else if (result == 2)
                {
                    return PreconditionResult.FromError(message.MentionedUsers.ElementAt(0).Username + " is already married!");
                }

                return PreconditionResult.FromSuccess();
            }
        }
        public class  CheckBJUser: PreconditionAttribute
        {
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                SocketMessage message = (SocketMessage)context.Message;
                SocketUser sender = message.Author;

                if (!BlackJack.bjPlayers.ContainsKey(context.Guild.Id))
                {
                    List<BJPlayers> guildPlayers = new List<BJPlayers>();
                    guildPlayers.Add(new BJPlayers(context.Message.Author.Id, 0, 0));
                    BlackJack.bjPlayers.Add(context.Guild.Id, guildPlayers);
                }


                bool playerExists = BlackJack.bjPlayers[context.Guild.Id].Exists(x => x.userID == message.Author.Id);

                if(command.Name.Contains("new"))
                {
                    if(playerExists == true)
                    {
                        return PreconditionResult.FromError("You've already started a game!");
                    }
                }


                if (command.Name.Contains("hit") || command.Name.Contains("stay"))
                {
                    if (playerExists == false)
                    {
                        return PreconditionResult.FromError("You need to start a game before you can hit!");
                    }
                }

                return PreconditionResult.FromSuccess();
            }
        }
    }
}
