using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static DCordBot.CustomPreconditions;

/// <summary>
/// I know I can simplify it, don't want to bitch. I don't want to add brackets for 2 or 3 variables....
/// </summary>
#pragma warning disable IDE0017
#pragma warning disable IDE0011

public struct marriageQuery
{
    public ulong user1;
    public ulong user2;
}

public struct CurrentProposals
{
    public long user1;
    public long user2;
}

namespace DCordBot
{
    public class Marriage : CommandHandler
    {
        private List<CurrentProposals?> proposals = new List<CurrentProposals?>();

        [Command("marriagestatus")]
        [Summary("!marriagestatus <mention>")]
        public async Task ResponseMarriageStatus([Remainder] string name)
        {
            SocketMessage message = Context.Message;
            SocketUser userToSearch = message.MentionedUsers.ElementAt(0);
            if (userToSearch == null)
            {
                await message.Channel.SendMessageAsync("Sorry, I couldn't find a that user!");
                return;
            }
            if (userToSearch.IsBot)
            {
                await message.Channel.SendMessageAsync("Bots can't be married!");
                return;
            }

            SqlBuilder builder = new SqlBuilder("spCheckUserMarriageStatus", CommandType.StoredProcedure);
            builder.AddParameter("@TARGET", SqlDbType.BigInt, Convert.ToInt64(userToSearch.Id));
            builder.AddParameter("@SERVERID", SqlDbType.BigInt, Convert.ToInt64(message.Channel.Id));
            builder.AddParameter("@RESULT", SqlDbType.BigInt, 0);
            long result = builder.GetReturnValue("@RESULT");
            
            using (var reader = await builder.ExecuteReader())
            {
                try
                {
                    while (await reader.ReadAsync())
                    {
                        marriageQuery marriage = new marriageQuery();
                        marriage.user1 = Convert.ToUInt64(reader["User1"]);
                        marriage.user2 = Convert.ToUInt64(reader["User2"]);
                        IUser user1 = await message.Channel.GetUserAsync(marriage.user1);
                        IUser user2 = await message.Channel.GetUserAsync(marriage.user2);

                        await message.Channel.SendMessageAsync(user1.Username + " is married to " + user2.Username);
                        return;
                    }
                    await message.Channel.SendMessageAsync(userToSearch.Username + " is not married");
                    return;
                }
                catch(Exception e)
                {
                    await message.Channel.SendMessageAsync(e.Message);
                }
            }
        }

        [Command("propose")]
        [Summary("!propose <mention>")]
        [CheckMarriageStatus]
        public async Task ResponseProposal([Remainder] string name)
        {
            SocketMessage message = Context.Message;
            if (message.MentionedUsers.Count == 0)
                return;
            SocketGuildUser sender = (SocketGuildUser)message.Author;
            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);

            if (targetUser.IsBot)
            {
                await message.Channel.SendMessageAsync("You can't marry a bot baka!");
                return;
            }

            CurrentProposals newProposal = new CurrentProposals();
            newProposal.user1 = Convert.ToInt64(sender.Id);
            newProposal.user2 = Convert.ToInt64(targetUser.Id);

            if (proposals.Contains(newProposal))
            {
                await message.Channel.SendMessageAsync("You've already proposed to this person! Hold your horses stud, let them respond!");
                return;
            }
        }

        [Command("acceptproposal")]
        [Summary("<mention> the person that you want to marry. you can be proposed to by multiple people so make sure you type the right name!")]
        public async Task ResponseAcceptProposal([Remainder] string name)
        {
            SocketMessage message = Context.Message;
            SocketGuildUser proposer = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser sender = (SocketGuildUser)message.Author;

            SocketGuild channel = Context.Guild as SocketGuild;

            CurrentProposals? proposal = proposals.Find(prop => prop.GetValueOrDefault().user1 == Convert.ToInt64(proposer.Id) && prop.GetValueOrDefault().user2 == Convert.ToInt64(sender.Id));
            if (proposal == null)
                return;
            
            SqlBuilder builder = new SqlBuilder("InsertMarriage",CommandType.StoredProcedure);
            builder.AddParameter("@User1", SqlDbType.BigInt, Convert.ToInt64(proposer.Id));
            builder.AddParameter("@User2", SqlDbType.BigInt, Convert.ToInt64(sender.Id));
            builder.AddParameter("@ServerID", SqlDbType.BigInt, Convert.ToInt64(channel.Id));

            long result = await builder.ExecuteNonQueryAsync();
            if (result != 0)
            {

                Embedder embedder = new Embedder();
                embedder.SetTitle(sender.Username + " Has Accepted " + proposer.Username + "'s Proposal!");
                embedder.SetDescription("todo");
                embedder.AddImageUrl("https://media1.tenor.com/images/f11b1e79d26818b34939b4e0a69a40e0/tenor.gif");

                await message.Channel.SendMessageAsync("", false, embedder.Build());
                proposals.RemoveAll(x => x?.user1 == Convert.ToInt64(proposer.Id) || x?.user2 == Convert.ToInt64(proposer.Id));
            }
        }

        [Command("rejectproposal")]
        [Summary("<mention> the person that you want to be heartbroken")]
        public async Task ResponseRejectProposal([Remainder] string user)
        {
            SocketMessage message = Context.Message;
            SocketGuildUser proposer = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser rejecter = (SocketGuildUser)message.Author;

            CurrentProposals? proposal = proposals.Find(prop => prop.GetValueOrDefault().user1 == Convert.ToInt64(proposer.Id) && prop.GetValueOrDefault().user2 == Convert.ToInt64(rejecter.Id));
            if (proposal == null)
                return;

            Embedder embedder = new Embedder();
            embedder.SetTitle(rejecter.Username + " Has Rejected " + proposer.Username + "'s Proposal!");
            embedder.SetDescription("todo");
            embedder.AddImageUrl("https://image.myanimelist.net/ui/BQM6jEZ-UJLgGUuvrNkYUCG8p-X1WhZLiR4h-oxkqQdCOemHHt53hbh8NqvLUdgbHx1MAqvh-EbmQQ9qTYikkg");

            await message.Channel.SendMessageAsync("", false, embedder.Build());
            proposals.Remove(proposal);
        }

        [Command("divorce")]
        [Summary("<mention> the person you want to divorce")]
        public async Task ResponseDivorce([Remainder] string user)
        {
            SocketMessage message = Context.Message;

            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser sender = (SocketGuildUser)message.Author;
            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;


            SqlBuilder builder = new SqlBuilder("spDivorceUser", CommandType.StoredProcedure);
            builder.AddParameter("@User1", SqlDbType.BigInt, Convert.ToInt64(sender.Id));
            builder.AddParameter("@User2", SqlDbType.BigInt, Convert.ToInt64(targetUser.Id));
            builder.AddParameter("@ServerID", SqlDbType.BigInt, Convert.ToInt64(channel.Id));
            long rowsAffected = await builder.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {

                Embedder embedder = new Embedder();
                embedder.SetTitle(sender.Username + " Has Divorced " + targetUser.Username + "!");
                embedder.SetDescription("todo");
                embedder.AddImageUrl("https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif");

                await message.Channel.SendMessageAsync("", false, embedder.Build());
            }
        }
    }
}