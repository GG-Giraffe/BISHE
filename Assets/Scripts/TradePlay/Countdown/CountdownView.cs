using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 倒计时显示。底图一直显示。特效区只切换减少、增加、归零。
    /// </summary>
    [ExecuteAlways]
    public sealed class CountdownView : MonoBehaviour
    {
        [SerializeField] int remaining = 10;
        [SerializeField] Text remainingText;
        [SerializeField] Text effectText;
        [SerializeField] Text failText;
        [SerializeField] CountdownEffectState effectState = CountdownEffectState.无;

        public int Remaining => Mathf.Max(0, remaining);

        void Awake()
        {
            ApplyChineseFont();
            RefreshVisuals();
        }

        void OnValidate()
        {
            RefreshVisuals();
        }

        public void SetRemaining(int value, CountdownEffectState state)
        {
            remaining = Mathf.Max(0, value);
            effectState = state;
            RefreshVisuals();
        }

        public void ShowTradeFailed()
        {
            effectState = CountdownEffectState.归零;
            RefreshVisuals();
            if (failText != null)
            {
                failText.gameObject.SetActive(true);
                failText.text = "交易失败";
            }
        }

        void RefreshVisuals()
        {
            if (remainingText != null)
            {
                remainingText.text = Mathf.Max(0, remaining).ToString();
            }

            if (effectText != null)
            {
                effectText.text = effectState == CountdownEffectState.无 ? string.Empty : effectState.ToString();
            }

            if (failText != null && effectState != CountdownEffectState.归零)
            {
                failText.gameObject.SetActive(false);
            }
        }

        void ApplyChineseFont()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial" },
                20);
            ApplyFont(remainingText, font);
            ApplyFont(effectText, font);
            ApplyFont(failText, font);
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
