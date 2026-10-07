using System;
using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 一层 Buff 对当前这一下的加减。实际结算时再乘层数。
    /// </summary>
    [Serializable]
    public sealed class BuffModifier
    {
        [SerializeField] BuffModifierKind kind = BuffModifierKind.造成的伤害;
        [SerializeField] int valuePerStack = 1;

        public BuffModifierKind Kind => kind;
        public int ValuePerStack => valuePerStack;

        public int ScaledValue(int stacks)
        {
            return valuePerStack * Mathf.Max(1, stacks);
        }
    }
}
