using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 把出牌、弃牌和倒计时接到回合结束上。
    /// 交易失败只在玩家操作结束、效果结算完、倒计时仍为 0 时出现。
    /// </summary>
    public sealed class CardBattleFlow : MonoBehaviour
    {
        [SerializeField] CardZone cardZone;
        [SerializeField] CountdownState countdown;
        [SerializeField] CombatLogView log;

        public bool TradeFailed { get; private set; }

        void Awake()
        {
            if (cardZone == null)
            {
                cardZone = FindObjectOfType<CardZone>();
            }

            if (GetComponent<EffectQueue>() == null)
            {
                gameObject.AddComponent<EffectQueue>();
            }

            if (countdown == null)
            {
                countdown = FindObjectOfType<CountdownState>();
            }

            if (log == null)
            {
                log = FindObjectOfType<CombatLogView>();
            }
        }

        public bool Play(CardView view)
        {
            if (TradeFailed || cardZone == null)
            {
                return false;
            }

            TurnManager turns = FindObjectOfType<TurnManager>();
            if (turns != null && !turns.IsPlayerTurn)
            {
                return false;
            }

            return cardZone.Play(view, countdown, log);
        }

        public bool FinishPlayerTurn()
        {
            if (TradeFailed)
            {
                return true;
            }

            if (cardZone != null)
            {
                cardZone.DiscardHand();
            }

            if (countdown == null)
            {
                return false;
            }

            int cost = countdown.NextEndTurnCost;
            countdown.ApplyEndOfTurn();
            if (log != null)
            {
                log.Print("结束回合，倒计时消耗 " + cost + "，剩余 " + countdown.Remaining);
            }

            if (countdown.Remaining > 0)
            {
                return false;
            }

            TradeFailed = true;
            countdown.ShowTradeFailed();
            if (log != null)
            {
                log.Print("交易失败");
            }

            return true;
        }
    }
}
