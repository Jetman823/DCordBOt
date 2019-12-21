using Discord.WebSocket;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public struct CharInfo
{
    public int Level;
    public int XP, BP;
    public int CID, CLID;
    public string clanName;
    public int killCount, deathCount;
    public byte sex;
}

namespace DCordBot
{
    class GunzCommands : CommandModule
    {
        public override async Task Response(SocketMessage message, CommandInfo command)
        {
            string commandName = command.commandName;
            switch (commandName.ToLower())
            {
                case "!charinfo":
                    {
                        int argc = message.Content.Count(count => count == ' ');
                        if (argc < command.argc || argc > command.argc)
                            return;

                        await ResponseCharInfo(message, message.Content.Substring(10));
                    }
                    break;
                case "!serverstatus":
                    {
                        await ResponseServerStatus(message);
                    }
                    break;
                case "!getitem":
                    {
                        int argc = message.Content.Count(count => count == ' ');
                        if (argc < command.argc)
                            return;
                        if (!message.Content.Contains("\""))
                        {
                            await message.Channel.SendMessageAsync("Please put quotes around the item name!");
                            return;
                        }
                        await ResponseItemInfo(message, message.Content.Substring(9));
                    }
                    break;
                default:
                    return;
            }
        }
        private async Task ResponseCharInfo(SocketMessage message, string charName)
        {
            Console.Write(charName);
            SqlCommand command = Program.connection.CreateCommand();
            if (charName.Contains("@'|?!=-\""))
                return;
            command.CommandText = @"SELECT [Level],XP,BP ,CID ,KillCount,DeathCount,[Sex] FROM [dbo].[Character] WHERE Name = @NAME";
            command.Parameters.Add("@NAME", System.Data.SqlDbType.VarChar, 24);

            CharInfo charInfo = new CharInfo();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    charInfo.Level = reader.GetInt16(0);
                    charInfo.XP = reader.GetInt32(1);
                    charInfo.BP = reader.GetInt32(2);
                    charInfo.CID = reader.GetInt32(3);
                    charInfo.killCount = reader.GetInt32(4);
                    charInfo.deathCount = reader.GetInt32(5);
                    charInfo.sex = reader.GetByte(6);
                }

            }
            catch (SqlException exception)
            {
                Console.Write(exception.Message);
                return;
            }
            reader.Close();
            command.Dispose();

            command = Program.connection.CreateCommand();
            command.CommandText = @"SELECT [CLID] FROM [dbo].[ClanMember] WHERE CID = @CID";
            SqlParameter sqlParameter = new SqlParameter("@CID", charInfo.CID);
            command.Parameters.Add(sqlParameter);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                charInfo.CLID = reader.GetInt32(0);
            }
            reader.Close();
            command.Dispose();

            command = Program.connection.CreateCommand();
            command.CommandText = @"SELECT [Name] FROM [dbo].[Clan] WHERE CLID = @CLID";
            sqlParameter = new SqlParameter("@CLID", charInfo.CLID);
            command.Parameters.Add(sqlParameter);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                charInfo.clanName = reader.GetString(0);
            }
            reader.Close();
            command.Dispose();

            Embedder embedder = new Embedder();
            embedder.SetTitle("Character Information:\n");

            string response = "Clan Name: " + charInfo.clanName + "\n" + "Level: " + charInfo.Level + "\n" + "XP: " + charInfo.XP + "\n" + "BP: " + charInfo.BP + "\n" +
                "KillCount: " + charInfo.killCount + "\n" + "DeathCount: " + charInfo.deathCount + "\n" + "Sex: ";
            if (charInfo.sex == 0)
                response += "Male";
            else
                response += "Female";

            embedder.SetDescription(response);


            await message.Channel.SendMessageAsync("", false, embedder.Build());
        }

        private async Task ResponseServerStatus(SocketMessage message)
        {
            int playerCount = 0;
            SqlCommand command = Program.connection.CreateCommand();
            command.CommandText = @"SELECT [CurrPlayer] FROM [dbo].[ServerStatus](NOLOCK)";

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    try
                    {
                        playerCount = Convert.ToInt32(reader.GetInt16(0));
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Write(exception.Message);
                return;
            }
            reader.Close();
            command.Dispose();

            string response = "The Current PlayerCount is " + playerCount.ToString() + "\n";
            await message.Channel.SendMessageAsync(response);
        }

        private async Task ResponseItemInfo(SocketMessage message, string itemName)
        {
            itemName = itemName.Replace("\"", "");
            itemName = itemName.ToLower();
            ZItemInfo itemInfo;
            foreach (ZItemInfo item in ZItemManager.zItemList)
            {
                string tolowerName = item.name.ToLower();
                if (tolowerName == itemName)
                {

                    itemInfo = item;

                    string response = "Name: " + item.name + "\n" + "Desc: " + item.desc + "\n" + "Type: " + itemInfo.type + "\n" + "Slot: " + itemInfo.slot + "\n" + "Delay: " + itemInfo.delay + "\n" +
                        "Damage: " + itemInfo.damage + "\n";

                    Embedder embedder = new Embedder();
                    embedder.SetTitle("Item Information:\n");
                    embedder.SetDescription(response);


                    await message.Channel.SendMessageAsync("", false, embedder.Build());

                }
            }

            await message.Channel.SendMessageAsync("Item not found!");
        }

    }
}
