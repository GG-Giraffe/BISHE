using System.Collections.Generic;
using UnityEngine;

namespace TradePlay
{
    [CreateAssetMenu(menuName = "TradePlay/卡牌配置", fileName = "Card_")]
    public sealed class CardData : ScriptableObject
    {
        [SerializeField] string cardId = "card_001";
        [SerializeField] string cardName = "新卡牌";
        [SerializeField] CardRarity rarity = CardRarity.普通;
        [SerializeField] Sprite artwork;
        [SerializeField] string character = "角色";
        [SerializeField] CardType cardType = CardType.攻击;
        [SerializeField] int countdownCost = 1;
        [SerializeField] [TextArea(2, 4)] string description = string.Empty;
        [SerializeField] int level = 1;
        [SerializeField] int damage;
        [SerializeField] int shield;
        [SerializeField] List<CardEffect> effects = new List<CardEffect>();

        public string CardId => cardId;
        public string CardName => cardName;
        public CardRarity Rarity => rarity;
        public Sprite Artwork => artwork;
        public string Character => character;
        public CardType CardType => cardType;
        public int CountdownCost => countdownCost;
        public string Description => description;
        public int Level => level;
        public int Damage => damage;
        public int Shield => shield;
        public IReadOnlyList<CardEffect> Effects => effects;

        public string[] CollectBuffIds()
        {
            return CollectIds(CardEffectKind.施加BUFF, true);
        }

        public string[] CollectVfxIds()
        {
            return CollectIds(CardEffectKind.播放特效, false);
        }

        void OnValidate()
        {
            int totalDamage = 0;
            int totalShield = 0;
            if (effects != null)
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    CardEffect effect = effects[i];
                    if (effect == null)
                    {
                        continue;
                    }

                    if (effect.Kind == CardEffectKind.伤害)
                    {
                        totalDamage += effect.Value;
                    }
                    else if (effect.Kind == CardEffectKind.获得护盾)
                    {
                        totalShield += effect.Value;
                    }
                }
            }

            damage = totalDamage;
            shield = totalShield;
        }

        string[] CollectIds(CardEffectKind kind, bool useBuffId)
        {
            if (effects == null || effects.Count == 0)
            {
                return new string[0];
            }

            List<string> ids = new List<string>();
            for (int i = 0; i < effects.Count; i++)
            {
                CardEffect effect = effects[i];
                if (effect == null || effect.Kind != kind)
                {
                    continue;
                }

                string id = useBuffId ? effect.BuffId : effect.VfxId;
                if (!string.IsNullOrEmpty(id))
                {
                    ids.Add(id);
                }
            }

            return ids.ToArray();
        }
    }
}
