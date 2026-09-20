namespace TradePlay
{
    /// <summary>
    /// 回合回调时传给玩家/敌人处理器的上下文。
    /// 真正的牌组、生命值、技能等数据以后可以挂在这里。
    /// </summary>
    public sealed class TurnContext
    {
        public TurnManager Manager { get; }
        public CombatLogView Log { get; }
        public int RoundIndex { get; }
        public TurnSide Side { get; }

        public TurnContext(TurnManager manager, CombatLogView log, int roundIndex, TurnSide side)
        {
            Manager = manager;
            Log = log;
            RoundIndex = roundIndex;
            Side = side;
        }
    }
}
