using System;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 卡牌回合制的回合切换中枢。
    /// 只负责：开始战斗 → 玩家回合 → 敌方回合 → 下一轮。
    /// </summary>
    public sealed class TurnManager : MonoBehaviour
    {
        [SerializeField] CombatLogView log;
        [SerializeField] PlayerTurnHandler playerHandler;
        [SerializeField] EnemyTurnHandler enemyHandler;
        [SerializeField] Button endTurnButton;

        int _roundIndex;
        TurnSide _currentSide = TurnSide.None;
        bool _turnLocked;

        public int RoundIndex => _roundIndex;
        public TurnSide CurrentSide => _currentSide;
        public bool IsPlayerTurn => _currentSide == TurnSide.Player && !_turnLocked;

        public event Action<TurnSide, int> TurnChanged;

        void Awake()
        {
            if (log == null)
            {
                log = GetComponent<CombatLogView>();
            }

            if (playerHandler == null)
            {
                playerHandler = GetComponent<PlayerTurnHandler>();
            }

            if (enemyHandler == null)
            {
                enemyHandler = GetComponent<EnemyTurnHandler>();
            }

            if (endTurnButton == null)
            {
                GameObject buttonObject = GameObject.Find("EndTurnButton");
                if (buttonObject != null)
                {
                    endTurnButton = buttonObject.GetComponent<Button>();
                }
            }

            if (endTurnButton != null)
            {
                endTurnButton.onClick.RemoveListener(EndCurrentTurn);
                endTurnButton.onClick.AddListener(EndCurrentTurn);
            }
        }

        void Start()
        {
            if (log != null)
            {
                StartBattle();
            }
        }

        public void Setup(CombatLogView combatLog, ITurnHandler player, ITurnHandler enemy, Button button)
        {
            log = combatLog;
            playerHandler = player as PlayerTurnHandler;
            enemyHandler = enemy as EnemyTurnHandler;
            endTurnButton = button;
        }

        public void StartBattle()
        {
            _roundIndex = 1;
            _turnLocked = false;
            log.Print("战斗开始");
            EnterTurn(TurnSide.Player);
        }

        public void EndCurrentTurn()
        {
            if (_currentSide == TurnSide.None || _turnLocked)
            {
                return;
            }

            _turnLocked = true;
            GetHandler(_currentSide)?.OnTurnExit(CreateContext(_currentSide));

            TurnSide next = _currentSide == TurnSide.Player ? TurnSide.Enemy : TurnSide.Player;
            if (next == TurnSide.Player)
            {
                _roundIndex++;
            }

            EnterTurn(next);
        }

        void EnterTurn(TurnSide side)
        {
            _currentSide = side;
            _turnLocked = false;
            RefreshHud();
            TurnChanged?.Invoke(_currentSide, _roundIndex);
            GetHandler(side)?.OnTurnEnter(CreateContext(side));
        }

        ITurnHandler GetHandler(TurnSide side)
        {
            switch (side)
            {
                case TurnSide.Player:
                    return playerHandler;
                case TurnSide.Enemy:
                    return enemyHandler;
                default:
                    return null;
            }
        }

        TurnContext CreateContext(TurnSide side)
        {
            return new TurnContext(this, log, _roundIndex, side);
        }

        void RefreshHud()
        {
            string sideName = _currentSide == TurnSide.Player ? "玩家回合" : "敌人回合";
            log.SetStatus($"第 {_roundIndex} 回合  |  {sideName}");

            if (endTurnButton != null)
            {
                endTurnButton.interactable = _currentSide == TurnSide.Player;
            }
        }
    }
}
