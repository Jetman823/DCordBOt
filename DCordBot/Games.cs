using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public struct GameInfo
{
    public string name;
};

namespace DCordBot
{
    class Games
    {
        public static List<GameInfo> games = null;

        public bool Load()
        {


            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("Games.xml");
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }

            games = new List<GameInfo>();

            XmlNode rootNode = doc.DocumentElement;
            foreach(XmlNode childNode in rootNode)
            {
                if(childNode.Name == "GAME")
                {
                    GameInfo game = new GameInfo
                    {
                        name = childNode.Attributes["name"].Value
                    };

                    games.Add(game);
                }
            }

            return true;
        }
    }
}
