using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlotTemplate
{
    public class GUIDisplayManager : MonoBehaviour
    {

        // == Events ==
        public event EventHandler<EventArgs> WinScoreRaisingEffectStarted;
        public event EventHandler<EventArgs> WinScoreRaisingEffectEnded;
        public event EventHandler<EventArgs> PlayerAdvanced;
        public event EventHandler<EventArgs> SpinButtonClicked;
        public event EventHandler<ReelsController.StopAllReelsEventArgs> StopAllReelsButtonClicked;
        public event EventHandler<PlayerStatus.BetRateAdjustEventArgs> AdjustBetRateButtonClicked;
        public event EventHandler<AutoplayHandler.AutoplayEventArgs> StartAutoplayButtonClicked;
        public event EventHandler<EventArgs> StopAutoplayButtonClicked;
        public event EventHandler<EventArgs> ExitButtonPressed;


        [SerializeField] WinScoreRaisingEffectInfo _winScoreRaisingEffectInfo;

        [Header("== Siblings ==")]
        [SerializeField] PlayerStatus _playerStatus;
        [SerializeField] BaseGameAutoplayingHandler _baseGameAutoplayingHandler;
        [SerializeField] BonusGamePlayingHandler _bonusGamePlayingHandler;
        [SerializeField] SlotMainBoard _slotMainBoard;

        [Header("== Children ==")]
        [SerializeField] TextMeshProUGUI _machineIdTextContainer;
        [SerializeField] TextMeshProUGUI _balanceTextContainer;
        [SerializeField] Text _betTextContainer;
        [SerializeField] Text _winScoreTextContainer;
        [SerializeField] GameObject _spinButtonObject = null;
        [SerializeField] GameObject _stopAllSpinningButtonObject = null;
        [SerializeField] MarqueeController _marqueeController;
        [SerializeField] OneShotAnimationBehaviour _coinsHoppingAnimationBehaviour;
        [SerializeField] TransitionsManager _transitionsManager;
        [SerializeField] GameObject _exitingMachinePanel;
        //[SerializeField] GameObject _notEnoughMoneyPanel;

        [Header("Autoplay")]
        [SerializeField] GameObject _startAutoplayButtonObject = null;
        [SerializeField] GameObject _stopAutoplayButtonObject = null;
        [SerializeField] GameObject _remainedAutoplayRoundsInfiniteSymbol = null;
        [SerializeField] Text _remainedAutoplayRoundNumberTextContainer;
        [SerializeField] AutoplayOptionsPanel _autoplayOptionsPanel;

        [Header("Bonus Game")]
        [SerializeField] GameObject _bonusGameRoundsStatusDisplay = null;
        [SerializeField] Text _bonusGameCurrentRoundNumberTextContainer;
        [SerializeField] Text _bonusGameTotalRoundsNumberTextContainer;


        bool _isInBonusGame = false;
        Coroutine _currentWinScoreNumberRaising = null;
        bool _winScoreNumberRaisingToTheEnd = false;
        public static bool _isBonusGame = false;


        void OnEnable()
        {
            if (_playerStatus != null)
            {
                //_playerStatus.NameChanged += OnPlayerNameChanged;
                _playerStatus.BalanceChanged += OnPlayerBalanceChanged;
                _playerStatus.BetRateChanged += OnPlayerBetRateChanged;
            }
            if (_baseGameAutoplayingHandler != null)
            {
                _baseGameAutoplayingHandler.StateChanged += OnAutoplayHandlerStateChanged;
                _bonusGamePlayingHandler.StateChanged += OnAutoplayHandlerStateChanged;
                _baseGameAutoplayingHandler.RoundsUpdated += OnBaseGameAutoplayRoundsUpdate;
                _bonusGamePlayingHandler.RoundsUpdated += OnBonusGameAutoplayRoundsUpdate;
            }
            if (_slotMainBoard != null)
            {
                _slotMainBoard.StateChanged += OnSlotMainBoardStateChanged;
                _slotMainBoard.TotalWonAnimtaionStarted += OnTotalWonAnimationStart;
                _slotMainBoard.WonLineAnimationStarted += OnWonLineAnimationStart;

                if (_slotMainBoard.animManager != null)
                {
                    _slotMainBoard.animManager.ClearedAllWinningAnimation += OnClearAllWinningAnimation;
                }
            }
            if (_autoplayOptionsPanel != null)
            {
                _autoplayOptionsPanel.PanelOpended += OnAutoplayOptionsPanelOpened;
                _autoplayOptionsPanel.StartAutoplayButtonClicked += OnStartAutoplayButtonClicked;
            }
        }

        void OnDisable()
        {
            if (_playerStatus != null)
            {
                //_playerStatus.NameChanged -= OnPlayerNameChanged;
                _playerStatus.BalanceChanged -= OnPlayerBalanceChanged;
                _playerStatus.BetRateChanged -= OnPlayerBetRateChanged;
            }
            if (_baseGameAutoplayingHandler != null)
            {
                _baseGameAutoplayingHandler.StateChanged -= OnAutoplayHandlerStateChanged;
                _bonusGamePlayingHandler.StateChanged -= OnAutoplayHandlerStateChanged;
                _baseGameAutoplayingHandler.RoundsUpdated -= OnBaseGameAutoplayRoundsUpdate;
                _bonusGamePlayingHandler.RoundsUpdated -= OnBonusGameAutoplayRoundsUpdate;
            }
            if (_slotMainBoard != null)
            {
                _slotMainBoard.StateChanged -= OnSlotMainBoardStateChanged;
                _slotMainBoard.TotalWonAnimtaionStarted -= OnTotalWonAnimationStart;
                _slotMainBoard.WonLineAnimationStarted -= OnWonLineAnimationStart;

                if (_slotMainBoard.animManager != null)
                {
                    _slotMainBoard.animManager.ClearedAllWinningAnimation -= OnClearAllWinningAnimation;
                }
            }
            if (_autoplayOptionsPanel != null)
            {
                _autoplayOptionsPanel.PanelOpended -= OnAutoplayOptionsPanelOpened;
                _autoplayOptionsPanel.StartAutoplayButtonClicked -= OnStartAutoplayButtonClicked;
            }
        }

        public void SetMachineIdText(ushort machineId)
        {
            if (_machineIdTextContainer != null)
            {
                _machineIdTextContainer.text = machineId.ToString();
            }
        }

        public void SwitchToBonusGameShowing(int gainedRounds)
        {
            SetBonusGameRoundsDisplay(0, gainedRounds);
        }

        public void SetPlayerNameText()
        {
            GetPlayerProfile();
        }


        public Coroutine GetTransitionRunning(TransitionsManager.TransitionType type, object? parameters = null, Func<bool> getAdvancingCondition = null)
        {
            if (_transitionsManager != null)
            {
                return StartCoroutine(_transitionsManager.TransitionRunning(type, parameters, getAdvancingCondition));
            }
            return null;
        }

        public void SetWinScoreDisplay(decimal score)
        {
            if (_winScoreTextContainer != null)
            {
                _winScoreTextContainer.text = score.ToString();
            }
        }

        public Coroutine PlayingWinScoreRaisingAnimation(decimal appended, bool fromZero = false, bool withCoinsHoppingEffect = false, Action endCallback = null, Action<float> linkElapsedTimeRateAction = null)
        {
            int start = fromZero || _winScoreTextContainer == null ? 0 : (int)decimal.Parse(_winScoreTextContainer.text);
            return PlayingWinScoreRaisingAnimation(start, start + appended, withCoinsHoppingEffect, endCallback, linkElapsedTimeRateAction);
        }

        public Coroutine PlayingWinScoreRaisingAnimation(decimal start, decimal end, bool withCoinsHoppingEffect = false, Action endCallback = null, Action<float> linkElapsedTimeRateAction = null)
        {
            if (end - start > 0)
            {
                _currentWinScoreNumberRaising = StartCoroutine(WinScoreNumberRaisingEffect(start, end, _winScoreRaisingEffectInfo, withCoinsHoppingEffect, endCallback, linkElapsedTimeRateAction));
                return _currentWinScoreNumberRaising;
            }
            return null;
        }

        public void WinScoreRaisingJumpToTheEnd()
        {
            if (_currentWinScoreNumberRaising != null)
            {
                _winScoreNumberRaisingToTheEnd = true;
            }
        }

        public bool OpenExitMachinePanel()
        {
            if (_exitingMachinePanel)
            {
                _exitingMachinePanel.SetActive(true);
                return true;
            }
            else
            {
                Debug.LogError("Exit Machine Panel is Missing!!");
                return false;
            }
        }

        public void NotEnoughMoneyAlert(bool set)
        {
            //_notEnoughMoneyPanel.SetActive(set);
        }

        void SetBonusGameRoundsDisplay(int currentRounds, int totalRounds)
        {
            if (_bonusGameCurrentRoundNumberTextContainer != null)
            {
                _bonusGameCurrentRoundNumberTextContainer.text = currentRounds.ToString();
            }
            if (_bonusGameTotalRoundsNumberTextContainer != null)
            {
                _bonusGameTotalRoundsNumberTextContainer.text = totalRounds.ToString();
            }
        }



        // OnEvents
        public void OnPlayerAdvance()
        {
            PlayerAdvanced?.Invoke(this, EventArgs.Empty);
        }

        public void OnSlotMainBoardAreaClick()
        {
            Button button = null;
            if (_spinButtonObject && _spinButtonObject.activeSelf)
            {
                button = _spinButtonObject.GetComponentInChildren<Button>();
            }
            else if (_stopAllSpinningButtonObject && _stopAllSpinningButtonObject.activeSelf)
            {
                button = _stopAllSpinningButtonObject.GetComponentInChildren<Button>();
            }

            button?.onClick.Invoke();
        }

        public void OnSpinButtonClicked()
        {
            if(!_isBonusGame || int.Parse(_bonusGameCurrentRoundNumberTextContainer.text) == 0)
            {
                SpinButtonClicked?.Invoke(this, EventArgs.Empty);
                OnPlayerAdvance();
            }
        }

        public void OnStopAllReelsButtonClicked(bool isFastSpeed = false)
        {
            StopAllReelsButtonClicked?.Invoke(this, new ReelsController.StopAllReelsEventArgs { isFastSpeed = false });
            OnPlayerAdvance();
        }

        public void OnBetRateAdjustButtonClicked(int step)
        {
            AdjustBetRateButtonClicked?.Invoke(this, new PlayerStatus.BetRateAdjustEventArgs { step = step });
        }

        public void OnStopAutoplayButtonClicked()
        {
            StopAutoplayButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public void OnExitButtonPressed()
        {
            ExitButtonPressed?.Invoke(this, EventArgs.Empty);
        }


        void OnTotalWonAnimationStart(object sender, SlotMainBoard.TotalWonAnimationStartedEventArgs args)
        {
            try
            {
                if (args.containedAnimatedIconsId[0] != 1 || _isInBonusGame || _isBonusGame)
                {
                    _marqueeController?.ShowTotalScore(args.totalWin);
                }
                else { return; }
            }
            catch { Debug.Log("args.containedAnimatedIconsId.Length:" + args.containedAnimatedIconsId.Length); }
        }

        void OnTotalWonAnimationStop(object sender, EventArgs args)
        {
            _winScoreNumberRaisingToTheEnd = true;
            _currentWinScoreNumberRaising = null;
        }

        void OnWonLineAnimationStart(object sender, SlotMainBoard.WonLineAnimationStartedEventArgs args)
        {
            _marqueeController?.ShowWonIconAmountAndWonScore(args.wonIconId, args.wonIconCount, args.winScore);
        }

        void OnClearAllWinningAnimation(object sender, EventArgs args)
        {
            if (!_isInBonusGame)
            {
                if (_winScoreTextContainer != null)
                {
                    _winScoreTextContainer.text = "0" ;
                    //RESET OnClearAllWinningAnimation "!"
                }
            }
            _marqueeController?.Hide();
        }


        void OnAutoplayOptionsPanelOpened(object sender, EventArgs args)
        {
            if (sender is AutoplayOptionsPanelTypeA)
            {
                AutoplayOptionsPanelTypeA autoplayPanel = new AutoplayOptionsPanelTypeA();

                int minBalanceValue = (int)_playerStatus.Balance + autoplayPanel.MinBalanceValueOffsetFromPlayerCurrentBalance;
                int maxBalanceValue = (int)_playerStatus.Balance + autoplayPanel.maxBalanceValueOffsetFromPlayerCurrentBalance;

                Debug.Log($"Check Min: {autoplayPanel.isSettingMin}, Check Max: {autoplayPanel.isSettingMax}");

                if (autoplayPanel.minValueTemp > 1000 || autoplayPanel.maxValueTemp < -1000 && minBalanceValue - autoplayPanel.minValueTemp > 0)
                    autoplayPanel.SetInitAutoplayOptionsMinMaxBalanceValue(autoplayPanel.minValueTemp, autoplayPanel.maxValueTemp);
                else if (autoplayPanel.isSettingMin == true)
                {
                    autoplayPanel.SetInitAutoplayOptionsMinMaxBalanceValue(autoplayPanel.minValueTemp, maxBalanceValue);
                }
                else if (autoplayPanel.isSettingMax == true)
                {
                    autoplayPanel.SetInitAutoplayOptionsMinMaxBalanceValue(minBalanceValue, autoplayPanel.maxValueTemp);
                }
                else
                {
                    autoplayPanel.SetInitAutoplayOptionsMinMaxBalanceValue(minBalanceValue, maxBalanceValue);
                }
            }
        }

        void OnStartAutoplayButtonClicked(object sender, AutoplayHandler.AutoplayEventArgs args)
        {
            if (StartAutoplayButtonClicked != null)
            {
                StartAutoplayButtonClicked(this, args);
            }
        }

        void OnAutoplayHandlerStateChanged(object sender, StatesController.StateChangedEventArgs args)
        {

            if (_bonusGamePlayingHandler != null)
            {
                _isInBonusGame = _bonusGamePlayingHandler.IsActive;
            }

            if (_startAutoplayButtonObject && _baseGameAutoplayingHandler != null)
            {
                _startAutoplayButtonObject.SetActive(!_isInBonusGame && !_baseGameAutoplayingHandler.IsActive);
            }

            if (_stopAutoplayButtonObject && _bonusGamePlayingHandler != null)
            {
                _stopAutoplayButtonObject.SetActive(!_isInBonusGame && _baseGameAutoplayingHandler.IsActive);
            }

            if (_bonusGameRoundsStatusDisplay)
            {
                _bonusGameRoundsStatusDisplay.SetActive(_isInBonusGame);
            }
        }

        void OnBaseGameAutoplayRoundsUpdate(object sender, AutoplayHandler.RoundsUpdatedEventArgs args)
        {
            if (args.targetTotalRounds < 0)
            {
                _remainedAutoplayRoundsInfiniteSymbol?.SetActive(true);
                _remainedAutoplayRoundNumberTextContainer?.gameObject.SetActive(false);
            }
            else
            {
                _remainedAutoplayRoundsInfiniteSymbol?.SetActive(false);

                if (_remainedAutoplayRoundNumberTextContainer != null)
                {
                    _remainedAutoplayRoundNumberTextContainer.gameObject.SetActive(true);
                    _remainedAutoplayRoundNumberTextContainer.text = args.RoundsRemained.ToString();
                }
            }
        }

        void OnBonusGameAutoplayRoundsUpdate(object sender, AutoplayHandler.RoundsUpdatedEventArgs args)
        {
            SetBonusGameRoundsDisplay(args.currentPlayingRounds, args.targetTotalRounds);
        }

        void OnSlotMainBoardStateChanged(object sender, StatesController.StateChangedEventArgs args)
        {
            if (args.newStateName != "Spinning")
            {
                if (_spinButtonObject)
                {
                    _spinButtonObject?.SetActive(true);
                }
                if (_stopAllSpinningButtonObject)
                {
                    _stopAllSpinningButtonObject?.SetActive(false);
                }
            }
            else
            {
                if (_spinButtonObject)
                {
                    _spinButtonObject?.SetActive(false);
                }
                if (_stopAutoplayButtonObject)
                {
                    _stopAllSpinningButtonObject?.SetActive(true);
                }
            }
        }

        void OnPlayerBalanceChanged(object sender, PlayerStatus.BalanceChangedEventArgs args)
        {
            if (_balanceTextContainer != null)
            {
                _balanceTextContainer.text = args.newBalance.ToString("N");
            }
        }

        void OnPlayerBetRateChanged(object sender, PlayerStatus.BetRateChangedEventArgs args)
        {
            if (_betTextContainer != null)
            {
                _betTextContainer.text = args.newBet.ToString();
            }
        }

        public void StartBonusWin(decimal score)
        {
            PlayingWinScoreRaisingAnimation(score, false,false);
        }

        IEnumerator WinScoreNumberRaisingEffect(decimal startNumber, decimal endNumber, WinScoreRaisingEffectInfo info, bool withCoinsHoppingEffect = false, Action endCallback = null, Action<float> linkElapsedTimeRateAction = null)
        {
            //Debug.Log($"EndNumber: {endNumber}");
            _winScoreNumberRaisingToTheEnd = false;

            WinScoreRaisingEffectStarted?.Invoke(this, EventArgs.Empty);

            float maxDuration = (float)(endNumber - startNumber) / info.minSpeed;
            float duration = Mathf.Min(info.duration, maxDuration);
            decimal deltaNumber = endNumber - startNumber;


            // Start
            if (withCoinsHoppingEffect)
            {
                _coinsHoppingAnimationBehaviour?.gameObject.SetActive(true);
            }

            float startTime = Time.time;

            float elapsedRate = 0f;
            while (elapsedRate < 1f)
            {

                yield return null;

                if (_winScoreNumberRaisingToTheEnd)
                {
                    elapsedRate = 1f;
                }
                else
                {
                    elapsedRate = (Time.time - startTime) / duration;
                }

                decimal currentAddedNumber = decimal.Floor((decimal)Mathf.Lerp(0, (float)deltaNumber, elapsedRate));

                if (_winScoreTextContainer != null)
                {
                    _winScoreTextContainer.text = (startNumber + currentAddedNumber).ToString("0") ;
                }

                linkElapsedTimeRateAction?.Invoke(elapsedRate);
            }

            // End
            if (_coinsHoppingAnimationBehaviour != null && _coinsHoppingAnimationBehaviour.gameObject.activeSelf)
            {
                _coinsHoppingAnimationBehaviour.Advance();
            }

            _winScoreNumberRaisingToTheEnd = false;
            _currentWinScoreNumberRaising = null;

            endCallback?.Invoke();

            WinScoreRaisingEffectEnded?.Invoke(this, EventArgs.Empty);
        }


        #region Retrive Player Info

        public void GetPlayerProfile()
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true
                }
            },
               result =>
               {
                   //_playerNameTextContainer.text = result.PlayerProfile.DisplayName;
               },
               error => Debug.LogError(error.GenerateErrorReport()));
        }

        #endregion Retrive Player Info 
        // == Nested Classes ==
        [Serializable]
        class WinScoreRaisingEffectInfo
        {
            public float duration = 10f;
            public float minSpeed = 1000f;
        }

        [Serializable]
        class BonusGameElements
        {
            public TextMeshProUGUI totalRoundsText;
            public TextMeshProUGUI currentRoundsText;
        }
    }
}

