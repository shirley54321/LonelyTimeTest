using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class AutoplayHandler : MonoBehaviour {

        // == Events ==
        public event EventHandler<StatesController.StateChangedEventArgs> StateChanged;
        public event EventHandler<RequestedForSpinEventArgs> RequestedForSpin;
        public event EventHandler<RoundsUpdatedEventArgs> RoundsUpdated;
        public string CurrentStateName => _statesController != null ? _statesController.currentStateName : "";
        public bool IsActive => CurrentStateName == "AutoPlaying" || CurrentStateName == "Waiting";
        public bool IsFastSpin => CurrentAutoPlayOptions.fastSpin;
        public bool WillStopAndBonusOccurred => CurrentAutoPlayOptions.stopWhenBonusOccurred;

        public AutoPlayOptions CurrentAutoPlayOptions {get; protected set;} = AutoPlayOptions.defaultValue;
        public AutoplayingStatus CurrentAutoPlayingStatus {get; protected set;} = null;
        public RoundData CurrentRoundData {get; protected set;}

        //紀錄進入Bonus前自動遊玩的場數
        public int EnterBonusBeforePlayedRound;

        StatesController _statesController;
        protected StatesController statesController => _statesController;

        StatesParameters _statesParameters = new StatesParameters();
        protected StatesParameters statesParameters => _statesParameters;


        [SerializeField] AutoPlayOptionsSettings _autoPlayOptionsSettings;
        [SerializeField] float _waitingForNextRoundDurationWhenNotWon = 0.3f;
        [SerializeField] float _waitingForNextRoundDurationWhenWon = 0f;

        [Header("Parents")]
        [SerializeField] GameStatesManager _gameStatesManager;
        protected GameStatesManager gameStatesManager => _gameStatesManager;

        [Header("Siblings")]
        [SerializeField] PlayerStatus _playerStatus;
        protected PlayerStatus playerStatus => _playerStatus;

        #if UNITY_EDITOR
            [Header("Debug Showing")]
            [SerializeField] string currentStateName;
        #endif


        // MonoBehaviour Messages
        void Awake () {
            _statesController = new StatesController(this, new State[] {
                new State("StandBy", StateStandBy),
                new State("AutoPlaying", StateAutoplaying),
                new State("Waiting", StateWaiting)
            } );
            _statesController.StateChanged += (sender, args) => {
                if (StateChanged != null) {
                    StateChanged(this, args);
                }
                #if UNITY_EDITOR
                    currentStateName = _statesController.currentStateName;
                #endif
            };
            _statesController.Run("StandBy");
        }

        void OnEnable () {
            _gameStatesManager.RoundEnded += OnRoundEnded;
        }

        void OnDisable () {
            _gameStatesManager.RoundEnded -= OnRoundEnded;
        }


        // Message from GameStatesManager
        protected virtual void ReceiveRoundData (RoundData data) {}


        // == Public methods ==
        public void PrepareToStart () {
            if (!IsActive) {
                _statesParameters.isPreparingToStart = true;
            }
        }

        public void StartAutoplay (AutoPlayOptions options) {
            if (_statesController.currentStateName != "AutoPlaying") {
                _statesParameters.isStartingAutoplay = true;
                CurrentAutoPlayOptions = options;
            }
        }

        public void StopAutoPlay () {
            if (IsActive) {
                PauseAutoPlay();
                _statesParameters.isReadyToStandBy = true;
            }
           ReelController.AddiotionalReelDelay = 0f;
        }

        public void PauseAutoPlay () {
            if (CurrentStateName == "AutoPlaying") {
                _statesParameters.isPausingAutoplay = true;
            }
        }

        public void ResumeAutoPlay () {
            if (CurrentStateName == "Waiting") {
                _statesParameters.isStartingAutoplay = true;
            }
        }

        public void SetReadyToStandBy () {
            if (CurrentStateName == "Waiting") {
                _statesParameters.isReadyToStandBy = true;
            }
        }

        public bool CheckForConitune (StoppingConditionDeterminingData determiningData, bool willStopImmediately = false) {
            if (!CurrentAutoPlayingStatus.CheckIfContinue(CurrentAutoPlayOptions, determiningData)) {
                if (willStopImmediately) {
                    StopAutoPlay();
                }
                else {
                    PauseAutoPlay();
                }
                return false;
            }
            return true;
        }



        // == Private methods ==
        // --- States ---
        IEnumerator StateStandBy (StatesController attachedStatesController) {
            _statesParameters.isReadyToStandBy = false;

            while (true) {

                if (_statesParameters.isStartingAutoplay) {
                    attachedStatesController.nextStateName = "AutoPlaying";
                    break;
                }
                else if (_statesParameters.isPreparingToStart) {
                    attachedStatesController.nextStateName = "Waiting";
                    break;
                }

                yield return null;
            }

            _statesParameters.isStartingAutoplay = false;
            _statesParameters.isPreparingToStart = false;
        }

        IEnumerator StateWaiting (StatesController attachedStatesController) {
            _statesParameters.isPausingAutoplay = false;

            while (true) {

                if (_statesParameters.isStartingAutoplay) {
                    attachedStatesController.nextStateName = "AutoPlaying";
                    break;
                }
                else if (_statesParameters.isReadyToStandBy) {
                    attachedStatesController.nextStateName = "StandBy";
                    break;
                }

                yield return null;
            }

            _statesParameters.isStartingAutoplay = false;
            _statesParameters.isReadyToStandBy = false;
        }

        IEnumerator StateAutoplaying (StatesController attachedStatesController) {

            _statesParameters.isRoundEnded = false;
            CurrentAutoPlayingStatus = new AutoplayingStatus();

            //在非Bonus遊戲與進入Bonus前的遊玩場次不為0
            if (!_gameStatesManager.statesParameters.isInBonusGame && EnterBonusBeforePlayedRound != 0) 
            {
                //當前遊玩場次數量與紀錄遊玩場次一樣
                CurrentAutoPlayingStatus.playedRounds = EnterBonusBeforePlayedRound;
                EnterBonusBeforePlayedRound = 0;
            }

            //紀錄顯示在自動遊玩場次按鈕的數值是多少
            RoundsUpdated?.Invoke(this, new RoundsUpdatedEventArgs{
                targetTotalRounds = CurrentAutoPlayOptions.targetPlayRounds,
                currentPlayingRounds = CurrentAutoPlayingStatus.playedRounds
            });


            float prevRoundEndTime = -1f;
            bool hasSpinned = false;
            while (true) {

                if (hasSpinned && _statesParameters.isRoundEnded) {
                    // OnAutoPlayRoundEnded();
                    hasSpinned = false;
                }

                if (_gameStatesManager.CurrentStateName == "ReadyForSpin") {

                    // OnNewRoundIsReadyToStart();

                    if (_statesParameters.isRoundEnded) {

                        prevRoundEndTime = Time.time;
                        _statesParameters.isRoundEnded = false;
                    }


                    if (!_statesParameters.isPausingAutoplay) {
                        if (!hasSpinned) {
                            bool isWon = CurrentRoundData != null && CurrentRoundData.resultData.totalWin > 0;
                            float waitingForNextRoundDuration = isWon ? _waitingForNextRoundDurationWhenWon : _waitingForNextRoundDurationWhenNotWon;

                            if ((prevRoundEndTime == -1f || Time.time - prevRoundEndTime > waitingForNextRoundDuration)) {
                                // Spin
                                _statesParameters.isFailedToSpin = false;

                                RequestedForSpin?.Invoke(this, new RequestedForSpinEventArgs{
                                    roundTimeScale = CurrentAutoPlayOptions.fastSpin ? _autoPlayOptionsSettings.fastSpinTimeScale : 1f,
                                    willIgnoreWaitingForBonusIconEffect = CurrentAutoPlayOptions.fastSpin && _autoPlayOptionsSettings.willIgnoreWaitingForBonusIconEffectWhenFastSpin
                                });

                                if (CurrentAutoPlayOptions.fastSpin)
                                {
                                    ReelController.AddiotionalReelDelay = 0f;
                                }
                                else
                                {
                                    ReelController.AddiotionalReelDelay = 0.5f;
                                }

                                CurrentAutoPlayingStatus.playedRounds++;

                                RoundsUpdated?.Invoke(this, new RoundsUpdatedEventArgs{
                                    targetTotalRounds = CurrentAutoPlayOptions.targetPlayRounds,
                                    currentPlayingRounds = CurrentAutoPlayingStatus.playedRounds
                                });

                                hasSpinned = true;

                                // Check if continue
                                while (true) {
                                    bool isProceed = false;

                                    if (_statesParameters.isFailedToSpin) {
                                        _statesParameters.isPausingAutoplay = true;
                                        _statesParameters.isReadyToStandBy = true;
                                        isProceed = true;
                                    }
                                    else if (CurrentRoundData != null) {
                                        CheckForConitune(new StoppingConditionDeterminingData{
                                            estimatedWonRate = CurrentRoundData.WonRate,
                                            estimatedBalanceAfterRoundEnded = playerStatus.Balance + CurrentRoundData.resultData.totalWin,
                                            isTriggeredBonusGame = CurrentRoundData.resultData.IsBonusOccurred
                                        });
                                        isProceed = true;
                                    }

                                    if (isProceed) {
                                        break;
                                    }

                                    yield return null;
                                }

                            }
                        }
                    }
                }

                //此處只有在切換場景與取消自動遊玩時會觸發
                if (_statesParameters.isPausingAutoplay) {
                    attachedStatesController.nextStateName = "Waiting";
                    //狀態為"EnteringBonusGame"與遊戲模式為Base時，紀錄遊玩次數
                    if (!_gameStatesManager.statesParameters.isInBonusGame && _gameStatesManager.CurrentStateName == "EnteringBonusGame")
                    {
                        EnterBonusBeforePlayedRound = CurrentAutoPlayingStatus.playedRounds;
                    }
                    break;
                }

                yield return null;
            }

            CurrentAutoPlayingStatus = null;
        }
        // --- States_end ---



        // protected virtual void OnAutoPlayRoundEnded () {}
        // protected virtual void OnNewRoundIsReadyToStart() {}

        protected void OnAttemptToSpinButFailed () {
            _statesParameters.isFailedToSpin = true;
        }


        void OnRoundEnded (object sender, EventArgs args) {
            CurrentRoundData = null;
            _statesParameters.isRoundEnded = true;
        }




        // == Nested public structs/classes ==
        public struct AutoPlayOptions {
            public bool fastSpin;
            public int targetPlayRounds;
            public int stoppingWonRate;
            public int minBalance;
            public int maxBalance;
            public bool stopWhenBonusOccurred;

            public static AutoPlayOptions defaultValue = new AutoPlayOptions {
                fastSpin = false,
                targetPlayRounds = -1,
                stoppingWonRate = -1,
                minBalance = -1,
                maxBalance = -1,
                stopWhenBonusOccurred = false
            };



            public override string ToString () {
                string result = "AutoPlayOptions:";
                result += fastSpin ? "fastSpin, " : "";
                result += targetPlayRounds > 0 ? $"targetPlayRounds: {targetPlayRounds}, " : "";
                result += stoppingWonRate > 0 ? $"stoppingWonRate: {stoppingWonRate}, " : "";
                result += minBalance > 0 ? $"minBalance: {minBalance}, " : "";
                result += maxBalance > 0 ? $"maxBalance: {maxBalance}, " : "";
                result += stopWhenBonusOccurred ? "stopWhenBonusOccurred" : "";
                return result;
            }
        }

        public class AutoplayingStatus {
            public int playedRounds = 0;


            public bool CheckIfContinue (AutoPlayOptions options, StoppingConditionDeterminingData data) {
                if (options.targetPlayRounds != -1 && playedRounds >= options.targetPlayRounds) {
                    return false;
                }
                if (options.stoppingWonRate != -1 && data.estimatedWonRate >= options.stoppingWonRate) {
                    return false;
                }
                if (options.minBalance != -1 && data.estimatedBalanceAfterRoundEnded < options.minBalance) {
                    return false;
                }
                if (options.maxBalance != -1 && data.estimatedBalanceAfterRoundEnded > options.maxBalance) {
                    return false;
                }
                if (options.stopWhenBonusOccurred && data.isTriggeredBonusGame) {
                    return false;
                }
                return true;
            }


            public bool CheckIfContinue (AutoPlayOptions options) {
                if (options.targetPlayRounds != -1 && playedRounds >= options.targetPlayRounds) {
                    return false;
                }
                return true;
            }

            public bool CheckIfContinue (AutoPlayOptions options, decimal playerBalance) {
                if (options.minBalance != -1 && playerBalance < options.minBalance) {
                    return false;
                }
                if (options.maxBalance != -1 && playerBalance > options.maxBalance) {
                    return false;
                }
                return CheckIfContinue(options);
            }

            public bool CheckIfContinue (AutoPlayOptions options, BaseGameRoundData baseGameRoundData) {
                if (options.stoppingWonRate != -1 && baseGameRoundData.WonRate >= options.stoppingWonRate) {
                    return false;
                }
                if (options.stopWhenBonusOccurred && baseGameRoundData.resultData.IsBonusOccurred) {
                    return false;
                }
                return CheckIfContinue(options);
            }

            public bool CheckIfContinue (AutoPlayOptions options, decimal bet, decimal win) {
                if (options.stoppingWonRate != -1 && win / bet >= options.stoppingWonRate) {
                    return false;
                }
                return CheckIfContinue(options);
            }
        }

        public struct StoppingConditionDeterminingData {
            public float estimatedWonRate;
            public decimal estimatedBalanceAfterRoundEnded;
            public bool isTriggeredBonusGame;
        }


        public class AutoplayEventArgs : EventArgs {
            public AutoPlayOptions autoplayOptions;
        }

        public class RequestedForSpinEventArgs : EventArgs {
            public float roundTimeScale;
            public bool willIgnoreWaitingForBonusIconEffect;
        }

        public class RoundsUpdatedEventArgs : EventArgs {
            public int targetTotalRounds;
            public int currentPlayingRounds;

            public int RoundsRemained => targetTotalRounds - currentPlayingRounds;
        }



        // == Nested internal classes
        [Serializable]
        public class AutoPlayOptionsSettings {
            public float fastSpinTimeScale = 1.5f;
            public bool willIgnoreWaitingForBonusIconEffectWhenFastSpin = false;
        }

        protected class StatesParameters {
            public bool isPreparingToStart = false;
            public bool isStartingAutoplay = false;
            public bool isPausingAutoplay  = false;
            public bool isReadyToStandBy = false;
            public bool isRoundEnded = false;

            public bool isFailedToSpin = false;
        }

    }
}
