using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 客人预制体：立绘、名字、意图、兴趣值、护盾、BUFF、特效锚点。
    /// 不含战斗逻辑，只保存属性和对应可视引用。
    /// </summary>
    [ExecuteAlways]
    public sealed class GuestView : MonoBehaviour
    {
        [Header("立绘")]
        [SerializeField] GuestPortraitState portraitState = GuestPortraitState.待机;
        [SerializeField] Sprite idleSprite;
        [SerializeField] Sprite attackSprite;
        [SerializeField] Sprite hitSprite;
        [SerializeField] Image portraitImage;

        [Header("名字")]
        [SerializeField] string guestName = "示例客人";
        [SerializeField] bool nameVisible = true;
        [SerializeField] Text nameText;

        [Header("意图区域")]
        [SerializeField] bool intentVisible = true;
        [SerializeField] GuestIntentKind intentKind = GuestIntentKind.攻击;
        [SerializeField] Sprite intentIcon;
        [SerializeField] int intentAttackValue = 8;
        [SerializeField] GameObject intentArea;
        [SerializeField] Image intentIconImage;
        [SerializeField] Text intentValueText;
        [SerializeField] GameObject intentPopup;

        [Header("兴趣值区域")]
        [SerializeField] bool interestVisible = true;
        [SerializeField] int interestCurrent = 30;
        [SerializeField] int interestStageMax = 50;
        [SerializeField] int interestStage = 1;
        [SerializeField] int interestStageLimit = 3;
        [SerializeField] GameObject interestArea;
        [SerializeField] Image interestBarBackground;
        [SerializeField] Image interestBarFill;
        [SerializeField] Image interestHeartBackground;
        [SerializeField] Image interestHeartFill;
        [SerializeField] Text interestValueText;
        [SerializeField] Text interestStageText;

        [Header("护盾区")]
        [SerializeField] int shieldValue = 5;
        [SerializeField] GameObject shieldArea;
        [SerializeField] Image shieldBarFill;
        [SerializeField] Image shieldIcon;
        [SerializeField] Text shieldValueText;

        [Header("BUFF区域")]
        [SerializeField] GuestBuffSlotView[] buffSlots;

        [Header("特效区")]
        [SerializeField] string effectType = string.Empty;
        [SerializeField] Transform effectAnchor;

        bool _nameHovered;

        void Awake()
        {
            ApplyChineseFont();
            RefreshVisuals();
        }

        void OnValidate()
        {
            RefreshVisuals();
        }

        public void SetNameHovered(bool hovered)
        {
            _nameHovered = hovered;
            RefreshNameVisible();
        }

        public void RefreshVisuals()
        {
            RefreshPortrait();
            RefreshName();
            RefreshIntent();
            RefreshInterest();
            RefreshShield();
            RefreshBuffs();
        }

        void RefreshPortrait()
        {
            if (portraitImage == null)
            {
                return;
            }

            Sprite sprite = idleSprite;
            if (portraitState == GuestPortraitState.攻击 && attackSprite != null)
            {
                sprite = attackSprite;
            }
            else if (portraitState == GuestPortraitState.受击 && hitSprite != null)
            {
                sprite = hitSprite;
            }

            if (sprite != null)
            {
                portraitImage.sprite = sprite;
            }
        }

        void RefreshName()
        {
            if (nameText != null)
            {
                nameText.text = guestName;
            }

            RefreshNameVisible();
        }

        void RefreshNameVisible()
        {
            if (nameText == null)
            {
                return;
            }

            bool show = nameVisible;
            if (Application.isPlaying)
            {
                show = _nameHovered;
            }

            nameText.gameObject.SetActive(show);
        }

        void RefreshIntent()
        {
            if (intentArea != null)
            {
                intentArea.SetActive(intentVisible);
            }

            if (intentIconImage != null)
            {
                intentIconImage.sprite = intentIcon;
                intentIconImage.enabled = intentIcon != null;
            }

            bool showAttackValue = intentKind == GuestIntentKind.攻击;
            if (intentValueText != null)
            {
                intentValueText.gameObject.SetActive(showAttackValue);
                intentValueText.text = intentAttackValue.ToString();
            }
        }

        void RefreshInterest()
        {
            if (interestArea != null)
            {
                interestArea.SetActive(interestVisible);
            }

            float barFill = interestStageMax <= 0 ? 0f : Mathf.Clamp01((float)interestCurrent / interestStageMax);
            SetFill(interestBarFill, barFill);

            float heartFill = interestStageLimit <= 0 ? 0f : Mathf.Clamp01((float)interestStage / interestStageLimit);
            SetFill(interestHeartFill, heartFill);

            if (interestValueText != null)
            {
                interestValueText.text = interestCurrent + "/" + interestStageMax;
            }

            if (interestStageText != null)
            {
                interestStageText.text = interestStage + "/" + interestStageLimit;
            }
        }

        void RefreshShield()
        {
            bool hasShield = shieldValue > 0;
            if (shieldArea != null)
            {
                shieldArea.SetActive(hasShield);
            }

            if (shieldIcon != null)
            {
                shieldIcon.enabled = hasShield;
            }

            SetFill(shieldBarFill, hasShield ? 1f : 0f);

            if (shieldValueText != null)
            {
                shieldValueText.text = shieldValue.ToString();
                shieldValueText.gameObject.SetActive(hasShield);
            }
        }

        void RefreshBuffs()
        {
            if (buffSlots == null)
            {
                return;
            }

            for (int i = 0; i < buffSlots.Length; i++)
            {
                if (buffSlots[i] != null)
                {
                    buffSlots[i].RefreshVisuals();
                }
            }
        }

        void ApplyChineseFont()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial" },
                20);

            ApplyFont(nameText, font);
            ApplyFont(intentValueText, font);
            ApplyFont(interestValueText, font);
            ApplyFont(interestStageText, font);
            ApplyFont(shieldValueText, font);
        }

        static void SetFill(Image image, float amount)
        {
            if (image == null)
            {
                return;
            }

            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = amount;
        }

        static void ApplyFont(Text label, Font font)
        {
            if (label != null && font != null)
            {
                label.font = font;
            }
        }
    }
}
