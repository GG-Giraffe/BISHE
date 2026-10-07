using System;

namespace TradePlay
{
    /// <summary>
    /// 客人战斗数值。伤害先扣护盾，剩余加兴趣。
    /// 未到最后阶段时，兴趣满了就进入下一段；已在最后阶段时，溢出点数单独累计。
    /// </summary>
    public sealed class GuestCombatState
    {
        public int Interest { get; private set; }
        public int StageMax { get; private set; }
        public int Stage { get; private set; }
        public int StageLimit { get; private set; }
        public int Shield { get; private set; }
        public int OverflowInterest { get; private set; }

        public bool AtFinalStage => Stage >= StageLimit;

        public event Action<int> StageChanged;
        public event Action<int> OverflowChanged;

        public GuestCombatState(int interest, int stageMax, int stage, int stageLimit, int shield)
        {
            StageMax = Math.Max(1, stageMax);
            StageLimit = Math.Max(1, stageLimit);
            Stage = Math.Max(1, Math.Min(stage, StageLimit));
            Interest = Math.Max(0, Math.Min(interest, StageMax));
            Shield = Math.Max(0, shield);
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            int absorbed = Math.Min(Shield, amount);
            Shield -= absorbed;
            int rest = amount - absorbed;
            while (rest > 0)
            {
                if (!AtFinalStage && Interest >= StageMax)
                {
                    AdvanceStage();
                    continue;
                }

                int room = StageMax - Interest;
                if (rest < room || (rest == room && AtFinalStage))
                {
                    Interest += rest;
                    rest = 0;
                    continue;
                }

                if (!AtFinalStage)
                {
                    rest -= room;
                    AdvanceStage();
                    continue;
                }

                OverflowInterest += rest - room;
                Interest = StageMax;
                rest = 0;
                OverflowChanged?.Invoke(OverflowInterest);
            }
        }

        public void ApplyHeal(int amount)
        {
        }

        public void AddShield(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Shield += amount;
        }

        void AdvanceStage()
        {
            Stage = Math.Min(StageLimit, Stage + 1);
            Interest = 0;
            StageChanged?.Invoke(Stage);
        }
    }
}
