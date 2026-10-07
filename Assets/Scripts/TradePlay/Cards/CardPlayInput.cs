using UnityEngine;
using UnityEngine.EventSystems;

namespace TradePlay
{
    /// <summary>
    /// 按住卡牌向上拖过一段距离后打出。距离不够时回到原位。
    /// </summary>
    public sealed class CardPlayInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        const float PlayDistance = 90f;

        CardZone _zone;
        RectTransform _rect;
        Vector2 _origin;
        bool _dragging;

        public void Init(CardZone zone)
        {
            _zone = zone;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _rect = transform as RectTransform;
            if (_rect == null)
            {
                return;
            }

            _origin = _rect.anchoredPosition;
            _dragging = true;
            _rect.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging || _rect == null)
            {
                return;
            }

            float scale = 1f;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.scaleFactor > 0f)
            {
                scale = canvas.scaleFactor;
            }

            _rect.anchoredPosition += eventData.delta / scale;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging || _rect == null)
            {
                return;
            }

            _dragging = false;
            float rise = _rect.anchoredPosition.y - _origin.y;
            if (rise < PlayDistance)
            {
                _rect.anchoredPosition = _origin;
                return;
            }

            CardBattleFlow flow = FindObjectOfType<CardBattleFlow>();
            bool played = flow != null && flow.Play(GetComponent<CardView>());
            if (!played && _rect != null)
            {
                _rect.anchoredPosition = _origin;
            }
        }
    }
}
