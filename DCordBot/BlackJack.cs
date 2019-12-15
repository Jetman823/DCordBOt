using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public enum Cards
{
    card_ace = 1,
    card_two = 2,
    card_three = 3,
    card_four = 4,
    card_five = 5,
    card_six = 6,
    card_seven = 7,
    card_eight = 8,
    card_nine = 9,
    card_ten = 10,
    card_jack = 10,
    card_queen = 10,
    card_king = 10
};

public enum Suits
{
    suit_clubs = 1,
    suit_spades = 2,
    suit_hearts = 3,
    suit_diamons = 4
}

struct BJPlayers
{
    public string userID;
    public int gamesWon;
    public int gamesLost;
    public int playerScore;
    public int botScore;
    public Dictionary<Suits, List<Cards>> currentHand;
}

namespace DCordBot
{
    class BlackJack : CommandModule
    {
        SortedDictionary<string, BJPlayers> bjPlayers = new SortedDictionary<string, BJPlayers>();

        public BlackJack()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("BJPlayers.xml");
            XmlNode rootNode = doc.DocumentElement;

            foreach (XmlNode childNode in rootNode)
            {
                if (childNode.Name == "PLAYER")
                {
                    BJPlayers player = new BJPlayers
                    {
                        userID = childNode.Attributes["userID"].Value,
                        gamesWon = Convert.ToInt32(childNode.Attributes["gamesWon"].Value),
                        gamesLost = Convert.ToInt32(childNode.Attributes["gamesLost"].Value),
                        playerScore = Convert.ToInt32(childNode.Attributes["playerScore"].Value),
                        botScore = Convert.ToInt32(childNode.Attributes["botScore"].Value)
                    };

                    bjPlayers.Add(player.userID, player);
                }
            }
        }
        public override async Task Response(SocketMessage message, CommandInfo command)
        {
            switch (command.commandName.ToLower())
            {
                case "!bjnew":
                    {
                        await ResponseBjNew(message, message.Author);
                    }
                    break;
                case "!bjhit":
                    {
                        await ResponseBjHit(message, message.Author);
                    }
                    break;
                case "!bjstay":
                    {
                        //await ResponseGelBooru(message);
                    }
                    break;
                case "!bjwin":
                    {
                        await ResponseBjWin(message, message.Author);
                    }
                    break;
                case "!bjlose":
                    {
                        await ResponseBjLose(message, message.Author);
                    }
                    break;
                default:
                    {
                        return;
                    }
            }
        }

        //TODO: finish this
        private async Task ResponseBjNew(SocketMessage message, SocketUser sender)
        {
            if (!bjPlayers.ContainsKey(sender.Username))
            {
                BJPlayers bjPlayer = new BJPlayers
                {
                    userID = sender.Username,
                    gamesWon = 0,
                    gamesLost = 0,
                    playerScore = 0,
                    botScore = 0
                };
                bjPlayers.Add(bjPlayer.userID, bjPlayer);

                XmlDocument doc = new XmlDocument();
                doc.Load("BJPlayers.xml");
                XmlNode childNode = doc.CreateNode(XmlNodeType.Element, "PLAYER", "");
                XmlAttribute attr = doc.CreateAttribute("userID");
                attr.Value = bjPlayer.userID;
                childNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("gamesWon");
                attr.Value = Convert.ToString(bjPlayer.gamesWon);
                childNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("gamesLost");
                attr.Value = Convert.ToString(bjPlayer.gamesLost);
                childNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("playerScore");
                attr.Value = Convert.ToString(bjPlayer.playerScore);
                childNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("botScore");
                attr.Value = Convert.ToString(bjPlayer.botScore);
                childNode.Attributes.Append(attr);
                doc.DocumentElement.AppendChild(childNode);
                doc.Save("BJPlayers.xml");
            }


            //XmlDocument doc = new XmlDocument();
            //doc.Load("BJ.xml");
            //XmlNode node = doc.FirstChild;
            await ResponseBjWin(message, sender);
            //await message.Channel.SendMessageAsync("To be implemented! :)");
        }

        private async Task ResponseBjHit(SocketMessage message, SocketUser sender)
        {
            if (!bjPlayers.ContainsKey(sender.Username))
                return;

            BJPlayers player = bjPlayers[sender.Username];
            Random randomSuit = new Random();
            Random randomCard = new Random();
            await message.Channel.SendMessageAsync("To be implemented! :)");
        }

        private async Task ResponseBjStay(SocketMessage message, SocketUser sender)
        {
            return;
        }

        private async Task ResponseBjWin(SocketMessage message, SocketUser sender)
        {
            string response = "YOU WIN! Your score is now ";

            XmlDocument doc = new XmlDocument();
            doc.Load("BJPlayers.xml");

            XmlNode rootElement = doc.DocumentElement;

            foreach (XmlNode childNode in rootElement)
            {
                if (childNode.Attributes["userID"].Value.Equals(sender.Username))
                {
                    childNode.Attributes["gamesWon"].Value = Convert.ToString(Convert.ToInt32(childNode.Attributes["gamesWon"].Value) + 1);
                    response += childNode.Attributes["gamesWon"].Value;

                }
            }

            doc.Save("BJPlayers.xml");


            await message.Channel.SendMessageAsync(response);
        }

        private async Task ResponseBjLose(SocketMessage message, SocketUser sender)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("BJPlayers.xml");

            XmlNode rootNode = doc.DocumentElement;

            foreach (XmlNode childNode in rootNode)
            {
                if (childNode.Attributes["userID"].Value.Equals(sender.Username))
                {
                    childNode.Attributes["gamesLost"].Value = Convert.ToString(Convert.ToInt32(childNode.Attributes["gamesLost"].Value) + 1);

                    doc.Save("BJPlayers.xml");

                    string response = "YOU LOSE! You have lost ";
                    response += Convert.ToString(childNode.Attributes["gamesLost"].Value) + "games";

                    await message.Channel.SendMessageAsync(response);
                    break;
                }
            }
        }

    }
}