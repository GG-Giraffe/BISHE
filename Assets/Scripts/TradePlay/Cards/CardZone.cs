using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 牌库、手牌、弃牌堆。打出时只结算倒计时、抽牌和弃牌，其余效果先记进日志。
    /// </summary>
    public sealed class CardZone : MonoBehaviour
    {
        [SerializeField] List<CardData> startingDeck = new List<CardData>();
        [SerializeField] RectTransform handArea;
        [SerializeField] CardView cardPrefab;
        [SerializeField] int maxHand = 10;
        [SerializeField] float cardGap = 16f;
        [SerializeField] float cardScale = 0.45f;

        readonly List<CardData> _deck = new List<CardData>();
        readonly List<CardData> _hand = new List<CardData>();
        readonly List<CardData> _discard = new List<CardData>();
        readonly List<CardView> _handViews = new List<CardView>();

        public int HandCount => _hand.Count;
        public IReadOnlyList<CardData> Hand => _hand;

        void Awake()
        {
            if (handArea == null)
            {
                handArea = transform as RectTransform;
            }

            if (cardPrefab == null)
            {
                cardPrefab = Resources.Load<CardView>("Card");
            }

            _deck.Clear();
            for (int i = 0; i < startingDeck.Count; i++)
            {
                if (startingDeck[i] != null)
                {
                    _deck.Add(startingDeck[i]);
                }
            }

            Shuffle(_deck);
        }

        public int Draw(int count)
        {
            int drawn = 0;
            for (int i = 0; i < count; i++)
            {
                if (_hand.Count >= maxHand)
                {
                    break;
                }

                if (_deck.Count == 0)
                {
                    RefillDeck();
                }

                if (_deck.Count == 0)
                {
                    break;
                }

                CardData card = _deck[0];
                _deck.RemoveAt(0);
                _hand.Add(card);
                _handViews.Add(Spawn(card));
                drawn++;
            }

            LayoutHand();
            return drawn;
        }

        public bool Play(CardView view, CountdownState countdown, CombatLogView log)
        {
            if (view == null || view.Data == null)
            {
                return false;
            }

            int index = _handViews.IndexOf(view);
            if (index < 0)
            {
                return false;
            }

            CardData card = view.Data;
            _hand.RemoveAt(index);
            _handViews.RemoveAt(index);
            _discard.Add(card);
            Destroy(view.gameObject);
            LayoutHand();

            if (countdown != null)
            {
                countdown.Spend(card.CountdownCost);
            }

            if (log != null)
            {
                log.Print("打出 " + card.CardName + "，倒计时消耗 " + card.CountdownCost);
            }

            IReadOnlyList<CardEffect> effects = card.Effects;
            EffectQueue queue = FindObjectOfType<EffectQueue>();
            if (queue != null)
            {
                queue.Run(effects, TurnSide.Player);
            }

            return true;
        }

        public void DiscardHand()
        {
            for (int i = 0; i < _hand.Count; i++)
            {
                _discard.Add(_hand[i]);
            }

            _hand.Clear();
            for (int i = 0; i < _handViews.Count; i++)
            {
                if (_handViews[i] != null)
                {
                    Destroy(_handViews[i].gameObject);
                }
            }

            _handViews.Clear();
        }

        public int DiscardFromHand(int count)
        {
            int discarded = 0;
            while (discarded < count && _hand.Count > 0)
            {
                int last = _hand.Count - 1;
                _discard.Add(_hand[last]);
                _hand.RemoveAt(last);
                CardView view = _handViews[last];
                _handViews.RemoveAt(last);
                if (view != null)
                {
                    Destroy(view.gameObject);
                }

                discarded++;
            }

            LayoutHand();
            return discarded;
        }

        void RefillDeck()
        {
            if (_discard.Count == 0)
            {
                return;
            }

            _deck.AddRange(_discard);
            _discard.Clear();
            Shuffle(_deck);
        }

        CardView Spawn(CardData card)
        {
            CardView view = Instantiate(cardPrefab, handArea);
            view.Bind(card);
            CardPlayInput input = view.gameObject.AddComponent<CardPlayInput>();
            input.Init(this);
            Image[] images = view.GetComponentsInChildren<Image>(true);
            if (images.Length > 0)
            {
                images[0].raycastTarget = true;
            }

            return view;
        }

        void LayoutHand()
        {
            float width = 120f;
            int count = _handViews.Count;
            float total = count * width + Mathf.Max(0, count - 1) * cardGap;
            float start = -total * 0.5f + width * 0.5f;
            for (int i = 0; i < count; i++)
            {
                RectTransform rect = _handViews[i].GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(start + i * (width + cardGap), 0f);
                rect.localScale = Vector3.one * cardScale;
            }
        }

        static void Shuffle(List<CardData> cards)
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int swap = Random.Range(0, i + 1);
                CardData temp = cards[i];
                cards[i] = cards[swap];
                cards[swap] = temp;
            }
        }
    }
}
