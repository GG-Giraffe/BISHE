using System;
using UnityEngine;

namespace TradePlay
{
    [Serializable]
    public sealed class CardEffect
    {
        [SerializeField] CardEffectKind kind = CardEffectKind.伤害;
        [SerializeField] CardEffectTarget target = CardEffectTarget.敌人;
        [SerializeField] int value = 1;
        [SerializeField] int duration;
        [SerializeField] string buffId = string.Empty;
        [SerializeField] string vfxId = string.Empty;
        [SerializeField] string note = string.Empty;

        public CardEffectKind Kind => kind;
        public CardEffectTarget Target => target;
        public int Value => value;
        public int Duration => duration;
        public string BuffId => buffId;
        public string VfxId => vfxId;
        public string Note => note;
    }
}
