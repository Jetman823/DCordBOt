using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static DCordBot.CustomPreconditions;

public struct HiScores
{
    public string SkillName;
    public int Rank;
    public long TotalExp;
    public short Level;
}

namespace DCordBot
{
    public class Runescape : CommandHandler
    {
        [Command("GetOSRSHiScore")]
        [Summary("!GetOSRSHiScore UserName")]
        [RequireCoolDown]
        public async Task GetHiScoreByUserOSRS([Remainder] string userName)
        {
            string webURL = "https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=";
            webURL += userName;
            Uri uri = new Uri(webURL);
            using (var webClient = new WebClient())
            {
                string contents = await webClient.DownloadStringTaskAsync(uri);
                string[] stats = contents.Split('\n').Take(24).ToArray();
                if (stats == null)
                    return;

                List<HiScores> allSkills = new List<HiScores>();
                int i = 0;
                foreach (string stat in stats)
                {
                    HiScores score = new HiScores();
                    score.SkillName = GetSkillNameByIndex(i);
                    var parts = stat.Split(',');
                    score.Rank = Convert.ToInt32(parts[0]);
                    score.TotalExp = Convert.ToInt64(parts[2]);
                    score.Level = Convert.ToInt16(parts[1]);
                    ++i;
                    allSkills.Add(score);
                }

                Embedder embedder = new Embedder();
                string title = userName + "'s OSRS HiScores!";
                embedder.SetTitle(title);
                foreach (HiScores score in allSkills)
                {
                    string fieldText = "Rank: " + score.Rank + "    TotalExp: " + score.TotalExp + "    Level: " + score.Level + "\n";
                    embedder.AddField(score.SkillName, fieldText);
                }

                await Context.Channel.SendMessageAsync("", false, embedder.Build());
            }
        }

        [Command("GetRs3HiScore")]
        [Summary("!GetR3HiScore username here")]
        public async Task GetRs3HiScoreByName([Remainder] string userName)
        {
            string webURL = "https://secure.runescape.com/m=hiscore/index_lite.ws?player=";
            webURL += userName;
            Uri uri = new Uri(webURL);
            using (var webClient = new WebClient())
            {
                string contents = await webClient.DownloadStringTaskAsync(uri);
                string[] stats = contents.Split('\n').Take(29).ToArray();
                if (stats == null)
                    return;

                List<HiScores> allSkills = new List<HiScores>();
                int i = 0;
                foreach (string stat in stats)
                {
                    HiScores score = new HiScores();
                    score.SkillName = GetSkillNameByIndex(i);
                    var parts = stat.Split(',');
                    score.Rank = Convert.ToInt32(parts[0]);
                    score.TotalExp = Convert.ToInt64(parts[2]);
                    score.Level = Convert.ToInt16(parts[1]);
                    ++i;
                    allSkills.Add(score);
                }

                Embedder embedder = new Embedder();
                string title = userName + "'s HiScores!";
                embedder.SetTitle(title);
                foreach (HiScores score in allSkills)
                {
                    if(embedder.Fields.Count == 25)
                    {
                        await Context.Channel.SendMessageAsync("", false, embedder.Build());
                        continue;
                    }
                    if (embedder.Fields.Count > 25)
                    {
                        continue;
                    }
                    string fieldText = score.SkillName + ": " + "Rank: " + score.Rank + "    TotalExp: " + score.TotalExp + "    Level: " + score.Level + "\n";
                    embedder.AddField(score.SkillName, fieldText);
                }
                embedder.Fields.Clear();
                List<HiScores> lastSkills = allSkills.GetRange(25, 4);
                foreach(HiScores score in lastSkills)
                {
                    string fieldText = score.SkillName + ": " + "Rank: " + score.Rank + "    TotalExp: " + score.TotalExp + "    Level: " + score.Level + "\n";
                    embedder.AddField(score.SkillName, fieldText);
                }

                await Context.Channel.SendMessageAsync("", false, embedder.Build());
            }
        }

        public string GetSkillNameByIndex(int i)
        {
            switch (i)
            {
                case 0:
                    return "Overall";
                case 1:
                    return "Attack";
                case 2:
                    return "Defense";
                case 3:
                    return "Strength";
                case 4:
                    return "Constitution";
                case 5:
                    return "Ranged";
                case 6:
                    return "Prayer";
                case 7:
                    return "Magic";
                case 8:
                    return "Cooking";
                case 9:
                    return "Woodcutting";
                case 10:
                    return "Fletching";
                case 11:
                    return "Fishing";
                case 12:
                    return "Firemaking";
                case 13:
                    return "Crafting";
                case 14:
                    return "Smithing";
                case 15:
                    return "Mining";
                case 16:
                    return "Herblore";
                case 17:
                    return "Agility";
                case 18:
                    return "Thieving";
                case 19:
                    return "Slayer";
                case 20:
                    return "Farming";
                case 21:
                    return "Runecrafting";
                case 22:
                    return "Hunter";
                case 23:
                    return "Construction";
                case 24:
                    return "Summoning";
                case 25:
                    return "Dungeoneering";
                case 26:
                    return "Divination";
                case 27:
                    return "Invention";
                case 28:
                    return "Archaeology";
                default:
                    return "Not finished";
            }
        }

        [Command("GetRs3Item")]
        [Summary("GetItem itemname")]
        public async Task GetItemInfo([Remainder] string itemName)
        {
            string url = "https://runescape.wiki/w/" + itemName;
            WebClient webclient = new WebClient();
            string result = webclient.DownloadString(url);

            string correctedItemName = itemName.Replace(' ', '_');
            correctedItemName += "_detail.png";

            string imageUrl = "https://runescape.wiki/images/thumb/9/97/" + correctedItemName + "/100px-" + correctedItemName;

            Embedder embedder = new Embedder();
            embedder.AddImageUrl(imageUrl);
            await Context.Channel.SendMessageAsync("", false, embedder.Build());

        }
    }
}
