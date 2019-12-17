using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;



public struct CommandInfo
{
    public string commandName;
    public int commandType;
    public byte argc;
    public string desc;
    public int kawaiiType;
};

namespace DCordBot
{
     class CommandModule
    {
        public List<CommandInfo> commandList = new List<CommandInfo>();
        private static GunzCommands gunzCommands = new GunzCommands();
        private static NSFWCommands nsfwCommands = new NSFWCommands();
        private static AdminCommands adminCommands = new AdminCommands();
        private static KawaiiCommands kawaiiCommands = new KawaiiCommands();
        private static Marriage marriageCommands = new Marriage();

        public void Load()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Commands.xml");
            XmlNode node = doc.FirstChild;

            foreach (XmlNode childNode in node.FirstChild)
            {
                if (childNode.Name == "COMMAND")
                {
                    CommandInfo comm = new CommandInfo
                    {
                        commandName = childNode.InnerText,
                        commandType = Convert.ToInt32(childNode.Attributes["type"].Value),
                        argc = Convert.ToByte(childNode.Attributes["argc"].Value),
                        desc = Convert.ToString(childNode.Attributes["desc"] == null ? "" : childNode.Attributes["desc"].Value ),
                        kawaiiType = Convert.ToInt32(childNode.Attributes["kawaiiType"] == null ? "-1" : childNode.Attributes["kawaiiType"].Value)
                    };
                    commandList.Add(comm);
                }
            }
        }
        public virtual async Task Response(SocketMessage message, CommandInfo commandInfo)
        {
            string commandName = commandInfo.commandName;
            switch (commandName.ToLower())
            {
                case "!help":
                    {
                        string response = "List of Commands: !charinfo, !serverstatus, !getitem";
                        response += "How to use commands:\n";
                        response += "!charinfo playername\n";
                        response += "!serverstatus (just type !serverstatus)\n";
                        response += "!getitem \"Item Name\"\n";
                        await message.Channel.SendMessageAsync(response);
                    }
                    break;
                case "!play":
                    {
                        ///todo:
                    }break;
                default:
                    break;
            }
            await gunzCommands.Response(message, commandInfo);
            await nsfwCommands.Response(message, commandInfo);
            await adminCommands.Response(message, commandInfo);
            await kawaiiCommands.Response(message, commandInfo);
            await marriageCommands.Response(message, commandInfo);
        }
    }
}
