using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraQliphothMeltdown
{
    public class IsolateOverloadPatch
    {
        public IsolateOverloadPatch(HarmonyInstance instance)
        {
            instance.Patch<IsolateOverload, IsolateOverloadPatch>(
                "Awake",
                null,
                nameof(AwakePostfix)
            );
            instance.Patch<IsolateOverload, IsolateOverloadPatch>(
                "Update",
                nameof(UpdatePrefix),
                null
            );
            instance.Patch<IsolateOverload, IsolateOverloadPatch>(
                nameof(IsolateOverload.SetTimer),
                null,
                nameof(SetTimerPostfix)
            );
        }

        public static void AwakePostfix(IsolateOverload __instance)
        {
            foreach (Image alarm in __instance.alarms)
            {
                alarm.sprite = Harmony_Patch.CustomIsolateAlarm;
                alarm.color = new Color32(252, 58, 57, byte.MaxValue);
            }
        }

        public static bool UpdatePrefix(IsolateOverload __instance)
        {
            if (__instance.GetField<bool>("_isActivated"))
            {
                float a = (0f - Mathf.Cos(__instance.GetField<float>("alarmValue"))) / 2f + 0.5f;
                foreach (Image alarm in __instance.alarms)
                {
                    Color color = alarm.color;
                    color.a = a;
                    alarm.color = color;
                }
                __instance.SetField("alarmValue", __instance.GetField<float>("alarmValue") + Time.deltaTime * 4f);
                __instance.SetField("yAdder", __instance.GetField<float>("yAdder") - Time.deltaTime);
                if (__instance.GetField<float>("yAdder") < 0f)
                    __instance.SetField("yAdder", 0f);
            }
            else
            {
                __instance.SetField("yAdder", __instance.GetField<float>("yAdder") + Time.deltaTime * 2.4f);
                if (__instance.GetField<float>("yAdder") > 1.2f)
                    __instance.SetField("yAdder", 1.2f);
            }
            Vector3 localPosition = __instance.transform.localPosition;
            localPosition.y = __instance.GetField<float>("originPositionY") - __instance.GetField<float>("yAdder");
            __instance.transform.localPosition = localPosition;
            return false;
        }

        public static void SetTimerPostfix(IsolateOverload __instance, float t, float max)
        {
            IsolateRoom room = __instance.GetComponentInParent<IsolateRoom>();
            __instance.timerText.text += $"+{ExtraQliphothMeltdownManager.Instance[room.GetCreatureModel()].Count}";
        }
    }
}
