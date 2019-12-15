using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public struct CurrentProposals
{
    public string user1;
    public string user2;
}

struct Marriages
{
    public string user1;
    public string user2;
};

namespace DCordBot
{
    class Marriage : CommandModule
    {
        private List<Marriages> marriages = new List<Marriages>();
        private List<CurrentProposals> proposals = new List<CurrentProposals>();
        public Marriage()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Marriages.xml");

            foreach (XmlNode childNode in xmlDoc.FirstChild)
            {
                if(childNode.Name == "MARRIAGE")
                {
                    Marriages marriage = new Marriages();
                    marriage.user1 = childNode.Attributes["user1"].Value;
                    marriage.user2 = childNode.Attributes["user2"].Value;

                    marriages.Add(marriage);
                }
            }
        }

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
            foreach(Marriages marriage in marriages)
            {
                string user = message.Author.Username;
                if(user == marriage.user1)
                {
                    response = message.Author.Username + " is married to " + marriage.user2;
                }
                else if(user == marriage.user2)
                {
                    response = message.Author.Username + " is married to " + marriage.user1;
                }
                await message.Channel.SendMessageAsync(response);
            }
        }

        public async Task ResponseProposal(SocketMessage message)
        {
            if(message.MentionedUsers.Count > 1)
            {
                //no polygamy;
                return;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load("Marriages.xml");
            XmlNode rootNode = doc.FirstChild;

            foreach(Marriages marriage in marriages)
            {
                if(message.MentionedUsers.ElementAt(0).Username == marriage.user1 || message.MentionedUsers.ElementAt(0).Username == marriage.user2)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title = message.MentionedUsers.ElementAt(0).Username + " is already married!"
                    };
                    await message.Channel.SendMessageAsync("", false, embed.Build());
                    return;
                }

                if(message.Author.Username == marriage.user1 || message.Author.Username == marriage.user2)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title ="You're already married!"
                    };
                    await message.Channel.SendMessageAsync("", false, embed.Build());
                    return;
                }
            }

            if (proposals.Count == 0)
            {
                CurrentProposals newProposal = new CurrentProposals();
                newProposal.user1 = message.Author.Username;
                newProposal.user2 = message.MentionedUsers.ElementAt(0).Username;
                proposals.Add(newProposal);

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = newProposal.user1 + " Is Proposing to " + newProposal.user2 + "!",
                    Description = "to be filled, type !acceptproposal or !rejectproposal"
                };

                await message.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }
            foreach(CurrentProposals proposal in proposals)
            {
                if((proposal.user1 == message.Author.Username || proposal.user2 == message.Author.Username) && (proposal.user2 == message.MentionedUsers.ElementAt(0).Username ||
                    proposal.user1 == message.MentionedUsers.ElementAt(0).Username))
                {
                    return;
                }
                else
                {
                    CurrentProposals newProposal = new CurrentProposals();
                    newProposal.user1 = message.Author.Username;
                    newProposal.user2 = message.MentionedUsers.ElementAt(0).Username;
                    proposals.Add(newProposal);

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
        }

        public async Task ResponseAcceptProposal(SocketMessage message)
        {
            string responder = message.Author.Username;
            foreach(CurrentProposals proposal in proposals)
            {
                string acceptedProposal = message.MentionedUsers.ElementAt(0).Username;
                if(proposal.user1 == acceptedProposal && responder == proposal.user2)     
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("Marriages.xml");
                    XmlNode childNode = doc.CreateNode(XmlNodeType.Element, "MARRIAGE", "");
                    XmlAttribute attr = doc.CreateAttribute("user1");
                    attr.Value = acceptedProposal;
                    childNode.Attributes.Append(attr);
                    attr = doc.CreateAttribute("user2");
                    attr.Value = responder;
                    childNode.Attributes.Append(attr);
                    doc.DocumentElement.AppendChild(childNode);
                    doc.Save("Marriages.xml");

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title = proposal.user2 + " Has Accepted " + acceptedProposal + "'s Proposal!",
                        Description = "todo",
                        ImageUrl = "https://media1.tenor.com/images/f11b1e79d26818b34939b4e0a69a40e0/tenor.gif"

                    };

                    await message.Channel.SendMessageAsync("", false, embed.Build());
                    proposals.Remove(proposal);

                    Marriages marriage = new Marriages();
                    marriage.user1 = proposal.user1;
                    marriage.user2 = proposal.user2;
                    marriages.Add(marriage);
                    proposals.RemoveAll(x => x.user1 == acceptedProposal || x.user2 == acceptedProposal);
                    return;
                }
            }
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
            string userToDivorce = message.MentionedUsers.ElementAt(0).Username;
            string author = message.Author.Username;
            XmlDocument doc = new XmlDocument();
            doc.Load("Marriages.xml");

            XmlNode rootNode = doc.FirstChild;
            foreach(XmlNode childNode in rootNode)
            {
                if (childNode.Name == "MARRIAGE")
                {
                    string user1 = childNode.Attributes["user1"].Value;
                    string user2 = childNode.Attributes["user2"].Value;

                    if(message.Author.Username == user1 || message.Author.Username == user2)
                    {
                        if(userToDivorce == user1 || userToDivorce == user2)
                        {


                            EmbedBuilder embed = new EmbedBuilder()
                            {
                                Color = Color.Red,
                                Title = message.Author.Username + " Has Divorced " + userToDivorce + "!",
                                Description = "todo",
                                ImageUrl = "https://pa1.narvii.com/6289/9341ed205608180606dd197a733d3f9392a3f8b1_hq.gif"

                            };

                           await message.Channel.SendMessageAsync("", false, embed.Build());

                            rootNode.RemoveChild(childNode);
                        }
                    }
                }
            }
            doc.Save("Marriages.xml");
            foreach (Marriages marriage in marriages)
            {
                if (marriage.user1 == userToDivorce || marriage.user2 == userToDivorce)
                {
                    marriages.Remove(marriage);
                }
            }
        }
    }
}
