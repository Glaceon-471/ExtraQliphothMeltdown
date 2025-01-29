using Harmony;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace ExtraQliphothMeltdown
{
    public class CreatureModelPatch
    {
        public CreatureModelPatch(HarmonyInstance instance)
        {
            instance.Patch<CreatureModel, CreatureModelPatch>(
                nameof(CreatureModel.ActivateOverload),
                nameof(ActivateOverloadPrefix),
                null
            );
            instance.Patch<CreatureModel, CreatureModelPatch>(
                nameof(CreatureModel.ExplodeOverload),
                nameof(ExplodeOverloadPrefix),
                null
            );
            instance.Patch<CreatureModel, CreatureModelPatch>(
                nameof(CreatureModel.CancelOverload),
                nameof(CancelOverloadPrefix),
                null
            );
        }

        public static bool ActivateOverloadPrefix(CreatureModel __instance, int level, float iOverloadTime = 60f, OverloadType overloadType = OverloadType.DEFAULT)
        {
            ExtraQliphothMeltdownManager manager = ExtraQliphothMeltdownManager.Instance;
            if (overloadType != OverloadType.DEFAULT && !ConfigManager.Instance.SpecialQliphothMeltdownsOverlapping)
            {
                manager[__instance].Clear();
                ExtraQliphothMeltdownManager.SetColor(__instance.Unit.room, new Color32(252, 58, 57, byte.MaxValue));
                return true;
            }
            manager[__instance].Add(new ExtraQliphothMeltdownManager.OverloadData(level, iOverloadTime, overloadType));
            if (!__instance.isOverloaded) manager.ActivateOverload(__instance);
            else ExtraQliphothMeltdownManager.SetColor(__instance.Unit.room);
            if (overloadType != OverloadType.DEFAULT) __instance.Unit.room.SetOverloadAlarmColor(overloadType);
            return false;
        }

        public static bool ExplodeOverloadPrefix(CreatureModel __instance)
        {
            ExtraQliphothMeltdownManager.Instance.ExplodeOverload(__instance);
            return false;
        }

        public static bool CancelOverloadPrefix(CreatureModel __instance)
        {
            ExtraQliphothMeltdownManager.Instance.CancelOverload(__instance);
            return false;
        }
    }
}
