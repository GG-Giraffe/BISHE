using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 玩家角色预制体：立绘、压力值、护盾、BUFF、特效锚点。
    /// 不含战斗逻辑，只保存属性和对应可视引用。
    /// </summary>
    [ExecuteAlways]
    public sealed class PlayerView : MonoBehaviour
    {
        [Header("立绘")]
        [SerializeField] PlayerPortraitState portraitState = PlayerPortraitState.待机;
        [SerializeField] Sprite idleSprite;
        [SerializeField] Sprite attackSprite;
        [SerializeField] Sprite hitSprite;
        [SerializeField] Image portraitImage;

        [Header("压力值区域")]
        [SerializeField] bool pressureVisible = true;
        [SerializeField] int pressureCurrent = 40;
        [SerializeField] int pressureMax = 100;
        [SerializeField] GameObject pressureArea;
        [SerializeField] Image pressureBarBackground;
        [SerializeField] Image pressureBarFill;
        [SerializeField] Text pressureValueText;

        [Header("护盾区")]
        [SerializeField] int shieldValue = 5;
        [SerializeField] GameObject shieldArea;
        [SerializeField] Image shieldBarFill;
        [SerializeField] Image shieldIcon;
        [SerializeField] Text shieldValueText;

        [Header("BUFF区域")]
        [SerializeField] PlayerBuffSlotView[] buffSlots;

        [Header("特效区")]
        [SerializeField] string effectType = string.Empty;
        [SerializeField] Transform effectAnchor;

        public int PressureCurrent => pressureCurrent;
        public int PressureMax => Mathf.Max(1, pressureMax);
        public int Shield => Mathf.Max(0, shieldValue);

        public void SetCombatNumbers(int pressure, int maxPressure, int shield)
        {
            pressureCurrent = pressure;
            pressureMax = Mathf.Max(1, maxPressure);
            shieldValue = Mathf.Max(0, shield);
            RefreshVisuals();
        }

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
            RefreshPortrait();
            RefreshPressure();
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
            if (portraitState == PlayerPortraitState.攻击 && attackSprite != null)
            {
                sprite = attackSprite;
            }
            else if (portraitState == PlayerPortraitState.受击 && hitSprite != null)
            {
                sprite = hitSprite;
            }

            if (sprite != null)
            {
                portraitImage.sprite = sprite;
            }
        }

        void RefreshPressure()
        {
            if (pressureArea != null)
            {
                pressureArea.SetActive(pressureVisible);
            }

            int max = Mathf.Max(1, pressureMax);
            int current = Mathf.Clamp(pressureCurrent, 0, max);
            float percent = (float)current / max;
            SetFill(pressureBarFill, percent);

            if (pressureValueText != null)
            {
                pressureValueText.text = current + "  " + Mathf.RoundToInt(percent * 100f) + "%";
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

            ApplyFont(pressureValueText, font);
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
