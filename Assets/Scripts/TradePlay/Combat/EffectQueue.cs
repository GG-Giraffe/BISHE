using System.Collections.Generic;
using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 按顺序结算卡牌和客人攻击的效果。压力到上限后，后面的效果不再执行。
    /// </summary>
    public sealed class EffectQueue : MonoBehaviour
    {
        BattleActors _actors;
        CardZone _zone;
        CountdownState _countdown;
        CombatLogView _log;
        TurnManager _turns;

        public bool IsStopped { get; private set; }

        void Awake()
        {
            _actors = FindObjectOfType<BattleActors>();
            _zone = FindObjectOfType<CardZone>();
            _countdown = FindObjectOfType<CountdownState>();
            _log = FindObjectOfType<CombatLogView>();
            _turns = FindObjectOfType<TurnManager>();
        }

        public void Run(IReadOnlyList<CardEffect> effects, TurnSide source)
        {
            if (effects == null || IsStopped)
            {
                return;
            }

            for (int i = 0; i < effects.Count; i++)
            {
                if (IsStopped)
                {
                    return;
                }

                RunOne(effects[i], source);
                if (_actors != null && _actors.IsPlayerDefeated)
                {
                    Stop("玩家压力达到上限，战斗失败");
                    return;
                }
            }
        }

        public void RunGuestAttack()
        {
            if (IsStopped)
            {
                return;
            }

            if (_actors == null)
            {
                _actors = FindObjectOfType<BattleActors>();
            }

            GuestView view = FindObjectOfType<GuestView>();
            if (view == null || view.IntentKind != GuestIntentKind.攻击)
            {
                Print("客人没有攻击意图");
                return;
            }

            Print("客人攻击 " + view.IntentAttackValue);
            ApplyDamage(TurnSide.Enemy, CardEffectTarget.敌人, view.IntentAttackValue);
            if (_actors != null && _actors.IsPlayerDefeated)
            {
                Stop("玩家压力达到上限，战斗失败");
            }
        }

        void RunOne(CardEffect effect, TurnSide source)
        {
            if (effect == null)
            {
                return;
            }

            switch (effect.Kind)
            {
                case CardEffectKind.伤害:
                    Print("伤害 " + effect.Value);
                    ApplyDamage(source, effect.Target, effect.Value);
                    break;
                case CardEffectKind.回复:
                    Print("回复 " + effect.Value);
                    ApplyHeal(source, effect.Target, effect.Value);
                    break;
                case CardEffectKind.获得护盾:
                    Print("获得护盾 " + effect.Value);
                    ApplyShield(source, effect.Target, effect.Value);
                    break;
                case CardEffectKind.抽牌:
                    int drawn = _zone != null ? _zone.Draw(effect.Value) : 0;
                    Print("抽牌 " + drawn);
                    break;
                case CardEffectKind.弃牌:
                    int discarded = _zone != null ? _zone.DiscardFromHand(effect.Value) : 0;
                    Print("弃牌 " + discarded);
                    break;
                case CardEffectKind.增加倒计时:
                    if (_countdown != null)
                    {
                        _countdown.Add(effect.Value);
                    }

                    Print("增加倒计时 " + effect.Value);
                    break;
                default:
                    Print("待结算 " + effect.Kind + " " + effect.Value);
                    break;
            }
        }

        void ApplyDamage(TurnSide source, CardEffectTarget target, int amount)
        {
            if (_actors == null || amount <= 0)
            {
                return;
            }

            if (HitsPlayer(source, target))
            {
                _actors.DamagePlayer(amount);
            }

            if (HitsGuest(source, target))
            {
                _actors.DamageGuest(amount);
            }
        }

        void ApplyHeal(TurnSide source, CardEffectTarget target, int amount)
        {
            if (_actors == null || amount <= 0 || !HitsPlayer(source, target))
            {
                return;
            }

            _actors.HealPlayer(amount);
        }

        void ApplyShield(TurnSide source, CardEffectTarget target, int amount)
        {
            if (_actors == null || amount <= 0)
            {
                return;
            }

            if (HitsPlayer(source, target))
            {
                _actors.AddPlayerShield(amount);
            }

            if (HitsGuest(source, target))
            {
                _actors.AddGuestShield(amount);
            }
        }

        static bool HitsPlayer(TurnSide source, CardEffectTarget target)
        {
            if (target == CardEffectTarget.自身)
            {
                return source == TurnSide.Player;
            }

            return source == TurnSide.Enemy;
        }

        static bool HitsGuest(TurnSide source, CardEffectTarget target)
        {
            if (target == CardEffectTarget.自身)
            {
                return source == TurnSide.Enemy;
            }

            return source == TurnSide.Player;
        }

        void Stop(string message)
        {
            IsStopped = true;
            if (_turns != null)
            {
                _turns.EndBattle(message);
            }
            else
            {
                Print(message);
            }
        }

        void Print(string message)
        {
            if (_log != null)
            {
                _log.Print(message);
            }
        }
    }
}
