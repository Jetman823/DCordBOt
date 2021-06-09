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
            private readonly int cooldownTime;

            public RequireCoolDown(int cooldown) => cooldownTime = cooldown;
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                var messages = await context.Channel.GetMessagesAsync(cooldownTime).FlattenAsync();
                foreach (IUserMessage message in messages)
                {
                    if (message.Author.IsBot || message.Timestamp == context.Message.Timestamp)
                        continue;

                    if (message.Timestamp.Date.ToLocalTime() != DateTime.Today)
                        continue;

                    long timeElapsed = context.Message.Timestamp.ToUnixTimeSeconds() - message.Timestamp.ToUnixTimeSeconds();
                    if (timeElapsed < cooldownTime)
                    {
                        return PreconditionResult.FromError("This command can only be executed every 30 seconds.");
                    }
                    break;
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
        public class AddBJUser : PreconditionAttribute
        {
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                SocketMessage message = (SocketMessage)context.Message;
                SocketUser sender = message.Author;
                ///TODO:
                ///make this async!
                /// 
                bool result = new Task<bool>(() => BlackJack.bjPlayers.ContainsKey(context.Guild.Id)).Result;
                if(result == false)
                {
                    List<BJPlayers> guildPlayers = new List<BJPlayers>();
                    guildPlayers.Add(new BJPlayers(context.Message.Author.Id, 0, 0));
                    BlackJack.bjPlayers.Add(context.Guild.Id, guildPlayers);
                    return PreconditionResult.FromSuccess();
                }
                else
                {
                    BJPlayers player = new Task<BJPlayers>(() => BlackJack.bjPlayers[context.Guild.Id].Find(x => x.userID == context.Message.Author.Id)).Result;
                    if(player == null)
                    {
                        BlackJack.bjPlayers[context.Guild.Id].Add(player);
                        return PreconditionResult.FromSuccess();
                    }
                    else
                    {
                        return PreconditionResult.FromError("You've already started a game!");
                    }
                }
            }
        }

        public class HitBJUser : PreconditionAttribute
        {
            //TODO:
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                if (BlackJack.bjPlayers[context.Guild.Id].Select(x => x.userID == context.Message.Author.Id) == null)
                {
                    return PreconditionResult.FromError("You haven't started a game yet!");
                }

                return PreconditionResult.FromSuccess();
            }
        }

        public class StayBJUser : PreconditionAttribute
        {
            //TODO:
            public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {

                if (BlackJack.bjPlayers[context.Guild.Id].Select(x => x.userID == context.Message.Author.Id) == null)
                {
                    return PreconditionResult.FromError("You haven't started a game yet!");
                }

                return PreconditionResult.FromSuccess();
            }
        }
    }
}
