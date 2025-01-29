using BinahBoss;
using Harmony;

namespace ExtraQliphothMeltdown
{
    public class BinahOverloadUIPatch
    {
        public BinahOverloadUIPatch(HarmonyInstance instance)
        {
            instance.Patch<BinahOverloadUI, BinahOverloadUIPatch>(
                nameof(BinahOverloadUI.SetTimer),
                null,
                nameof(SetTimerPostfix)
            );
        }

        public static void SetTimerPostfix(BinahOverloadUI __instance, float t, float max)
        {
            IsolateRoom room = __instance.GetComponentInParent<IsolateRoom>();
            CreatureModel creature = room.GetCreatureModel();
            __instance.timerText.text += $"+{ExtraQliphothMeltdownManager.Instance[creature].Count}";
        }
    }
}
