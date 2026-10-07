using System.Collections.Generic;
using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// Buff 配置。数值加成和触发效果都按层数相乘。
    /// 持有者回合结束时先触发「回合结束」，再减少持续回合。
    /// </summary>
    [CreateAssetMenu(menuName = "TradePlay/Buff配置", fileName = "Buff_")]
    public sealed class BuffData : ScriptableObject
    {
        [SerializeField] string buffId = "buff_001";
        [SerializeField] string buffName = "新Buff";
        [SerializeField] Sprite icon;
        [SerializeField] [TextArea(2, 4)] string description = string.Empty;
        [SerializeField] int duration = 1;
        [SerializeField] List<BuffModifier> modifiers = new List<BuffModifier>();
        [SerializeField] List<BuffTrigger> triggers = new List<BuffTrigger>();
        [SerializeField] List<CardEffect> effects = new List<CardEffect>();

        public string BuffId => buffId;
        public string BuffName => buffName;
        public Sprite Icon => icon;
        public string Description => description;
        public int Duration => Mathf.Max(1, duration);
        public IReadOnlyList<BuffModifier> Modifiers => modifiers;
        public IReadOnlyList<BuffTrigger> Triggers => triggers;
        public IReadOnlyList<CardEffect> Effects => effects;

        public bool HasTrigger(BuffTrigger trigger)
        {
            return triggers != null && triggers.Contains(trigger);
        }

        public int ModifierValue(BuffModifierKind kind, int stacks)
        {
            int total = 0;
            if (modifiers == null)
            {
                return 0;
            }

            for (int i = 0; i < modifiers.Count; i++)
            {
                BuffModifier modifier = modifiers[i];
                if (modifier != null && modifier.Kind == kind)
                {
                    total += modifier.ScaledValue(stacks);
                }
            }

            return total;
        }
    }
}
