using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 卡牌预制体上的数据和可视引用。不含战斗逻辑，只保存属性并把数据显示到对应 UI。
    /// </summary>
    [ExecuteAlways]
    public sealed class CardView : MonoBehaviour
    {
        [Header("配置")]
        [SerializeField] CardData cardData;

        [Header("数据")]
        [SerializeField] string cardId = "card_001";
        [SerializeField] CardRarity rarity = CardRarity.普通;
        [SerializeField] Sprite artwork;
        [SerializeField] string character = "角色";
        [SerializeField] CardType cardType = CardType.攻击;
        [SerializeField] string cardName = "示例卡";
        [SerializeField] int countdownCost = 2;
        [SerializeField] [TextArea(2, 4)] string description = "造成伤害并获得护盾。";
        [SerializeField] int level = 1;
        [SerializeField] int damage = 6;
        [SerializeField] int shield = 3;
        [SerializeField] string[] relatedBuffs;
        [SerializeField] string[] relatedEffects;

        [Header("可视元素（可在预制体里拖位置）")]
        [SerializeField] Image artworkImage;
        [SerializeField] Text idText;
        [SerializeField] Text rarityText;
        [SerializeField] Text characterText;
        [SerializeField] Text typeText;
        [SerializeField] Text nameText;
        [SerializeField] Text costText;
        [SerializeField] Text descriptionText;
        [SerializeField] Text levelText;
        [SerializeField] Text damageText;
        [SerializeField] Text shieldText;
        [SerializeField] Text buffText;
        [SerializeField] Text effectText;

        void Awake()
        {
            ApplyChineseFont();
            RefreshVisuals();
        }

        void OnValidate()
        {
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            ApplyCardData();
            SetText(idText, cardId);
            SetText(rarityText, rarity.ToString());
            SetText(characterText, character);
            SetText(typeText, cardType.ToString());
            SetText(nameText, cardName);
            SetText(costText, countdownCost.ToString());
            SetText(descriptionText, description);
            SetText(levelText, "Lv." + level);
            SetText(damageText, damage.ToString());
            SetText(shieldText, shield.ToString());
            SetText(buffText, Join(relatedBuffs));
            SetText(effectText, Join(relatedEffects));

            if (artworkImage != null)
            {
                artworkImage.sprite = artwork;
                artworkImage.enabled = artwork != null;
            }
        }

        void ApplyCardData()
        {
            if (cardData == null)
            {
                return;
            }

            cardId = cardData.CardId;
            rarity = cardData.Rarity;
            artwork = cardData.Artwork;
            character = cardData.Character;
            cardType = cardData.CardType;
            cardName = cardData.CardName;
            countdownCost = cardData.CountdownCost;
            description = cardData.Description;
            level = cardData.Level;
            damage = cardData.Damage;
            shield = cardData.Shield;
            relatedBuffs = cardData.CollectBuffIds();
            relatedEffects = cardData.CollectVfxIds();
        }

        void ApplyChineseFont()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial" },
                20);

            ApplyFont(idText, font);
            ApplyFont(rarityText, font);
            ApplyFont(characterText, font);
            ApplyFont(typeText, font);
            ApplyFont(nameText, font);
            ApplyFont(costText, font);
            ApplyFont(descriptionText, font);
            ApplyFont(levelText, font);
            ApplyFont(damageText, font);
            ApplyFont(shieldText, font);
            ApplyFont(buffText, font);
            ApplyFont(effectText, font);
        }

        static void SetText(Text label, string value)
        {
            if (label != null)
            {
                label.text = value ?? string.Empty;
            }
        }

        static void ApplyFont(Text label, Font font)
        {
            if (label != null && font != null)
            {
                label.font = font;
            }
        }

        static string Join(string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(" / ", values);
        }
    }
}
