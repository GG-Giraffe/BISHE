using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 待售古玩：立绘、名字、价格标签、特效挂点。
    /// 兴趣阶段未到上限时，涨价百分比 = 当前段数 × 每段涨价百分比。
    /// 到达阶段上限后，溢出的兴趣值每点再加 1%。
    /// </summary>
    [ExecuteAlways]
    public sealed class AntiqueView : MonoBehaviour
    {
        [Header("立绘")]
        [SerializeField] AntiquePortraitState portraitState = AntiquePortraitState.待机;
        [SerializeField] Sprite idleSprite;
        [SerializeField] Sprite priceUpSprite;
        [SerializeField] Image portraitImage;

        [Header("名字")]
        [SerializeField] string antiqueName = "示例古玩";
        [SerializeField] bool nameVisible = true;
        [SerializeField] Text nameText;

        [Header("价格")]
        [SerializeField] int basePrice = 1000;
        [SerializeField] int currentStage = 1;
        [SerializeField] int stageLimit = 3;
        [SerializeField] int percentPerStage = 1;
        [SerializeField] int overflowInterestPoints;
        [SerializeField] Text priceText;
        [SerializeField] Text markupText;

        [Header("特效区")]
        [SerializeField] string effectType = string.Empty;
        [SerializeField] Transform effectAnchor;

        bool _nameHovered;

        public int BasePrice => basePrice;
        public int StageMarkupPercent => Mathf.Max(0, currentStage) * Mathf.Max(0, percentPerStage);
        public bool AtStageLimit => stageLimit > 0 && currentStage >= stageLimit;
        public int MarkupPercent => StageMarkupPercent + (AtStageLimit ? Mathf.Max(0, overflowInterestPoints) : 0);
        public int CurrentPrice => basePrice * (100 + MarkupPercent) / 100;

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

        public void SetInterestStage(int stage, int limit)
        {
            currentStage = Mathf.Max(0, stage);
            stageLimit = Mathf.Max(1, limit);
            if (!AtStageLimit)
            {
                overflowInterestPoints = 0;
            }

            RefreshVisuals();
        }

        public void SetInterestOverflow(int pointsBeyondCap)
        {
            overflowInterestPoints = AtStageLimit ? Mathf.Max(0, pointsBeyondCap) : 0;
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            RefreshPortrait();
            RefreshName();
            RefreshPrice();
        }

        void RefreshPortrait()
        {
            if (portraitImage == null)
            {
                return;
            }

            bool priceUp = portraitState == AntiquePortraitState.涨价 || MarkupPercent > 0;
            Sprite sprite = priceUp && priceUpSprite != null ? priceUpSprite : idleSprite;
            if (sprite != null)
            {
                portraitImage.sprite = sprite;
            }
        }

        void RefreshName()
        {
            if (nameText != null)
            {
                nameText.text = antiqueName;
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

        void RefreshPrice()
        {
            if (priceText != null)
            {
                priceText.text = CurrentPrice.ToString();
            }

            if (markupText != null)
            {
                markupText.text = "+" + MarkupPercent + "%";
            }
        }

        void ApplyChineseFont()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial" },
                20);

            ApplyFont(nameText, font);
            ApplyFont(priceText, font);
            ApplyFont(markupText, font);
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
