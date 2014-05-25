using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace HREngine.Bots
{
    [Serializable]
    public class CardTemplate
    {
        public static string DatabasePath { get; set; }

        public string Id { get; set; }

        public int Cost { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Race { get; set; }

        public int Atk { get; set; }

        public int Health { get; set; }

        public int Durability { get; set; }

        public List<string> Mechanics { get; set; }

        static public List<CardTemplate> templateList = new List<CardTemplate>();

        public CardTemplate()
        {
            Id = String.Empty;
            Cost = 0;
            Name = String.Empty;
            Type = String.Empty;
            Race = String.Empty;
            Atk = 0;
            Health = 0;
            Mechanics = new List<string>();
        }

        public static CardTemplate LoadFromId(string id)
        {

            foreach (CardTemplate ct in templateList)
            {
                if (ct.Id == id)
                    return ct;
            }

            return null;
        }

        public static void LoadAll()
        {

            if (DatabasePath == null)
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "" + "Database.xml");

            foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
            {
                CardTemplate template = new CardTemplate();

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.Name == "Id")
                    {
                        template.Id = node.InnerText;
                    }
                    else if (node.Name == "Name")
                    {
                        template.Name = node.InnerText;
                    }
                    else if (node.Name == "Cost")
                    {
                        template.Cost = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "Type")
                    {
                        template.Type = node.InnerText;
                    }
                    else if (node.Name == "Race")
                    {
                        template.Race = node.InnerText;
                    }
                    else if (node.Name == "Atk")
                    {
                        template.Atk = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "Health")
                    {
                        template.Health = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "Durability")
                    {
                        template.Durability = int.Parse(node.InnerText);
                    }
                }
                templateList.Add(template);
            }
        }

        public override string ToString()
        {
            string ret = "";

            ret += "CardTemplate{[" + Id + "][" + Name + "][" + Cost.ToString() + "][" + Type + "]";

            if (Type == "Minion")
            {
                if (Race != String.Empty)
                {
                    ret += "[" + Race + "]";
                }
                ret += "[" + Atk.ToString() + "]";
                ret += "[" + Health.ToString() + "]";
            }
            else if (Type == "Weapon")
            {
                ret += "[" + Atk.ToString() + "]";
                ret += "[" + Durability.ToString() + "]";
            }


            ret += "}";


            return ret;
        }
    }
}
