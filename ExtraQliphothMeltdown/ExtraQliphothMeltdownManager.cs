using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraQliphothMeltdown
{
    public class ExtraQliphothMeltdownManager
    {
        private static ExtraQliphothMeltdownManager _instance;
        public static ExtraQliphothMeltdownManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ExtraQliphothMeltdownManager();
                return _instance;
            }
        }

        private Dictionary<CreatureModel, List<OverloadData>> QliphothMeltdownCount;

        public ExtraQliphothMeltdownManager()
        {
            QliphothMeltdownCount = new Dictionary<CreatureModel, List<OverloadData>>();
        }

        public List<OverloadData> this[CreatureModel creature]
        {
            get => QliphothMeltdownCount.TryGetValue(creature, out List<OverloadData> count) ? count : QliphothMeltdownCount[creature] = new List<OverloadData>();
            set => QliphothMeltdownCount[creature] = value;
        }

        public void ResetCount() => QliphothMeltdownCount = new Dictionary<CreatureModel, List<OverloadData>>();

        public void RemoveAt(CreatureModel creature, int index)
        {
            List<OverloadData> lists = this[creature];
            if (lists.Count > index) lists.RemoveAt(index);
        }

        public void ActivateOverload(Dictionary<CreatureModel, int> dict, OverloadData data)
        {
            foreach (KeyValuePair<CreatureModel, int> count in dict)
            {
                CreatureModel creature = count.Key;
                int value = count.Value;
                for (int i = 0; i < value; i++) this[creature].Add(data);
                if (!creature.isOverloaded) ActivateOverload(creature);
                else SetColor(creature.Unit.room);
            }
        }

        public void ActivateOverload(CreatureModel creature)
        {
            OverloadData data = this[creature][0];
            creature.isOverloaded = true;
            creature.overloadLevel = data.Level;
            creature.overloadTimer = 0f;
            creature.overloadType = data.Type;
            int num = 0;
            num += SefiraAbilityValueInfo.tipherethOfficerAliveValues[SefiraManager.instance.GetOfficerAliveLevel(SefiraEnum.TIPERERTH1)];
            creature.currentOverloadMaxTime = data.OverloadTime + num;
            SetColor(creature.Unit.room);
        }

        public void ExplodeOverload(CreatureModel creature)
        {
            Notice.instance.Send(NoticeName.OnIsolateOverloaded, creature, creature.overloadType);
            if (creature.overloadType != 0 && creature.ProbReductionValue > 0)
                creature.ProbReductionValue = 0;
            creature.isOverloaded = false;
            int sum = 0;
            foreach (OverloadData data in this[creature])
                sum += data.Level * 5;
            EnergyModel.instance.SubEnergy(sum);
            this[creature].Clear();
            if (creature.qliphothCounter > 0) creature.SetQliphothCounter(0);
        }

        public void CancelOverload(CreatureModel creature)
        {
            Notice.instance.Send(NoticeName.OnIsolateOverloadCanceled, creature, creature.overloadType);
            creature.isOverloaded = false;
            RemoveAt(creature, 0);
            if (this[creature].Count > 0) ActivateOverload(creature);
        }

        public static void SetColor(IsolateRoom room, Color color)
        {
            foreach (Image alarm in room.overloadUI.alarms)
                alarm.color = color;
        }

        public static void SetColor(IsolateRoom room)
        {
            int max = ConfigManager.Instance.OverlappingQliphothMeltdowns;
            int count = Instance[room.GetCreatureModel()].Count;
            switch (max)
            {
                case 1:
                    SetColor(room, new Color32(252, 58, 57, byte.MaxValue));
                    break;
                case 4:
                    SetColor(room, GameStatusUI.GameStatusUI.Window.emergencyController.EmergencyColor[count - 1]);
                    break;
                default:
                    SetColor(room, Color.HSVToRGB((count - 1f) / max, 0.774f, 0.988f));
                    break;
            }
        }

        public class OverloadData
        {
            public int Level;
            public float OverloadTime;
            public OverloadType Type;

            public OverloadData(int level, float overloadTime, OverloadType type)
            {
                Level = level;
                OverloadTime = overloadTime;
                Type = type;
            }
        }
    }
}
