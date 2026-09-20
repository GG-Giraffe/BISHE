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
            DrawCards(context, drawCount);
            context.Log.Print("等待玩家操作，点击「结束回合」进入敌方回合。");
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
