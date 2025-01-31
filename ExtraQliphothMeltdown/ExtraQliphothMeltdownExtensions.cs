using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace ExtraQliphothMeltdown
{
    public static class ExtraQliphothMeltdownExtensions
    {
        public static CreatureModel GetCreatureModel(this IsolateRoom room) => room.GetField<CreatureUnit>("_targetUnit").model;

        public static int GetMaxQliphothMeltdowns(this CreatureModel creature)
        {
            ConfigManager config = ConfigManager.Instance;
            if (creature.GetMaxWorkCount() == 0) return 0;
            if (config.ToolAbnormalityAlsoOverlapping && creature.metaInfo.creatureKitType == CreatureKitType.EQUIP) return 1;
            if (!config.ToolAbnormalityAlsoOverlapping && creature.metaInfo.creatureKitType != CreatureKitType.NONE) return 1;
            return config.OverlappingQliphothMeltdowns;
        }

        public static Dictionary<Type, Dictionary<string, FieldInfo>> FieldCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        public static void SetField(this object obj, string name, object value)
        {
            Type type = obj.GetType();
            if (FieldCache.TryGetValue(type, out Dictionary<string, FieldInfo> infos))
            {
                if (infos.TryGetValue(name, out FieldInfo info))
                {
                    info.SetValue(obj, value);
                    return;
                }
            }
            else FieldCache[type] = new Dictionary<string, FieldInfo>();
            FieldCache[type][name] = type.GetField(name, AccessTools.all);
            FieldCache[type][name].SetValue(obj, value);
        }

        public static T GetField<T>(this object obj, string name)
        {
            Type type = obj.GetType();
            if (FieldCache.TryGetValue(type, out Dictionary<string, FieldInfo> infos))
            {
                if (infos.TryGetValue(name, out FieldInfo info))
                    return (T)info.GetValue(obj);
            }
            else FieldCache[type] = new Dictionary<string, FieldInfo>();
            FieldCache[type][name] = type.GetField(name, AccessTools.all);
            return (T)FieldCache[type][name].GetValue(obj);
        }

        public static void Patch(this HarmonyInstance instance, MethodBase original, MethodInfo prefix, MethodInfo postfix) =>
            instance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

        public static void Patch(this HarmonyInstance instance, Type t1, Type t2, string original, string prefix = null, string postfix = null)
        {
            instance.Patch(
                t1.GetMethod(original, AccessTools.all),
                prefix != null ? t2.GetMethod(prefix, AccessTools.all) : null,
                postfix != null ? t2.GetMethod(postfix, AccessTools.all) : null
            );
        }

        public static void Patch<T1, T2>(this HarmonyInstance instance, string original, string prefix = null, string postfix = null) =>
            instance.Patch(typeof(T1), typeof(T2), original, prefix, postfix);

        public static XmlElement AddElement(this XmlDocument document, XmlNode node, string name)
        {
            XmlElement element = document.CreateElement(name);
            node.AppendChild(element);
            return element;
        }

        public static XmlElement AddElement(this XmlDocument document, XmlNode node, string name, string text)
        {
            XmlElement element = document.CreateElement(name);
            element.InnerText = text;
            node.AppendChild(element);
            return element;
        }

        public static bool TryGet(this XmlDocument document, string xpath, out XmlNode node)
        {
            node = document.SelectSingleNode(xpath);
            return node != null;
        }
    }
}
