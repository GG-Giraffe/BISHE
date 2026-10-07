using UnityEngine;
using UnityEngine.EventSystems;

namespace TradePlay
{
    /// <summary>
    /// 鼠标悬停时显示古玩名字。
    /// </summary>
    public sealed class AntiqueNameHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] AntiqueView antiqueView;

        public void Bind(AntiqueView view)
        {
            antiqueView = view;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (antiqueView != null)
            {
                antiqueView.SetNameHovered(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (antiqueView != null)
            {
                antiqueView.SetNameHovered(false);
            }
        }
    }
}
