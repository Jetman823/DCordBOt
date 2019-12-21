using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// I know I can simplify it, don't want to bitch. I don't want to add brackets for 2 or 3 variables....
/// </summary>
#pragma warning disable IDE0017

public struct CurrentProposals
{
    public long user1;
    public long user2;
}

namespace DCordBot
{
    class Marriage : CommandModule
    {
        private List<CurrentProposals?> proposals = new List<CurrentProposals?>();

        override public async Task Response(SocketMessage message, CommandInfo command)
        {
            switch (command.commandName.ToLower())
            {
                case "!marriagestatus":
                    {
                        await ResponseMarriageStatus(message);
                    }
                    break;
                case "!propose":
                    {
                        await ResponseProposal(message);
                    }
                    break;
                case "!acceptproposal":
                    {
                        await ResponseAcceptProposal(message);
                    }
                    break;
                case "!rejectproposal":
                    {
                        await ResponseRejectProposal(message);
                    }
                    break;
                case "!divorce":
                    {
                        await ResponseDivorce(message);
                    }
                    break;
                default:
                    return;
            }
        }

        public async Task ResponseMarriageStatus(SocketMessage message)
        {
            string response = "";
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

            SocketUser sender = message.Author;
            SqlCommand command = new SqlCommand("spCheckUserMarriageStatus", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", Convert.ToInt64(userToSearch.Id));
            command.Parameters.AddWithValue("@ServerID", Convert.ToInt64(((SocketGuildChannel)message.Channel).Id));
            command.Parameters.Add("@RESULT", SqlDbType.BigInt);
            command.Parameters["@RESULT"].Direction = ParameterDirection.ReturnValue;
            SqlDataReader reader = null;

            long result = 0;
            try
            {
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    await message.Channel.SendMessageAsync(userToSearch.Username + " Is not Married!");
                }
                else
                {

                    while (reader.Read())
                    {
                        result = reader.GetInt64(0);
                    }

                    IUser user = await message.Channel.GetUserAsync(Convert.ToUInt64(result));
                    response = userToSearch.Username + " is married to " + user.Username;
                    await message.Channel.SendMessageAsync(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            reader.Close();
        }

        public async Task ResponseProposal(SocketMessage message)
        {
            SocketGuildUser sender = (SocketGuildUser)message.Author;
            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);

            //if (sender.Username == targetUser.Username)
            //{
            //    await message.Channel.SendMessageAsync("You can't marry yourself baka!");
            //    return;
            //}

            if (targetUser == null)
                return;

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

            SqlCommand command = new SqlCommand("spProposeToUser", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@User1", SqlDbType.BigInt).Value = Convert.ToInt64(sender.Id);
            command.Parameters.AddWithValue("@User2", SqlDbType.BigInt).Value = Convert.ToInt64(targetUser.Id);
            command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = Convert.ToInt64(((SocketGuildChannel)message.Channel).Id);
            command.Parameters.Add("@RESULT", SqlDbType.Int);
            command.Parameters["@RESULT"].Direction = ParameterDirection.ReturnValue;


            byte returnValue = 0;
            SqlDataReader reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                Embedder embedder = new Embedder();
                embedder.SetTitle(sender.Username + " Is Proposing to " + targetUser.Username + "!");
                embedder.SetDescription("to be filled, type !acceptproposal or !rejectproposal");
                embedder.AddImageUrl("");

                await message.Channel.SendMessageAsync("", false, embedder.Build());
                proposals.Add(newProposal);
            }
            else
            {
                while (reader.Read())
                {
                    try
                    {
                        returnValue = reader.GetByte(3);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Embedder embedder = new Embedder();
                if (returnValue == 1)
                    embedder.SetTitle(targetUser.Username + " is already married!");
                else if (returnValue == 2)
                    embedder.SetTitle("You're already married! You must divorce your current spouse first.");


                await message.Channel.SendMessageAsync("", false, embedder.Build());
            }
            reader.Close();
        }

        public async Task ResponseAcceptProposal(SocketMessage message)
        {
            SocketGuildUser proposer = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser sender = (SocketGuildUser)message.Author;

            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;

            CurrentProposals? proposal = proposals.Find(prop => prop.GetValueOrDefault().user1 == Convert.ToInt64(proposer.Id) && prop.GetValueOrDefault().user2 == Convert.ToInt64(sender.Id));
            if (proposal == null)
                return;

            Embedder embedder = new Embedder();
            embedder.SetTitle(sender.Username + " Has Accepted " + proposer.Username + "'s Proposal!");
            embedder.SetDescription("todo");
            embedder.AddImageUrl("https://media1.tenor.com/images/f11b1e79d26818b34939b4e0a69a40e0/tenor.gif");

            await message.Channel.SendMessageAsync("", false, embedder.Build());

            SqlCommand command = new SqlCommand("InsertMarriage", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@User1", SqlDbType.BigInt).Value = Convert.ToInt64(proposer.Id);
            command.Parameters.Add("@User2", SqlDbType.BigInt).Value = Convert.ToInt64(sender.Id);
            command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = channel.Id;
            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            proposals.RemoveAll(x => x?.user1 == Convert.ToInt64(proposer.Id) || x?.user2 == Convert.ToInt64(proposer.Id));
        }

        public async Task ResponseRejectProposal(SocketMessage message)
        {
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

        public async Task ResponseDivorce(SocketMessage message)
        {
            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser sender = (SocketGuildUser)message.Author;

            SqlCommand command = new SqlCommand("spDivorceUser", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@User1", SqlDbType.BigInt).Value = sender.Id;
            command.Parameters.Add("@User2", SqlDbType.BigInt).Value = targetUser.Id;
            command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = ((SocketGuildChannel)message.Channel).Id;

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return;
            }
            reader.Close();

            Embedder embedder = new Embedder();
            embedder.SetTitle(sender.Username + " Has Divorced " + targetUser.Username + "!");
            embedder.SetDescription("todo");
            embedder.AddImageUrl("https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif");

            await message.Channel.SendMessageAsync("", false, embedder.Build());
        }
    }
}
