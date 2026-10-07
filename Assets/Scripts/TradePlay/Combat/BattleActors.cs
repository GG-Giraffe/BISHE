using UnityEngine;

namespace TradePlay
{
    /// <summary>
    /// 把玩家、客人、古玩的显示和战斗数值接在一起。这一步只处理数值，不做出牌。
    /// </summary>
    public sealed class BattleActors : MonoBehaviour
    {
        [SerializeField] PlayerView playerView;
        [SerializeField] GuestView guestView;
        [SerializeField] AntiqueView antiqueView;
        [SerializeField] int testAmount = 8;

        PlayerCombatState _player;
        GuestCombatState _guest;

        public PlayerCombatState Player => _player;
        public GuestCombatState Guest => _guest;
        public bool IsPlayerDefeated => _player != null && _player.IsDefeated;

        void Awake()
        {
            if (playerView == null)
            {
                playerView = FindObjectOfType<PlayerView>();
            }

            if (guestView == null)
            {
                guestView = FindObjectOfType<GuestView>();
            }

            if (antiqueView == null)
            {
                antiqueView = FindObjectOfType<AntiqueView>();
            }

            if (playerView != null)
            {
                _player = new PlayerCombatState(playerView.PressureCurrent, playerView.PressureMax, playerView.Shield);
                _player.Defeated += OnPlayerDefeated;
            }

            if (guestView != null)
            {
                _guest = new GuestCombatState(
                    guestView.InterestCurrent,
                    guestView.InterestStageMax,
                    guestView.InterestStage,
                    guestView.InterestStageLimit,
                    guestView.Shield);
                _guest.StageChanged += OnGuestStageChanged;
                _guest.OverflowChanged += OnGuestOverflowChanged;
            }

            SyncAll();
        }

        [ContextMenu("测试/伤害玩家")]
        public void TestDamagePlayer()
        {
            DamagePlayer(testAmount);
        }

        [ContextMenu("测试/伤害客人")]
        public void TestDamageGuest()
        {
            DamageGuest(testAmount);
        }

        [ContextMenu("测试/回复玩家")]
        public void TestHealPlayer()
        {
            HealPlayer(testAmount);
        }

        [ContextMenu("测试/双方获得护盾")]
        public void TestAddShield()
        {
            AddShield(testAmount);
        }

        public void DamagePlayer(int amount)
        {
            if (_player == null)
            {
                return;
            }

            _player.ApplyDamage(amount);
            SyncPlayer();
        }

        public void DamageGuest(int amount)
        {
            if (_guest == null)
            {
                return;
            }

            _guest.ApplyDamage(amount);
            SyncGuest();
        }

        public void HealPlayer(int amount)
        {
            if (_player == null)
            {
                return;
            }

            _player.ApplyHeal(amount);
            SyncPlayer();
        }

        public void AddPlayerShield(int amount)
        {
            if (_player == null || _player.IsDefeated || amount <= 0)
            {
                return;
            }

            _player.AddShield(amount);
            SyncPlayer();
        }

        public void AddGuestShield(int amount)
        {
            if (_guest == null || amount <= 0)
            {
                return;
            }

            _guest.AddShield(amount);
            SyncGuest();
        }

        public void AddShield(int amount)
        {
            if (_player != null && !_player.IsDefeated)
            {
                _player.AddShield(amount);
                SyncPlayer();
            }

            if (_guest != null)
            {
                _guest.AddShield(amount);
                SyncGuest();
            }
        }

        void OnPlayerDefeated()
        {
            Debug.Log("玩家压力达到上限，战斗失败");
        }

        void OnGuestStageChanged(int stage)
        {
            Debug.Log("客人进入兴趣阶段 " + stage);
        }

        void OnGuestOverflowChanged(int overflow)
        {
            Debug.Log("客人兴趣溢出 " + overflow + " 点，古玩额外涨价 " + overflow + "%");
        }

        void SyncAll()
        {
            SyncPlayer();
            SyncGuest();
        }

        void SyncPlayer()
        {
            if (_player == null || playerView == null)
            {
                return;
            }

            playerView.SetCombatNumbers(_player.Pressure, _player.PressureMax, _player.Shield);
        }

        void SyncGuest()
        {
            if (_guest == null)
            {
                return;
            }

            if (guestView != null)
            {
                guestView.SetCombatNumbers(
                    _guest.Interest,
                    _guest.StageMax,
                    _guest.Stage,
                    _guest.StageLimit,
                    _guest.Shield);
            }

            if (antiqueView != null)
            {
                antiqueView.SetInterestStage(_guest.Stage, _guest.StageLimit);
                antiqueView.SetInterestOverflow(_guest.OverflowInterest);
            }
        }
    }
}
