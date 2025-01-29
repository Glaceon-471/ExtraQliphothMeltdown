using System.IO;
using System.Xml;

namespace ExtraQliphothMeltdown
{
    public class ConfigManager
    {
        private static ConfigManager _instance;
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ConfigManager();
                return _instance;
            }
        }

        public XmlDocument ConfigDocument;
        public int OverlappingQliphothMeltdowns;
        public bool SpecialQliphothMeltdownsOverlapping;

        private ConfigManager()
        {
            OverlappingQliphothMeltdowns = 4;
        }

        public void LoadConfig()
        {
            XmlDocument document;
            if (!File.Exists(Harmony_Patch.ConfigPath))
            {
                document = new XmlDocument();
                document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement root = document.AddElement(document, "ExtraQliphothMeltdownConfig");
                root.AppendChild(document.CreateComment("Number of overlapping Qliphoth Meltdowns (default = 4)"));
                document.AddElement(root, "OverlappingQliphothMeltdowns", 4.ToString());
                root.AppendChild(document.CreateComment("Special Qliphoth Meltdowns also overlap (default = False)"));
                document.AddElement(root, "SpecialQliphothMeltdownsOverlapping", false.ToString());
                document.Save(Harmony_Patch.ConfigPath);
            }

            document = new XmlDocument();
            document.LoadXml(File.ReadAllText(Harmony_Patch.ConfigPath));
            OverlappingQliphothMeltdowns = document.TryGet("ExtraQliphothMeltdownConfig/OverlappingQliphothMeltdowns", out XmlNode node1) && int.TryParse(node1.InnerText, out int result1) ? result1 : 4;
            SpecialQliphothMeltdownsOverlapping = document.TryGet("ExtraQliphothMeltdownConfig/SpecialQliphothMeltdownsOverlapping", out XmlNode node2) && bool.TryParse(node2.InnerText, out bool result2) ? result2 : false;
        }

        public void SaveConfig()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(File.ReadAllText(Harmony_Patch.ConfigPath));
            if (document.TryGet("ExtraQliphothMeltdownConfig/OverlappingQliphothMeltdowns", out XmlNode node1))
                node1.InnerText = $"{OverlappingQliphothMeltdowns}";
            if (document.TryGet("ExtraQliphothMeltdownConfig/SpecialQliphothMeltdownsOverlapping", out XmlNode node2))
                node2.InnerText = $"{SpecialQliphothMeltdownsOverlapping}";
            document.Save(Harmony_Patch.ConfigPath);
        }
    }
}
