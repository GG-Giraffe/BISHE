using System;

namespace TradePlay
{
    /// <summary>
    /// 玩家战斗数值。伤害先扣护盾，剩余加压力；回复直接降低压力。
    /// </summary>
    public sealed class PlayerCombatState
    {
        public int Pressure { get; private set; }
        public int PressureMax { get; private set; }
        public int Shield { get; private set; }
        public bool IsDefeated { get; private set; }

        public event Action Defeated;

        public PlayerCombatState(int pressure, int pressureMax, int shield)
        {
            Pressure = Math.Max(0, pressure);
            PressureMax = Math.Max(1, pressureMax);
            Shield = Math.Max(0, shield);
            if (Pressure >= PressureMax)
            {
                Pressure = PressureMax;
                IsDefeated = true;
            }
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0 || IsDefeated)
            {
                return;
            }

            int absorbed = Math.Min(Shield, amount);
            Shield -= absorbed;
            int rest = amount - absorbed;
            if (rest <= 0)
            {
                return;
            }

            Pressure += rest;
            if (Pressure >= PressureMax)
            {
                Pressure = PressureMax;
                IsDefeated = true;
                Defeated?.Invoke();
            }
        }

        public void ApplyHeal(int amount)
        {
            if (amount <= 0 || IsDefeated)
            {
                return;
            }

            Pressure = Math.Max(0, Pressure - amount);
        }

        public void AddShield(int amount)
        {
            if (amount <= 0 || IsDefeated)
            {
                return;
            }

            Shield += amount;
        }
    }
}
