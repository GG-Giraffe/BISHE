using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 客人 BUFF 位显示。不含执行逻辑，只保存图标、层数和待机/执行状态。
    /// </summary>
    [ExecuteAlways]
    public sealed class GuestBuffSlotView : MonoBehaviour
    {
        [SerializeField] GuestBuffSlotState state = GuestBuffSlotState.待机;
        [SerializeField] int sortOrder;
        [SerializeField] Sprite icon;
        [SerializeField] int stacks = 1;
        [SerializeField] Image iconImage;
        [SerializeField] Text stackText;

        public int SortOrder => sortOrder;

        void OnValidate()
        {
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            bool show = icon != null && stacks > 0;
            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = show;
                iconImage.color = state == GuestBuffSlotState.执行
                    ? new Color(1f, 0.92f, 0.55f, 1f)
                    : Color.white;
            }

            if (stackText != null)
            {
                stackText.enabled = show;
                stackText.text = stacks > 1 ? stacks.ToString() : string.Empty;
            }
        }
    }
}
