using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public struct CurrentProposals
{
    public string user1;
    public string user2;
}



public class MarriageStruct
{
    public string _user1 { get; set; }
    public string _user2 { get; set; }
}

namespace DCordBot
{
    class Marriage : CommandModule
    {
        private List<CurrentProposals> proposals = new List<CurrentProposals>();

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
                    }break;
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

            SqlCommand command = new SqlCommand("CheckUserMarriageStatus", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@User1", message.Author.Username);

            SqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                return;
            }
            MarriageStruct marriage = new MarriageStruct();
            marriage._user1 = reader.GetString(0);
            marriage._user2 = reader.GetString(1);
            reader.Close();
            command.Dispose();

            string user = message.Author.Username;
            if (user == marriage._user1)
            {
                response = message.Author.Username + " is married to " + marriage._user2;
            }
            else if (user == marriage._user2)
            {
                response = message.Author.Username + " is married to " + marriage._user1;
            }
            await message.Channel.SendMessageAsync(response);
        }

        public async Task ResponseProposal(SocketMessage message)
        {
            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            var roles = targetUser.Roles;

            SqlCommand command = new SqlCommand("CheckUserMarriageStatus", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@User1", SqlDbType.VarChar, 50).Value = targetUser.Username;
            command.Parameters.Add("@User2", SqlDbType.VarChar, 50).Value = message.Author.Username;
            command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = ((SocketGuildUser)message.Channel).Id;

            int returnValue = 0;
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                returnValue = reader.GetInt32(0);
            }
            if(returnValue == 1)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = targetUser.Username + " is already married!"
                };
                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }
            else if(returnValue == 2)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = "You're already married! You must divorce your current spouse."
                };
                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            else if(returnValue == 3)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = "You're both already married! You must divorce your current spouses."
                };
                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            CurrentProposals newProposal = new CurrentProposals();
            newProposal.user1 = message.Author.Username;
            newProposal.user2 = message.MentionedUsers.ElementAt(0).Username;
            proposals.Add(newProposal);
            foreach (CurrentProposals proposal in proposals)
            {
                if((proposal.user1 == message.Author.Username || proposal.user2 == message.Author.Username) && (proposal.user2 == message.MentionedUsers.ElementAt(0).Username ||
                    proposal.user1 == message.MentionedUsers.ElementAt(0).Username))
                {
                    return;
                }
                else
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title = newProposal.user1 + " Is Proposing to " + newProposal.user2 + "!",
                        Description = "to be filled, type !acceptproposal or !rejectproposal",
                        ImageUrl = ""
                    };

                    await message.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            return;
        }

        public async Task ResponseAcceptProposal(SocketMessage message)
        {
            SocketGuildUser responder = (SocketGuildUser)message.Author;
            SocketGuildUser sender = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildChannel guild = message.Channel as SocketGuildChannel;
            foreach(CurrentProposals proposal in proposals)
            {
                string acceptedProposal = message.MentionedUsers.ElementAt(0).Username;
                if(proposal.user1 == sender.Username && responder.Username == proposal.user2)     
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title = proposal.user2 + " Has Accepted " + acceptedProposal + "'s Proposal!",
                        Description = "todo",
                        ImageUrl = "https://media1.tenor.com/images/f11b1e79d26818b34939b4e0a69a40e0/tenor.gif"

                    };


                    await message.Channel.SendMessageAsync("", false, embed.Build());

                    SqlCommand command = new SqlCommand("InsertMarriage", Program.botConnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@User1", SqlDbType.VarChar, 50).Value = proposal.user2;
                    command.Parameters.Add("@User2", SqlDbType.VarChar, 50).Value = acceptedProposal;
                    command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = guild.Id;
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                    command.Dispose();
                    proposals.Remove(proposal);
                }
            }
            return;
        }

        public async Task ResponseRejectProposal(SocketMessage message)
        {
            string rejector = message.Author.Username;
            string proposer = message.MentionedUsers.ElementAt(0).Username;

            foreach(CurrentProposals proposal in proposals)
            {
                if(proposal.user1 == rejector || proposal.user2 == rejector)
                {
                    if(proposal.user1 == proposer || proposal.user2 == proposer)
                    {
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Color = Color.Red,
                            Title = rejector + " Has Rejected " + proposer + "'s Proposal!",
                            Description = "todo",
                            ImageUrl = "https://image.myanimelist.net/ui/BQM6jEZ-UJLgGUuvrNkYUCG8p-X1WhZLiR4h-oxkqQdCOemHHt53hbh8NqvLUdgbHx1MAqvh-EbmQQ9qTYikkg"

                        };

                        await message.Channel.SendMessageAsync("", false, embed.Build());
                        proposals.Remove(proposal);
                    }
                }
            }
        }

        public async Task ResponseDivorce(SocketMessage message)
        {
            SocketGuildUser targetUser = (SocketGuildUser)message.MentionedUsers.ElementAt(0);
            SocketGuildUser sender = (SocketGuildUser)message.MentionedUsers.ElementAt(0);

            SqlCommand command = new SqlCommand("DivorceUser", Program.botConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@User1", SqlDbType.VarChar, 50).Value = sender.Username;
            command.Parameters.Add("@User2", SqlDbType.VarChar, 50).Value = targetUser.Username;
            command.Parameters.Add("@ServerID", SqlDbType.BigInt).Value = ((SocketGuildChannel)message.Channel).Id;

            int result = 0;
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                result = reader.GetInt32(0);
            }
            if (result == 1)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = sender.Username + " Has Divorced " + targetUser.Username + "!",
                    Description = "todo",
                    ImageUrl = "https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif"

                };

                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }
            else if (result == 2)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = sender.Username + " Has Divorced " + targetUser.Username + "!",
                    Description = "todo",
                    ImageUrl = "https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif"

                };

                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = sender.Username + " Has Divorced " + targetUser.Username + "!",
                    Description = "todo",
                    ImageUrl = "https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif"
                };

                await message.Channel.SendMessageAsync("", false, embed.Build());
            }
            return;
        }
    }
}
