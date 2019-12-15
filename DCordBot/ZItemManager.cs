using System;
using System.Collections.Generic;
using System.Xml;

struct StringItemInfo
{
    public string attrName;
    public string attrDesc;
    public string realName;
    public string realDesc;
}

//todo: clean this up, can't do it atm b/c too many people being obnoxious

public struct ZItemInfo
{
    public string name;
    public string desc;
    public string type;
    public string slot;
    public string delay;
    public string damage;
}

namespace DCordBot
{
    class ZItemManager
    {
        public static List<StringItemInfo> itemInfo = null;
        public static List<ZItemInfo> zItemList = null;
        public static Dictionary<string,string> itemName = null;
        public static Dictionary<string, string> itemDesc = null;

        public void Load()
        {
            zItemList = new List<ZItemInfo>();
            itemName = new Dictionary<string, string>();
            itemDesc = new Dictionary<string, string>();
            itemInfo = new List<StringItemInfo>();
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true
            };
            XmlReader reader = XmlReader.Create("strings.xml", settings);
            doc.Load(reader);

            foreach (XmlNode childNode in doc.FirstChild)
            {
                if (childNode.Name == "STR")
                {
                    if (childNode.Attributes["id"].Value.Contains("NAME"))
                    {
                        itemName.Add(childNode.Attributes["id"].Value, childNode.InnerText);
                    }
                    else if (childNode.Attributes["id"].Value.Contains("DESC"))
                    {
                        itemDesc.Add(childNode.Attributes["id"].Value, childNode.InnerText);
                    }
                }
            }

            foreach (KeyValuePair<string, string> id in itemName)
            {
                StringItemInfo item = new StringItemInfo
                {
                    attrName = id.Key,
                    realName = id.Value
                };

                string descName = item.attrName.Replace("NAME", "DESC");

                foreach (KeyValuePair<string, string> descID in itemDesc)
                {
                    if (descID.Key == descName)
                    {
                        item.attrDesc = descID.Key;
                        item.realDesc = descID.Value;
                    }
                }
                itemInfo.Add(item);
            }

            doc = new XmlDocument();
            try
            {
                doc.Load("zitem.xml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            foreach (XmlNode childNode in doc.FirstChild)
            {
                if (childNode.Name == "ITEM")
                {
                    string item = childNode.Attributes["name"].Value;
                    item = item.Replace("STR:", "");
                    ZItemInfo zItem = new ZItemInfo();

                    foreach(StringItemInfo stringItemInfo in itemInfo)
                    {
                        if(stringItemInfo.attrName == item)
                        {
                            zItem.name = stringItemInfo.realName.ToLower();
                            zItem.desc = stringItemInfo.realDesc;
                        }
                    }
                    zItem.type = childNode.Attributes["type"].Value == null ? "null" : childNode.Attributes["type"].Value;
                    zItem.slot = childNode.Attributes["slot"].Value == null ? "null" : childNode.Attributes["slot"].Value;
                    zItem.damage = childNode.Attributes["damage"] == null ? "0" : childNode.Attributes["damage"].Value;
                    zItem.delay = childNode.Attributes["delay"] == null ? "0" : childNode.Attributes["delay"].Value;

                    zItemList.Add(zItem);

                    string teststr = "ItemName: " + zItem.name + "ItemDesc " + zItem.desc;
                }
            }
        }
    }
}
