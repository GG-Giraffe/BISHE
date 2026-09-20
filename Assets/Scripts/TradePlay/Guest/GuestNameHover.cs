using UnityEngine;
using UnityEngine.EventSystems;

namespace TradePlay
{
    /// <summary>
    /// 鼠标悬停时显示客人名字。
    /// </summary>
    public sealed class GuestNameHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GuestView guestView;

        public void Bind(GuestView view)
        {
            guestView = view;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (guestView != null)
            {
                guestView.SetNameHovered(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (guestView != null)
            {
                guestView.SetNameHovered(false);
            }
        }
    }
}
