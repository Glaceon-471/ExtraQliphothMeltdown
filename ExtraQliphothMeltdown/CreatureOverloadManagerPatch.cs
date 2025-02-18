using GameStatusUI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExtraQliphothMeltdown
{
    public class CreatureOverloadManagerPatch
    {
        public CreatureOverloadManagerPatch(HarmonyInstance instance)
        {
            instance.Patch<CreatureOverloadManager, CreatureOverloadManagerPatch>(
                "SetQliphothOverloadLevel",
                nameof(SetQliphothOverloadLevelPrefix),
                null
            );
            instance.Patch(
                typeof(CreatureOverloadManager).GetMethod(nameof(CreatureOverloadManager.ActivateOverload), AccessTools.all, null, new Type[] { typeof(int), typeof(OverloadType), typeof(float), typeof(bool), typeof(bool), typeof(bool), typeof(long[]) }, null),
                typeof(CreatureOverloadManagerPatch).GetMethod(nameof(ActivateOverloadPrefix), AccessTools.all),
                null
            );
        }

        public static bool SetQliphothOverloadLevelPrefix(CreatureOverloadManager __instance, int level)
        {
            __instance.SetField("qliphothOverloadLevel", level);
            Notice.instance.Send(NoticeName.OnQliphothOverloadLevelChanged, level);
            int num1 = 0;
            if (!SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E4))
            {
                int num2 = 0;
                int num3 = 0;
                CreatureModel[] creatureList = CreatureManager.instance.GetCreatureList();
                foreach (CreatureModel creatureModel in creatureList)
                {
                    if (creatureModel.sefiraOrigin != null) continue;
                    if (__instance.GetField<HashSet<SefiraEnum>>("clearedBossMissions").Contains(creatureModel.sefiraOrigin.sefiraEnum) &&
                        !ConfigManager.Instance.IgnoreCoreSuppressionsAlreadyMade)
                        continue;
                    if (creatureModel.GetMaxWorkCount() != 0) num3 += creatureModel.GetMaxQliphothMeltdowns();
                    num2++;
                }
                num1 = Mathf.Min(num3, (num2 * level + 9) / 10);
            }
            __instance.SetField("qliphothOverloadIsolateNum", num1);
            EnergyController controller = GameStatusUI.GameStatusUI.Window.energyContorller;
            controller.SetOverloadLevel(level);
            controller.SetOverloadIsolateNum(num1);
            typeof(CreatureOverloadManager).GetMethod("CheckOrdealActivate", AccessTools.all).Invoke(__instance, new object[] { level });
            controller.SetOverLoadOrdeal(__instance.GetField<OrdealBase>("_nextOrdeal"));
            return false;
        }

        public static bool ActivateOverloadPrefix(CreatureOverloadManager __instance, ref List<CreatureModel> __result, int overloadCount, OverloadType type, float overloadTime, bool ignoreWork = false, bool ignoreBossReward = false, bool ignoreDefaultOverload = false, params long[] ignoredCreatureMetaId)
        {
            if (type != OverloadType.DEFAULT && !ConfigManager.Instance.SpecialQliphothMeltdownsOverlapping) return true;
            if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E4))
            {
                __result = new List<CreatureModel>();
                return false;
            }
            ExtraQliphothMeltdownManager manager = ExtraQliphothMeltdownManager.Instance;
            List<CreatureModel> list1 = new List<CreatureModel>();
            List<long> list2 = new List<long>(ignoredCreatureMetaId);
            SefiraEnum sefira = SefiraBossManager.Instance.CurrentActivatedSefira;
            CreatureModel[] creatures = CreatureManager.instance.GetCreatureList();
            foreach (CreatureModel creature in creatures)
            {
                if (creature.sefira.sefiraEnum == sefira ||
                    creature.IsEscaped() ||
                    (!ignoreWork && creature.IsWorkingState()) ||
                    creature.GetMaxWorkCount() == 0 ||
                    (creature.metaInfo.creatureKitType == CreatureKitType.EQUIP && creature.kitEquipOwner != null) ||
                    creature.metadataId == 100024 ||
                    creature.metadataId == 300101 ||
                    creature.metadataId == 300110 ||
                    list2.Contains(creature.metadataId)
                ) continue;

                if (SefiraBossManager.Instance.IsKetherBoss())
                {
                    if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E4))
                        continue;
                }

                else if (creature.sefiraOrigin != null) continue;

                else if (!ignoreBossReward &&
                    __instance.GetField<HashSet<SefiraEnum>>("clearedBossMissions").Contains(creature.sefiraOrigin.sefiraEnum) &&
                    !ConfigManager.Instance.IgnoreCoreSuppressionsAlreadyMade &&
                    sefira != SefiraEnum.TIPERERTH1 &&
                    sefira != SefiraEnum.BINAH &&
                    sefira != SefiraEnum.CHOKHMAH
                ) continue;

                if (!creature.isOverloaded || (creature.isOverloaded && manager[creature].Count < creature.GetMaxQliphothMeltdowns()) || (creature.overloadType == OverloadType.DEFAULT && ignoreDefaultOverload))
                    list1.Add(creature);
            }
            Dictionary<CreatureModel, int> dict = new Dictionary<CreatureModel, int>();
            List<CreatureModel> list3 = new List<CreatureModel>();
            for (int j = 0; j < overloadCount; j++)
            {
                if (list1.Count == 0) break;
                int index = Random.Range(0, list1.Count);
                CreatureModel creature = list1[index];
                if (!list3.Contains(creature)) list3.Add(creature);
                if (!dict.ContainsKey(creature)) dict[creature] = 0;
                if (++dict[creature] + manager[creature].Count >= creature.GetMaxQliphothMeltdowns())
                    list1.RemoveAt(index);
            }
            manager.ActivateOverload(dict, new ExtraQliphothMeltdownManager.OverloadData(__instance.GetField<int>("qliphothOverloadLevel"), overloadTime, type));
            __result = list3;
            return false;
        }
    }
}
