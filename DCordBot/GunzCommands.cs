using Discord.Commands;
using System;
using System.Data.SqlClient;
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
    public class GunzCommands : CommandHandler
    {
#if GUNZ
        [Command("charinfo")]
        [Summary("!charinfo username")]
        public async Task ResponseCharInfo([Remainder] string charName)
        {
            Console.Write(charName);
            SqlCommand command = Program.connection.CreateCommand();
            if (charName.Anyof("@'|?!=-\""))
                return;
            command.CommandText = @"SELECT [Level],XP,BP ,CID ,KillCount,DeathCount,[Sex] FROM [dbo].[Character] WHERE Name = @NAME";
            command.Parameters.Add("@NAME", System.Data.SqlDbType.VarChar, 24).Value = charName;

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


            await Context.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("serverstatus")]
        [Summary("gets the current online/offline status of the server")]
        private async Task ResponseServerStatus()
        {
            int playerCount = 0;
            bool opened = false;
            SqlCommand command = Program.connection.CreateCommand();
            command.CommandText = @"SELECT [CurrPlayer],[Opened] FROM [dbo].[ServerStatus](NOLOCK)";

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    try
                    {
                        playerCount = Convert.ToInt32(reader.GetInt16(0));
                        opened = Convert.ToBoolean(reader.GetByte(1));
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

            string response = "The Server is ";
            if (opened == true)
                response += "Online";
            else
                response += "Offline";

            response += "\nThe Current PlayerCount is " + playerCount.ToString() + "\n";

            Embedder embedder = new Embedder();
            embedder.SetTitle("Server Status:\n");
            embedder.SetDescription(response);
            embedder.AddImageUrl(opened == false ? "https://media.tenor.com/images/a8ef87ef4b9b23c2f59c31418cb01097/tenor.gif" :
                "https://thumbs.gfycat.com/BlindHonestFirefly.webp");


            await Context.Channel.SendMessageAsync("", false, embedder.Build());
        }

        [Command("iteminfo")]
        [Summary("!iteminfo itemname")]
        private async Task ResponseItemInfo([Remainder] string itemName)
        {
            itemName = itemName.Replace("\"", "");
            itemName = itemName.ToLower();
            ZItemInfo? item = ZItemManager.zItemList.Find(find => find.name == itemName);
            if(item != null)
            {
                string response = "Name: " + item.Value.name + "\n" + "Desc: " + item.Value.desc + "\n" + "Type: " + item.Value.type + "\n" + "Slot: " + item.Value.slot + "\n" + "Delay: " + item.Value.delay + "\n" +
                    "Damage: " + item.Value.damage + "\n";

                Embedder embedder = new Embedder();
                embedder.SetTitle("Item Information:\n");
                embedder.SetDescription(response);


                await Context.Channel.SendMessageAsync("", false, embedder.Build());
                return;
            }
            await Context.Channel.SendMessageAsync("Item not found!");
        }

    }
#endif

    }
}