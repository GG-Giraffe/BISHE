using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 全场共用的剩余回合数。结束回合的扣除从 1 开始，每按一次增加 1。
    /// </summary>
    public sealed class CountdownState : MonoBehaviour
    {
        [SerializeField] CountdownView view;

        int _remaining;
        int _nextEndTurnCost = 1;

        public int Remaining => _remaining;
        public int NextEndTurnCost => _nextEndTurnCost;

        void Awake()
        {
            if (view == null)
            {
                view = FindObjectOfType<CountdownView>();
            }

            GuestView guest = FindObjectOfType<GuestView>();
            _remaining = guest != null ? guest.InitialCountdown : (view != null ? view.Remaining : 10);
            Push(CountdownEffectState.无);
            RefreshEndTurnButton();
        }

        public void Spend(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _remaining = Mathf.Max(0, _remaining - amount);
            Push(CountdownEffectState.减少);
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _remaining += amount;
            Push(CountdownEffectState.增加);
        }

        public void ApplyEndOfTurn()
        {
            int cost = _nextEndTurnCost;
            _nextEndTurnCost++;
            Spend(cost);
            RefreshEndTurnButton();
        }

        public void ShowTradeFailed()
        {
            if (view != null)
            {
                view.ShowTradeFailed();
            }
        }

        void Push(CountdownEffectState state)
        {
            if (view != null)
            {
                view.SetRemaining(_remaining, state);
            }
        }

        void RefreshEndTurnButton()
        {
            GameObject buttonObject = GameObject.Find("EndTurnButton");
            if (buttonObject == null)
            {
                return;
            }

            Text label = buttonObject.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = "结束回合 -" + _nextEndTurnCost;
            }
        }
    }
}
