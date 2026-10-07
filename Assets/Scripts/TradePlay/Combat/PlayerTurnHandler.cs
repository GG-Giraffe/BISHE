using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 玩家回合：目前只打印发牌文字。
    /// 之后把 DrawCards 换成真实抽牌即可。
    /// </summary>
    public class PlayerTurnHandler : MonoBehaviour, ITurnHandler
    {
        [SerializeField] int drawCount = 5;

        public TurnSide Side => TurnSide.Player;

        public void OnTurnEnter(TurnContext context)
        {
            context.Log.Print($"玩家回合开始（第 {context.RoundIndex} 回合）");
            GuestView guest = FindObjectOfType<GuestView>();
            if (guest != null)
            {
                guest.PresentNextIntent();
            }

            CardZone zone = FindObjectOfType<CardZone>();
            if (zone != null)
            {
                int drawn = zone.Draw(drawCount);
                context.Log.Print($"给玩家发牌 x{drawn}");
            }
            else
            {
                DrawCards(context, drawCount);
            }

            context.Log.Print("点击手牌打出，点击「结束回合」弃掉剩余手牌并消耗倒计时。");
        }

        public void OnTurnExit(TurnContext context)
        {
            context.Log.Print("玩家回合结束");
        }

        protected virtual void DrawCards(TurnContext context, int count)
        {
            // TODO: 替换为真实牌库抽取与手牌 UI。
            context.Log.Print($"给玩家发牌 x{count}（占位，未实现真实发牌）");
        }
    }
}
