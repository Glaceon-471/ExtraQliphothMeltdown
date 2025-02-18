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
        public bool ToolAbnormalityAlsoOverlapping;
        public bool IgnoreCoreSuppressionsAlreadyMade;

        private ConfigManager()
        {
            OverlappingQliphothMeltdowns = 4;
            SpecialQliphothMeltdownsOverlapping = false;
            ToolAbnormalityAlsoOverlapping = true;
            IgnoreCoreSuppressionsAlreadyMade = false;
        }

        public void LoadConfig()
        {
            XmlDocument document = new XmlDocument();
            if (!File.Exists(Harmony_Patch.ConfigPath)) document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", null));
            else document.LoadXml(File.ReadAllText(Harmony_Patch.ConfigPath));

            if (!document.TryGet("ExtraQliphothMeltdownConfig", out XmlNode root))
                root = document.AddElement(document, "ExtraQliphothMeltdownConfig");

            if (root.TryGet("OverlappingQliphothMeltdowns", out XmlNode node1))
                OverlappingQliphothMeltdowns = int.TryParse(node1.InnerText, out int result) ? result : 4;
            else
            {
                root.AppendChild(document.CreateComment("Number of overlapping Qliphoth Meltdowns (default = 4)"));
                root.AddElement(document, "OverlappingQliphothMeltdowns", 4.ToString());
                OverlappingQliphothMeltdowns = 4;
            }

            if (root.TryGet("SpecialQliphothMeltdownsOverlapping", out XmlNode node2))
                SpecialQliphothMeltdownsOverlapping = bool.TryParse(node2.InnerText, out bool result) && result;
            else
            {
                root.AppendChild(document.CreateComment("Special Qliphoth Meltdowns also overlap (default = False)"));
                root.AddElement(document, "SpecialQliphothMeltdownsOverlapping", false.ToString());
                SpecialQliphothMeltdownsOverlapping = false;
            }

            if (root.TryGet("ToolAbnormalityAlsoOverlapping", out XmlNode node3))
                ToolAbnormalityAlsoOverlapping = !bool.TryParse(node3.InnerText, out bool result) || result;
            else
            {
                root.AppendChild(document.CreateComment("Qliphoth Meltdowns also overlap for tool abnormality other than Equipment tool abnormality (default = True)"));
                root.AddElement(document, "ToolAbnormalityAlsoOverlapping", true.ToString());
                ToolAbnormalityAlsoOverlapping = true;
            }

            if (root.TryGet("IgnoreCoreSuppressionsAlreadyMade", out XmlNode node4))
                IgnoreCoreSuppressionsAlreadyMade = bool.TryParse(node4.InnerText, out bool result) && result;
            else
            {
                root.AppendChild(document.CreateComment("Qliphoth Meltdown also occurs in the CoreSuppressions-suppressed section (default = False)"));
                root.AddElement(document, "IgnoreCoreSuppressionsAlreadyMade", false.ToString());
                IgnoreCoreSuppressionsAlreadyMade = false;
            }

            document.Save(Harmony_Patch.ConfigPath);
        }
    }
}
