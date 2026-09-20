using System.Collections;
using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 敌人回合：目前只打印行动文字，稍作停顿后自动结束回合。
    /// 之后把 ExecuteActions 换成真实 AI 即可。
    /// </summary>
    public class EnemyTurnHandler : MonoBehaviour, ITurnHandler
    {
        [SerializeField] float actionDuration = 1.2f;

        public TurnSide Side => TurnSide.Enemy;

        public void OnTurnEnter(TurnContext context)
        {
            context.Log.Print($"敌人回合开始（第 {context.RoundIndex} 回合）");
            StartCoroutine(RunEnemyTurn(context));
        }

        public void OnTurnExit(TurnContext context)
        {
            context.Log.Print("敌人回合结束");
        }

        IEnumerator RunEnemyTurn(TurnContext context)
        {
            ExecuteActions(context);
            yield return new WaitForSeconds(actionDuration);
            context.Manager.EndCurrentTurn();
        }

        protected virtual void ExecuteActions(TurnContext context)
        {
            // TODO: 替换为真实敌人技能/攻击选择。
            context.Log.Print("敌人行动（占位，未实现真实逻辑）");
        }
    }
}
