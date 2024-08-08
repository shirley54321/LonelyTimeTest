using GameLobby.TopBar;
using Games.SelectedMachine;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SlotTemplate
{
    public class PlayerStatus : MonoBehaviour
    {
        //Event
        public event EventHandler<BalanceChangedEventArgs> BalanceChanged;
        public event EventHandler<BetRateChangedEventArgs> BetRateChanged;

        [SerializeField] int _baseBet = 25;
        public decimal BaseBet => _baseBet;

        [SerializeField] BetRateDB _availableBetRates;

        private int _hallId = ((int)MachineLobbyMediator.Instance.SelectedHall);

        #region Instance(Singleton)
        private static PlayerStatus instance;

        public static PlayerStatus Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerStatus>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject with {typeof(PlayerStatus)} does not exist in the scene, " +
                                       $"yet its method is being called.\n" +
                                       $"Please add {typeof(PlayerStatus)} to the scene.");
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }
        #endregion

        //Player's balance 
        decimal _balance;
        public decimal Balance
        {
            get { return _balance; }
            private set 
            { 
                _balance = value; 
            }
        }

        // Temporary store the raised number of player's balance when the win score number raising animation (including the win score raising of big win animation) is playing.
        // After the number raising animation ended, this value will apply to the actual player's balance.
        decimal _additionalShowedBalance = 0;
        public decimal AdditionalShowedBalance
        {
            get { return _additionalShowedBalance; }
            set { _additionalShowedBalance = value; }
        }

        int _oldValue = 0;
        public int OldMoney
        { 
            get { return _oldValue; } 
            set {  _oldValue = value; } 
        }

        bool _isInited = false;

        public void Init()
        {
            GetVirtualCoin();
            SetBetRateIndex(0);
            _isInited = true;
            MoonWolfGameStatesManager.IsbonusGame = false;
            GameStatesManager.IsbonusGame = false;
        }

        public bool TryCost(decimal cost)
        {
           if(FetchMachineListHandler._machineList.Hall.ToString()== "Beginner")
            {
               
                if (Balance < cost) { return false; }
                else { GUISubtract(cost); return true; }
            }
            else
            {
                if (Balance / 100 < cost) { return false; }
                else { GUISubtract(cost); return true; }
            }
            
        }

        public void GUISubtract(decimal bet)
        {
            
            Debug.Log("addUserExp" + (((float)bet) / 100)+ (int)bet);
            if (_hallId != 1) {
                Balance = Balance - (bet * 100);
                userLevel.addUserExp((float)bet);
            }
            else { 
                Balance = Balance - bet; 
                userLevel.addUserExp(((float)bet)/100);
            }

            OnBalanceShowedChanged();
        }

        public IEnumerator Reward(decimal rewarded)
        {
            Task.Delay(1000);
            yield return new WaitUntil(() => SlotMainBoardAnimationManager.IsAnimationFinish);

            if (_hallId != 1) rewarded = rewarded * 100;
            Balance = Balance + rewarded;

            OnBalanceShowedChanged();
            Debug.Log($"Balance Before Update Check: {Balance}, reward: {rewarded}");

            SlotMainBoardAnimationManager.IsAnimationFinish = false;
            StopCoroutine("Reward");
        }

        public void ApplyAdditionalShowedBalanceToBalance()
        {
            /*decimal added = AdditionalShowedBalance;
            _additionalShowedBalance = 0;
            Balance += added;*/
        }

        void OnBalanceShowedChanged()
        {
            if (_hallId == 1)
            {
                BalanceChanged?.Invoke(null, new BalanceChangedEventArgs { newBalance = Balance });
            }
            else
            {
                BalanceChanged?.Invoke(null, new BalanceChangedEventArgs { newBalance = Balance / 100 });
            }
        }

        #region Bet Handler 
        int _currentBetRateIndex = 1;
        public int BetRate
        {
            get
            {
                return _availableBetRates.GetHallById(_hallId, _currentBetRateIndex);
            }
        }

        public decimal Bet => BaseBet * BetRate;


        // For debug use
#if UNITY_EDITOR
        [Header("Debug Showing")]
        [SerializeField] InfoShowedInInspector _infoShowing;
#endif

        public void AdjustBetRate(int step)
        {
            SetBetRateIndex(_currentBetRateIndex + step);
        }

        void SetBetRateIndex(int index)
        {
            while (index < 0)
            {
                index += _availableBetRates.GetBetRateLength(_hallId);
            }
            _currentBetRateIndex = index % _availableBetRates.GetBetRateLength(_hallId);

            BetRateChanged?.Invoke(this, new BetRateChangedEventArgs { newBetRate = BetRate, newBet = Bet });
        }

        #endregion Bet Handler 

        #region Call Playfab player balance
        public void GetVirtualCoin()
        {
            PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(), OnSuccess, OnError);
        }

        private void OnError(PlayFabError error)
        {
            print(error.ToString());
        }

        private void OnSuccess(GetUserInventoryResult result)
        {
            int coins = result.VirtualCurrency["DC"];
            if (coins > 0 && _isInited)
            {
                Balance = coins;
                OldMoney = coins;
                OnBalanceShowedChanged();
            }
        }
        #endregion

        #region Coin Update
        public void UpdateVirtualCoin()
        {
            int _nowMoney = (int)Balance;
            int _updateMoney;

            //Check money from playfab 
            if (_nowMoney > OldMoney)
            {
                _updateMoney = _nowMoney - OldMoney;
                if (_updateMoney != 0)
                {
                    AddVCoin(_updateMoney);
                }
            }
            else
            {
                _updateMoney = OldMoney - _nowMoney;
                if (_updateMoney != 0)
                {
                    SubtractVCoin(_updateMoney);
                }
            }
            OldMoney = _nowMoney;

            Debug.Log($"After => _nowMoney: {_nowMoney}, _updateMoney: {_updateMoney}, oldMoney: {OldMoney}, Balance: {Balance}");
        }
        
        private void AddVCoin(int amount)
        {
            var req = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = "DC",
                Amount = amount
            };

            PlayFabClientAPI.AddUserVirtualCurrency(req, OnAddSuccess, OnAddError);
        }

        private void OnAddError(PlayFabError error)
        {
            Debug.Log(error.ToString());
        }

        private void OnAddSuccess(ModifyUserVirtualCurrencyResult result)
        {
            //Success
        }

        private void SubtractVCoin(int amount)
        {

            var req = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = "DC",
                Amount = amount
            };

            PlayFabClientAPI.SubtractUserVirtualCurrency(req, OnSubSuccess, OnSubError);
        }

        private void OnSubError(PlayFabError error)
        {
            Debug.Log(error.ToString());
        }

        private void OnSubSuccess(ModifyUserVirtualCurrencyResult result)
        {
            //Success
        }
        #endregion Coin Update

        #region Event Args
        public class NameChangedEventArgs : EventArgs
        {
            public string newName;
        }

        public class BalanceChangedEventArgs : EventArgs
        {
            public decimal newBalance;
        }
        public class BetRateChangedEventArgs : EventArgs
        {
            public int newBetRate;
            public decimal newBet;
        }
        public class BetRateAdjustEventArgs : EventArgs
        {
            public int step;
        }
        #endregion Event Args

        // For debug use
#if UNITY_EDITOR
        [Serializable]
        class InfoShowedInInspector
        {
            public string currentName;
            public int currentBalance;
            public int currentBet;
        }
#endif
    }
}

