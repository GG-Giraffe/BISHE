using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 玩家、古玩、客人的界面站位。拖动三个 Slot 即可改生成位置。
    /// </summary>
    public sealed class BattleLayout : MonoBehaviour
    {
        [SerializeField] RectTransform playerSlot;
        [SerializeField] RectTransform antiqueSlot;
        [SerializeField] RectTransform guestSlot;
        [SerializeField] PlayerView playerPrefab;
        [SerializeField] AntiqueView antiquePrefab;
        [SerializeField] GuestView guestPrefab;

        void Awake()
        {
            Ensure(playerPrefab, playerSlot);
            Ensure(antiquePrefab, antiqueSlot);
            Ensure(guestPrefab, guestSlot);
            HideFrames();
        }

        void Ensure(Component prefab, RectTransform slot)
        {
            if (prefab == null || slot == null)
            {
                return;
            }

            if (slot.GetComponentInChildren(prefab.GetType(), true) != null)
            {
                return;
            }

            Component instance = Instantiate(prefab, slot);
            RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        void HideFrames()
        {
            HideFrame(playerSlot);
            HideFrame(antiqueSlot);
            HideFrame(guestSlot);
        }

        static void HideFrame(RectTransform slot)
        {
            if (slot == null)
            {
                return;
            }

            Transform frame = slot.Find("SlotFrame");
            if (frame != null)
            {
                frame.gameObject.SetActive(false);
            }
        }
    }
}
