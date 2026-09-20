namespace TradePlay
{
    /// <summary>
    /// 某一方在回合开始/结束时要做的事。
    /// 玩家发牌、敌人 AI 都实现这个接口，方便以后替换真实逻辑。
    /// </summary>
    public interface ITurnHandler
    {
        TurnSide Side { get; }
        void OnTurnEnter(TurnContext context);
        void OnTurnExit(TurnContext context);
    }
}
