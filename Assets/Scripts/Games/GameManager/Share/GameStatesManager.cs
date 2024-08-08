using Games.SelectedMachine;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SlotTemplate
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameManager))]
    public class GameStatesManager : MonoBehaviour, IDBCollectionReceiver
    {
        // == Events ==
        public event EventHandler<StatesController.StateChangedEventArgs> StateChanged;
        public event EventHandler<RoundEndedEventArgs> RoundEnded;
        public event EventHandler<EventArgs> BigWinAnimationStarted;
        public event EventHandler<EventArgs> BigWinAnimationEnded;
        public event EventHandler<EventArgs> BonusGameTransitionInStarted;
        public event EventHandler<EventArgs> BonusGameResultShowingStarted;
        public event EventHandler<EventArgs> BonusGameResultShowingEnded;
        public event EventHandler<EventArgs> BonusGameTransitionOutStarted;

        //�s�W�i�JBonus�C������J�¹�����
        public event EventHandler<EventArgs> EnteringBonusGameStart;
        //�s�W���}Bonus�C������X�¹�����
        public event EventHandler<EventArgs> EnteringBonusGameEnd;
        //�s�WBonus�`Ĺ�������ɭ���
        public event EventHandler<EventArgs> BonusGameTotalWinsDisplay;

        // public event Action<byte[]> DataReceived;

        public static bool IsbonusGame=false;

        public bool isPlayerAbleToSpin =>

            _statesController.currentStateName == "ReadyForSpin" &&
            _baseGameAutoplayingHandler.CurrentStateName == "StandBy" &&
            _slotMainBoard.CurrentStateName == "Idle";

        public bool IsPlayerAbleToStopReels => !_statesParameters.isInBonusGame;
        public bool ReelIsStoping = false;
        public bool IsPlayerAbleToAdvance => !(_statesParameters.isInBonusGame && _statesParameters.hasBonusGameStartedPlaying);

        public string CurrentStateName => _statesController != null ? _statesController.currentStateName : "";

        public static RecieveDataResult _results = null;

        public decimal basedWin = 0;

        StatesController _statesController;
        protected StatesController statesController
        {
            get => _statesController;
            set => _statesController = value;
        }

        StatesParameters _statesParameters = new StatesParameters();
        public StatesParameters statesParameters
        {
            get => _statesParameters;
            set => _statesParameters = value;
        }


        GameManager _gameManager;
        public GameManager gameManager
        {
            get
            {
                if (_gameManager == null)
                {
                    _gameManager = GetComponentInParent<GameManager>();
                }
                return _gameManager;
            }
        }

        [SerializeField] protected float _readyForNextRoundDelayDurationWhenWin = 2f;
        [SerializeField] protected float _waitingDurationAfterBigWinAnimationWhenAutoPlay = 3f;
        [SerializeField] protected int _coinsHoppingEffectTriggeredInBonusGameWinThreshold = 10000;

        [Header("DBs")]
        [SerializeField] protected BigWinWinRateStepsDB _bigWinWinRateStepsDB;
        [SerializeField] protected BonusGameInfoDB _bonusGameInfoDB;


        [Header("Children")]
        [SerializeField] protected PlayerStatus _playerStatus;
        [SerializeField] protected BaseGameAutoplayingHandler _baseGameAutoplayingHandler;
        [SerializeField] protected BonusGamePlayingHandler _bonusGamePlayingHandler;
        [SerializeField] protected BackgroundManager _backgroundManager;
        [SerializeField] protected GUIDisplayManager _guiDisplayManager;
        [SerializeField] protected SlotMainBoard _slotMainBoard;



        protected RoundData _currentRoundData = null;

#if UNITY_EDITOR
        [Header("Debug Showing")]
        [SerializeField] string _currentStateNameShowed;
#endif


        public void OnDBsAssignedByDBCollection(GameDBManager.DBCollection dbCollection)
        {
            _bigWinWinRateStepsDB = dbCollection.bigWinWinRateStepsDB ?? _bigWinWinRateStepsDB;
            _bonusGameInfoDB = dbCollection.bonusGameInfoDB ?? _bonusGameInfoDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty(GameDBManager.DBCollection dbCollection)
        {
            _bigWinWinRateStepsDB = dbCollection.bigWinWinRateStepsDB;
            _bonusGameInfoDB = dbCollection.bonusGameInfoDB;
        }


        protected virtual void Awake()
        {
            _gameManager = GetComponent<GameManager>();


            _statesController = new StatesController(this, new State[] {
                new State("Preparing", StatePreparing),
                new State("ReadyForSpin", StateReadyForSpin),
                new State("RoundRunning", StateRoundRunning),
                new State("EnteringBonusGame", StateEnteringBonusGame),
                new State("ExitingBonusGame", StateExitingBonusGame),
                new State("GainMoreBonusRoundsTransition", StateGainMoreBonusRoundsTransition),
                new State("GainedMostBonusRoundsTransition", StateGainedMostBonusRoundsTransition)
            });
            _statesController.StateChanged += (sender, args) =>
            {
                if (StateChanged != null)
                {
                    StateChanged(this, args);
                }
#if UNITY_EDITOR
                _currentStateNameShowed = _statesController.currentStateName;
#endif
            };
            _statesController.Run("Preparing");
        }

        protected virtual void OnEnable()
        {
            gameManager.networkDataReceiver.RoundResultDataLoaded += OnRoundResultDataLoaded;

            if (_baseGameAutoplayingHandler != null)
            {
                _baseGameAutoplayingHandler.RequestedForSpin += OnAutoplaySpin;
            }

            if (_bonusGamePlayingHandler != null)
            {
                _bonusGamePlayingHandler.RequestedForSpin += OnAutoplaySpin;
            }

            if (_slotMainBoard != null)
            {
                _slotMainBoard.SpinningEnded += OnSlotMainBoardSpinningEnded;
                _slotMainBoard.TotalWonAnimationEnded += OnSlotMainBoardTotalWonAnimationEneded;
                _slotMainBoard.GetSwildAnimationEnded += OnSlotMainBoardGetSwildAnimationEnded;
            }

            if (_guiDisplayManager != null)
            {
                _guiDisplayManager.PlayerAdvanced += OnPlayerAdvance;
                _guiDisplayManager.SpinButtonClicked += OnPlayerTryToSpin;
                _guiDisplayManager.StopAllReelsButtonClicked += OnTryToStopAllSpinning;
                _guiDisplayManager.StartAutoplayButtonClicked += OnStartAutoplayButtonClicked;
                _guiDisplayManager.StopAutoplayButtonClicked += OnStopAutoplayButtonClicked;
                _guiDisplayManager.AdjustBetRateButtonClicked += OnTryToAdjustBetRate;
            }
        }

        protected virtual void OnDisable()
        {
            gameManager.networkDataReceiver.RoundResultDataLoaded -= OnRoundResultDataLoaded;

            if (_baseGameAutoplayingHandler != null)
            {
                _baseGameAutoplayingHandler.RequestedForSpin -= OnAutoplaySpin;
            }

            if (_bonusGamePlayingHandler != null)
            {
                _bonusGamePlayingHandler.RequestedForSpin -= OnAutoplaySpin;
            }

            if (_slotMainBoard != null)
            {
                _slotMainBoard.SpinningEnded -= OnSlotMainBoardSpinningEnded;
                _slotMainBoard.TotalWonAnimationEnded -= OnSlotMainBoardTotalWonAnimationEneded;
                _slotMainBoard.GetSwildAnimationEnded += OnSlotMainBoardGetSwildAnimationEnded;
            }

            if (_guiDisplayManager != null)
            {
                _guiDisplayManager.PlayerAdvanced -= OnPlayerAdvance;
                _guiDisplayManager.SpinButtonClicked -= OnPlayerTryToSpin;
                _guiDisplayManager.StopAllReelsButtonClicked -= OnTryToStopAllSpinning;
                _guiDisplayManager.StartAutoplayButtonClicked -= OnStartAutoplayButtonClicked;
                _guiDisplayManager.StopAutoplayButtonClicked -= OnStopAutoplayButtonClicked;
                _guiDisplayManager.AdjustBetRateButtonClicked -= OnTryToAdjustBetRate;
            }
        }



        protected virtual void Update()
        {

            // Debug use: press space key to spin and advance
            if (Input.GetKeyDown("space"))
            {
                OnPlayerTryToSpin(null, EventArgs.Empty);
                OnPlayerAdvance(null, null);
            }

            // if (Input.GetMouseButtonDown(0)) {
            //     _statesParameters.isGeneralAdvancingOccurred = true;
            // }


            // if (Input.touchCount > 0) {
            //     for (int i = 0 ; i < Input.touchCount ; i++) {
            //         if (Input.GetTouch(i).phase == TouchPhase.Began) {
            //             _statesParameters.isGeneralAdvancingOccurred = true;
            //             break;
            //         }
            //     }
            // }

        }

        protected virtual void LateUpdate()
        {
            // _statesParameters.isGeneralAdvancingOccurred = false;
            _statesParameters.isPlayerAdvanced = false;
        }



        public virtual void Init(ushort machineId)
        {
            _guiDisplayManager.SetMachineIdText(machineId);
            _guiDisplayManager.SetPlayerNameText();
            _statesParameters.isInited = true;
        }



        // --- Main States ---
        protected virtual IEnumerator StatePreparing(StatesController attachedStatesController)
        {

            yield return new WaitUntil(() => _statesParameters.isInited);

            attachedStatesController.nextStateName = "ReadyForSpin";
        }


        // --- Base Game States ---
        protected virtual IEnumerator StateReadyForSpin(StatesController attachedStatesController)
        {

            yield return new WaitUntil(() => _statesParameters.isStartingToSpinSlot);

            _statesParameters.isStartingToSpinSlot = false;
            attachedStatesController.nextStateName = "RoundRunning";
        }

        protected virtual IEnumerator StateRoundRunning(StatesController attachedStatesController)
        {

            _statesParameters.gainedBonusRounds = 0;

            bool isSpinSucceed;

            if (!_statesParameters.isInBonusGame)
            {
                // Base game
                isSpinSucceed = SpinBaseGame();
            }
            else
            {
                // Bonus game//
                yield return new WaitUntil(() => _statesParameters.pendingBonusGameResultData.Count > 0);

                _statesParameters.hasBonusGameStartedPlaying = true;

                RoundResultData roundResultData = _statesParameters.pendingBonusGameResultData[0];
                _statesParameters.pendingBonusGameResultData.RemoveAt(0);

                isSpinSucceed = SpinBonusGame(roundResultData);

                // Multiplier Number Display
                bool isTheMaxNumber;
                int multiplierNumber = _bonusGameInfoDB.GetMultiplierNumberByBonusRoundNumber(roundResultData.bonusGameInfo.roundNumber, out isTheMaxNumber);
                _slotMainBoard.animManager.ShowBonusGameMultiplier(multiplierNumber, isTheMaxNumber);
            }

            attachedStatesController.nextStateName = "ReadyForSpin";

            if (isSpinSucceed)
            {
                if (!_statesParameters.isInBonusGame)
                {
                    // Base game
                    _guiDisplayManager.SetWinScoreDisplay(0);
                }

                yield return new WaitUntil(() => _statesParameters.isSlotMainBoardSpinningEnded);

                // decimal roundWin = 5000;
                decimal roundWin = _currentRoundData.resultData.totalWin;

                if (!_currentRoundData.resultData.HasWon)
                {
                    _statesParameters.isSlotMainBoardTotalWonAnimationEnd = true;
                }


                Coroutine winScoreRasing = null;
                bool isDelayForReadyForNextRound = false;

                Action<float> scoreRaisingLinkElaspedTimeRateAction = null;

                if (_currentRoundData != null)
                {
                    if (!_statesParameters.isInBonusGame)
                    {
                        // Base Game
                        if (_currentRoundData.resultData.IsBonusOccurred)
                        {
                            // Bonus game triggered
                            _statesParameters.wonBonusSituationsCount = _currentRoundData.resultData.WonBonusSituationsCount;
                            _statesParameters.gainedBonusRounds = _currentRoundData.resultData.gainedBonusRounds;

                            _statesParameters.pendingRoundDataWhenBonusGameTriggered = _currentRoundData;
                            //�����ibonus�ɥu�[�Fbonus�򥻤�(�S�������v)
                            _statesParameters.accumulatedBonusWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate;
                            basedWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate; ;

                            //Parn 2024/01/30
                            Debug.Log($"GetTotalBaseWinOfWonSituation: {RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo)} /n LastBetRate: {statesParameters.lastestBetRate}");
                            
                            winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(
                                0,
                                _statesParameters.accumulatedBonusWin / (decimal)_currentRoundData.resultData.WonBonusSituationsCount
                            );

                            isDelayForReadyForNextRound = false;

                            attachedStatesController.nextStateName = "EnteringBonusGame";
                        }
                        else
                        {
                            scoreRaisingLinkElaspedTimeRateAction = elapsedTimeRate => _playerStatus.AdditionalShowedBalance = decimal.Floor((decimal)Mathf.Lerp(0, (float)roundWin, elapsedTimeRate));

                            if (CheckForBigWinAnimation(roundWin, _currentRoundData.bet))
                            {
                                // Big win animtaion occurred
                                yield return BigWinAnimationPlaying(
                                    roundWin,
                                    _currentRoundData.bet,
                                    _baseGameAutoplayingHandler.IsActive,
                                    () => _playerStatus.ApplyAdditionalShowedBalanceToBalance(),
                                    scoreRaisingLinkElaspedTimeRateAction
                                );

                            }
                            else
                            {
                                if (roundWin > 0)
                                {
                                    winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(
                                        0,
                                        roundWin,
                                        false,
                                        () => _playerStatus.ApplyAdditionalShowedBalanceToBalance(),
                                        scoreRaisingLinkElaspedTimeRateAction
                                    );
                                }

                                if (_currentRoundData.resultData.HasWon)
                                {
                                    isDelayForReadyForNextRound = true;
                                }
                            }
                            //_ = PlayerStatus.instance.Reward(roundWin);
                        }
                    }
                    else
                    {
                        // Bonus Game
                        decimal displayedWin = roundWin / _statesParameters.wonBonusSituationsCount;  // bonus_game_round_win = base_round_win * won_bonus_lines_count
                        bool withCoinsHoppingEffect = displayedWin >= _coinsHoppingEffectTriggeredInBonusGameWinThreshold;
                        winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(displayedWin,false, withCoinsHoppingEffect);

                        if (_currentRoundData.resultData.HasWon)
                        {
                            isDelayForReadyForNextRound = true;
                        }

                        if (_currentRoundData.resultData.bonusGameInfo.isFinalRound)
                        {
                            // The final round of bonus game
                            attachedStatesController.nextStateName = "ExitingBonusGame";
                        }
                        else if (_currentRoundData.resultData.IsBonusOccurred)
                        {

                            _statesParameters.gainedBonusRounds = _currentRoundData.resultData.gainedBonusRounds;

                            if (!_statesParameters.hasAchievedMaxBonusRounds)
                            {
                                _statesParameters.hasAchievedMaxBonusRounds = _currentRoundData.resultData.bonusGameInfo.totalRoundsCount + _statesParameters.gainedBonusRounds == _bonusGameInfoDB.maxBonusRounds;

                                if (_statesParameters.hasAchievedMaxBonusRounds)
                                {
                                    attachedStatesController.nextStateName = "GainedMostBonusRoundsTransition";
                                }
                                else
                                {
                                    attachedStatesController.nextStateName = "GainMoreBonusRoundsTransition";
                                }
                            }
                        }
                    }

                    if (winScoreRasing != null)
                    {
                        Coroutine waitingForAdvance = StartCoroutine(WaitingForAdvanceToDoAction(() => _statesParameters.IsAdvancing, _guiDisplayManager.WinScoreRaisingJumpToTheEnd));
                        yield return winScoreRasing;
                        if (waitingForAdvance != null)
                        {
                            StopCoroutine(waitingForAdvance);
                        }
                    }

                    yield return new WaitUntil(() => _statesParameters.isSlotMainBoardTotalWonAnimationEnd);

                    if (isDelayForReadyForNextRound)
                    {
                        yield return new WaitForSeconds(_readyForNextRoundDelayDurationWhenWin);
                    }
                }

                RoundEnded?.Invoke(this, new RoundEndedEventArgs
                {
                    isBonusRound = _currentRoundData is BonusGameRoundData,
                    isFinalBonusRound = _currentRoundData.resultData.bonusGameInfo.isFinalRound
                });
            }
            else
            {
                BroadcastMessage("OnAttemptToSpinButFailed");
                Debug.Log("Broadcast \"OnAttemptToSpinButFailed\"");
            }


            if (!_statesParameters.isInBonusGame)
            {
                _baseGameAutoplayingHandler.SetReadyToStandBy();
            }

            _statesParameters.isSlotMainBoardSpinningEnded = false;
            _statesParameters.isSlotMainBoardTotalWonAnimationEnd = false;
            _currentRoundData = null;
        }

       
        protected virtual IEnumerator StateEnteringBonusGame(StatesController attachedStatesController)
        {
            _statesParameters.hasBonusGameStartedPlaying = false;

            if (_baseGameAutoplayingHandler.IsActive)
            {
                _baseGameAutoplayingHandler.PauseAutoPlay();
            }

            Debug.Log($"_statesParameters.pendingBonusGameResultData : {_statesParameters.pendingBonusGameResultData.Count}");
            Debug.Log($"_statesParameters.pendingBonusGameResultData : {GameCach.Instance.NormalResult_WU.Count}");
            yield return new WaitUntil(() => _statesParameters.pendingBonusGameResultData.Count >= 0);

            int gainedBonusRounds=10;
            //_statesParameters.pendingBonusGameResultData.Clear();
            if (_statesParameters.pendingBonusGameResultData.Count > 0)
            {
                gainedBonusRounds = _statesParameters.pendingBonusGameResultData[0].bonusGameInfo.totalRoundsCount;
                MoonWolfGameStatesManager.IsbonusGame = true;
                GUIDisplayManager._isBonusGame = true;
            }
            else
            {
                Debug.Log($"NOBonusGameResult : ");

                NetworkDataReceiver.NoResultBonusRoundsLeft = 13;
                GameStatesManager.IsbonusGame = true;

                gainedBonusRounds = 10;
                GameCach.Instance.GainMoreResult(0, (int)MachineLobbyMediator.Instance.SelectedGameId, NetworkDataReceiver.NoResultBonusRoundsLeft-1);
                //NetworkDataReceiver networkDataReceiver = new NetworkDataReceiver();
                yield return new WaitUntil(() => GameCach.Instance.NormalResult_WU.Count > 0);
                ConnectionScript.Instance.BonusPlay(gainedBonusRounds);

               
            }

            yield return WaitForEnteringBonusGameTransition(_baseGameAutoplayingHandler.IsActive && !_baseGameAutoplayingHandler.WillStopAndBonusOccurred, gainedBonusRounds);

            AutoplayHandler.AutoPlayOptions options = BonusGamePlayingHandler.GetAutoPlayOptions(gainedBonusRounds);
            _bonusGamePlayingHandler.StartAutoplay(options);

            _slotMainBoard.animManager.ClearAllWinningAnimations();

            //Add based score(bonus icon win when start rolling free game)
            _guiDisplayManager.SetWinScoreDisplay(basedWin);

            attachedStatesController.nextStateName = "ReadyForSpin";
        }

        protected virtual IEnumerator StateExitingBonusGame(StatesController attachedStatesController)
        {
            Debug.Log("Exiting Bonus Game");
            GUIDisplayManager._isBonusGame = false;
            MoonWolfGameStatesManager.IsbonusGame=false;
            GameStatesManager.IsbonusGame = false;

            _statesParameters.hasAchievedMaxBonusRounds = false;
            _slotMainBoard.animManager.HideBonusGameMultiplier();

            yield return WaitForExitingBonusGameTransition(_baseGameAutoplayingHandler.IsActive);

            decimal totalBonusWin = _statesParameters.accumulatedBonusWin;

            _ = StartCoroutine(PlayerStatus.Instance.Reward(totalBonusWin));
            _guiDisplayManager.SetWinScoreDisplay(totalBonusWin);


            RoundResultData pendingBaseRoundResultData = _statesParameters.pendingRoundDataWhenBonusGameTriggered.resultData;
            decimal totalRoundWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(pendingBaseRoundResultData.WonNonBonusSituationsInfo) * pendingBaseRoundResultData.BetRate;

            if (_baseGameAutoplayingHandler.IsActive)
            {
                bool isContinue = _baseGameAutoplayingHandler.CheckForConitune(
                    new AutoplayHandler.StoppingConditionDeterminingData
                    {
                        estimatedWonRate = (float)(totalRoundWin / _statesParameters.lastestBet),
                        estimatedBalanceAfterRoundEnded = _playerStatus.Balance + totalRoundWin,
                        isTriggeredBonusGame = false
                    },
                    true
                );
                yield return new WaitUntil(() => _baseGameAutoplayingHandler.IsActive == isContinue);  // Wait for _baseGameAutoplayingHandler to change state
            }


            Action<float> scoreRaisingLinkElaspedTimeRateAction = elapsedTimeRate => _playerStatus.AdditionalShowedBalance = decimal.Floor((decimal)Mathf.Lerp(0, (float)totalRoundWin, elapsedTimeRate));


            //�NtotalBonusWin�אּtotalRoundWin�Y�i(��]totalBonusWin��Bonus�`�����tBase��Ĺ��)
            if (CheckForBigWinAnimation(totalRoundWin, _statesParameters.lastestBet))
            {

                _guiDisplayManager.SetWinScoreDisplay(0);

                yield return BigWinAnimationPlaying(
                    totalRoundWin,
                    _statesParameters.lastestBet,
                    _baseGameAutoplayingHandler.IsActive,
                    () => _playerStatus.ApplyAdditionalShowedBalanceToBalance(),
                    scoreRaisingLinkElaspedTimeRateAction
                );

                _guiDisplayManager.SetWinScoreDisplay(totalRoundWin);
                Debug.Log($"Total Win At Game State Manager: {totalRoundWin}");
            }
            else
            {
                yield return _guiDisplayManager.PlayingWinScoreRaisingAnimation(
                    0,
                    totalRoundWin,
                    false,
                    () => _playerStatus.ApplyAdditionalShowedBalanceToBalance(),
                    scoreRaisingLinkElaspedTimeRateAction
                );
            }
            _statesParameters.wonBonusSituationsCount = 0;
            _statesParameters.accumulatedBonusWin = 0;
            _statesParameters.accumulatedSwildMultiplier = 1;
            attachedStatesController.nextStateName = "ReadyForSpin";
        }

        protected virtual IEnumerator StateGainMoreBonusRoundsTransition(StatesController attachedStatesController)
        {
            int gainedRounds = _statesParameters.gainedBonusRounds;
            if (gainedRounds > 0)
            {
                yield return WaitForGainMoreBonusRoundsTransition(_baseGameAutoplayingHandler.IsActive, gainedRounds);
            }

            attachedStatesController.nextStateName = "GainedMostBonusRoundsTransition";
        }

        protected virtual IEnumerator StateGainedMostBonusRoundsTransition(StatesController attachedStatesController)
        {
            if (_statesParameters.hasAchievedMaxBonusRounds)
            {
                yield return StartCoroutine(WaitForGainedMostBonusRoundsTransition(_baseGameAutoplayingHandler.IsActive));
            }
            attachedStatesController.nextStateName = "ReadyForSpin";
        }
        // --- States_end ---


        protected virtual IEnumerator BigWinAnimationPlaying(decimal targetScore, decimal bet, bool isAutoPlay = false, Action reachedTargetScoreCallback = null, Action<float> linkElapsedTimeRateAction = null)
        {

            BigWinAnimationStarted?.Invoke(this, EventArgs.Empty);

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.BigWinAnimation,
                new BigWinAnimationBehaviour.Parameters
                {
                    targetScore = targetScore,
                    bet = bet,
                    isAutoPlay = isAutoPlay,
                    reachedTargetScoreCallback = reachedTargetScoreCallback,
                    linkElapsedTimeRateAction = linkElapsedTimeRateAction
                },
                /*new BigWinTransitionControl.Parameters
                {
                    targetScore = targetScore,
                    bet = bet,
                    isAutoPlay = isAutoPlay,
                    reachedTargetScoreCallback = reachedTargetScoreCallback,
                    linkElapsedTimeRateAction = linkElapsedTimeRateAction
                },*/
                () => _statesParameters.IsAdvancing
            );

            BigWinAnimationEnded?.Invoke(this, EventArgs.Empty);
            //�⤽���쥻�b//Big win animtaion occurred ����o��
            _guiDisplayManager.SetWinScoreDisplay(targetScore);
            Debug.Log($"Total Win At Game State Manager target score: {targetScore}");

            //Waiting after big win animation when base game autoplaying
            float waitingStartTime = Time.time;
            while (_baseGameAutoplayingHandler.IsActive)
            {
                if (Time.time - waitingStartTime > _waitingDurationAfterBigWinAnimationWhenAutoPlay)
                {
                    break;
                }
                yield return null;
            }

        }

        protected virtual IEnumerator WaitForEnteringBonusGameTransition(bool isAutoPlay, int gainedRounds)
        {
            BonusGameTransitionInStarted?.Invoke(this, EventArgs.Empty);
            //�s�W�\��?
            EnteringBonusGameStart?.Invoke(this, EventArgs.Empty);

            // transition in
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeOut);

            _backgroundManager.ChangeBackground("BonusGame");
            _slotMainBoard.Reset(ReelController.GameModeType.BonusGame);
            _guiDisplayManager.SwitchToBonusGameShowing(gainedRounds);
            _bonusGamePlayingHandler.PrepareToStart();

            _statesParameters.isInBonusGame = true;

            // transition out
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeIn);


            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.BonusGameTriggering,
                new BonusGameTriggeringAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    gainedRounds = gainedRounds
                }
            );

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.ClickToStartBonusGameShowing,
                new SimpleWaitingForAdvanceBehaviour.Parameters
                {
                    autoAdvance = isAutoPlay
                },
                () => _statesParameters.IsAdvancing
            );
        }

        protected virtual IEnumerator WaitForExitingBonusGameTransition(bool isAutoPlay)
        {

            BonusGameResultShowingStarted?.Invoke(this, EventArgs.Empty);
            //�s�W�\��?
            BonusGameTotalWinsDisplay?.Invoke(this, EventArgs.Empty);

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.BonusGameResultShowing,
                new BonusGameResultShowingAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    bonusWinPerBonusLine = _statesParameters.BonusWinPerWonBonusLine,
                    bonusLineCount = _statesParameters.wonBonusSituationsCount
                }
            );

            BonusGameResultShowingEnded?.Invoke(this, EventArgs.Empty);

            //�s�W�\��?
            EnteringBonusGameEnd?.Invoke(this, EventArgs.Empty);

            BonusGameTransitionOutStarted?.Invoke(this, EventArgs.Empty);



            // transition fade in
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeOut);

            _backgroundManager.ChangeBackground("BaseGame");
            _slotMainBoard.RestoreBaseGameRoundFromPending(_statesParameters.BonusWinPerWonBonusLine);
            _baseGameAutoplayingHandler.ResumeAutoPlay();
            _bonusGamePlayingHandler.StopAutoPlay();

            _statesParameters.isInBonusGame = false;

            // transition fade out
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeIn);
        }

        protected virtual IEnumerator WaitForGainMoreBonusRoundsTransition(bool isAutoPlay, int gainedRounds)
        {

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.GainMoreBonusRounds,
                new GainMoreBonusRoundsAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    gainedRounds = gainedRounds
                }
            );

        }

        protected virtual IEnumerator WaitForGainedMostBonusRoundsTransition(bool isAutoPlay)
        {

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.GainedMaxBonusRounds,
                new GainedMostBonusRoundsAnimationBehaviour.Parameters
                {
                    autoAdvance = true
                }
            );

        }


        protected virtual IEnumerator WaitingForAdvanceToDoAction(Func<bool> getAdvancingCondition, Action action)
        {
            while (!getAdvancingCondition())
            {
                yield return null;
            }
            action?.Invoke();
        }


        protected virtual bool SpinBaseGame()
        {

            decimal bet = _playerStatus.Bet;
            decimal betRate = _playerStatus.BetRate;
            //Parnnnn
            if (_playerStatus.TryCost(bet))
            {
                _statesParameters.lastestBet = bet;
                _statesParameters.lastestBetRate = betRate;

                _slotMainBoard.Spin(_statesParameters.roundTimeScale, _statesParameters.willIgnoreWaitingForBonusIconEffect);
                _statesParameters.roundTimeScale = 1f;
                gameManager.networkMessageSender.SendSpinSlotMessage(bet);
                //_guiDisplayManager.NotEnoughMoneyAlert(false);
                return true;
            }
            else
            {
                //_guiDisplayManager.NotEnoughMoneyAlert(true);
                Debug.Log("Not enough balance.");
                return false;
            }
        }

        protected virtual bool SpinBonusGame(RoundResultData resultData)
        {
            //Parn Debug
            _currentRoundData = new BonusGameRoundData(_playerStatus.Balance, _statesParameters.lastestBet, _statesParameters.lastestBetRate, resultData, _statesParameters.accumulatedBonusWin, _statesParameters.wonBonusSituationsCount);

            _slotMainBoard.Spin(_statesParameters.roundTimeScale, _statesParameters.willIgnoreWaitingForBonusIconEffect);
            _statesParameters.roundTimeScale = 1f;

            _bonusGamePlayingHandler.AddTargetTotalRounds(_currentRoundData.resultData.gainedBonusRounds);

            // Add total data 
            _statesParameters.accumulatedBonusWin += resultData.totalWin;

            BroadcastMessage("ReceiveRoundData", _currentRoundData);
            return true;
        }

        protected virtual void TryToSpin(bool isFromAutoplay = false, float overrideTimeScale = 1f, bool willIgnoreWaitingForBonusIconEffect = false)
        {

            bool isSpinnable = false;
            ReelIsStoping=false;
            if (!isFromAutoplay)
            {
                if (isPlayerAbleToSpin)
                {
                    _statesParameters.isFromAutoplay = false;

                    isSpinnable = true;
                }
            }
            else
            {
                _statesParameters.isFromAutoplay = true;

                isSpinnable = true;
            }

            if (isSpinnable)
            {
                _statesParameters.isStartingToSpinSlot = true;
                _statesParameters.roundTimeScale = overrideTimeScale;
                // _statesParameters.isReadyForNextRound = false;
                _statesParameters.willIgnoreWaitingForBonusIconEffect = willIgnoreWaitingForBonusIconEffect;
            }
        }

        protected virtual bool CheckForBigWinAnimation(decimal targetScore, decimal bet)
        {
            BigWinType bigWinType = _bigWinWinRateStepsDB.GetTargetType((float)(targetScore / bet));
            return bigWinType != BigWinType.None;
        }


        // OnEvents
        protected virtual void OnPlayerAdvance(object sender, EventArgs args)
        {
            if (IsPlayerAbleToAdvance)
            {
                _statesParameters.isPlayerAdvanced = true;
            }
        }

        protected virtual void OnPlayerTryToSpin(object sender, EventArgs args)
        {
            TryToSpin();
        }

        protected virtual void OnAutoplaySpin(object sender, AutoplayHandler.RequestedForSpinEventArgs args)
        {
            TryToSpin(true, args.roundTimeScale, args.willIgnoreWaitingForBonusIconEffect);
        }

        protected virtual void OnTryToStopAllSpinning(object sender, ReelsController.StopAllReelsEventArgs args)
        {
            if (IsPlayerAbleToStopReels&&!ReelIsStoping)
            {
                ReelIsStoping = true;
                _slotMainBoard.StopAllSpinning(args.isFastSpeed);
            }
        }

        protected virtual void OnStartAutoplayButtonClicked(object sender, AutoplayHandler.AutoplayEventArgs args)
        {
            if (_statesController.currentStateName == "ReadyForSpin" || _statesController.currentStateName == "RoundRunning")
            {
                _baseGameAutoplayingHandler.StartAutoplay(args.autoplayOptions);
            }
        }

        protected virtual void OnStopAutoplayButtonClicked(object sender, EventArgs args)
        {
            _baseGameAutoplayingHandler.StopAutoPlay();
        }

        protected virtual void OnTryToAdjustBetRate(object sender, PlayerStatus.BetRateAdjustEventArgs args)
        {
            if (isPlayerAbleToSpin)
            {
                print("Show Bet Step: " + args.step);
                _playerStatus.AdjustBetRate(args.step);
            }
        }

        protected virtual void OnSlotMainBoardSpinningEnded(object sender, EventArgs args)
        {
            _statesParameters.isSlotMainBoardSpinningEnded = true;
        }

        protected virtual void OnSlotMainBoardTotalWonAnimationEneded(object sender, EventArgs args)
        {
            _statesParameters.isSlotMainBoardTotalWonAnimationEnd = true;
        }

        protected virtual void OnSlotMainBoardGetSwildAnimationEnded(object sender, EventArgs args)
        {
            _statesParameters.isSlotMainBoardGetSwildAnimationEnd = true;
        }

        protected virtual void OnRoundResultDataLoaded(object sender, NetworkDataReceiver.RoundResultDataLoadedEventArgs args)
        {
           
            if (args.roundResultData.bonusGameInfo == null || !args.roundResultData.bonusGameInfo.IsBonusRound)
            {
                // Base game round
                _currentRoundData = new BaseGameRoundData(_playerStatus.Balance, _statesParameters.lastestBet, _statesParameters.lastestBetRate, args.roundResultData);
                BroadcastMessage("ReceiveRoundData", _currentRoundData);
            }
            else
            {
                // Bonus game round
                _statesParameters.pendingBonusGameResultData.Insert(0, args.roundResultData);
            }            
        }


        // Event Calling for Derived Classes
        protected virtual void InvokeRoundEnded(RoundEndedEventArgs args)
        {
            RoundEnded?.Invoke(this, args);
        }
        protected virtual void InvokeBigWinAnimationStarted()
        {
            BigWinAnimationStarted?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void InvokeBigWinAnimationEnded()
        {
            BigWinAnimationEnded?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void InvokeBonusGameTransitionInStarted()
        {
            BonusGameTransitionInStarted?.Invoke(this, EventArgs.Empty);
        }
        //���Ĺ�����G
        protected virtual void InvokeBonusGameResultShowingStarted()
        {
            BonusGameResultShowingStarted?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void InvokeBonusGameResultShowingEnded()
        {
            BonusGameResultShowingEnded?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void InvokeBonusGameTransitionOutStarted()
        {
            BonusGameTransitionOutStarted?.Invoke(this, EventArgs.Empty);
        }

        //�s�W�i�JBonus�C������J�¹�����
        protected virtual void InvokeEnteringBonusGameStart()
        {
            EnteringBonusGameStart?.Invoke(this, EventArgs.Empty);
        }
        //�s�W���}Bonus�C������X�¹�����
        protected virtual void InvokeEnteringBonusGameEnd()
        {
            EnteringBonusGameEnd?.Invoke(this, EventArgs.Empty);
        }
        //�s�W���}Bonus�C������X�¹�����
        protected virtual void InvokeBonusGameTotalWinsDisplay()
        {
            BonusGameTotalWinsDisplay?.Invoke(this, EventArgs.Empty);
        }


        // == Nested Classes ==
        public class StopAllSpinningEventArgs : EventArgs
        {
            public bool isFastSpeed;
        }

        public class RoundEndedEventArgs : EventArgs
        {
            public bool isBonusRound;
            public bool isFinalBonusRound;
        }


        public class StatesParameters
        {
            // public bool isListeningGeneralAdvance = false;
            // public bool isGeneralAdvancingOccurred = false;
            public bool isPlayerAdvanced = false;

            public bool IsAdvancing => /*(isListeningGeneralAdvance && isGeneralAdvancingOccurred) ||*/ isPlayerAdvanced;


            public bool isInited = false;

            public bool isStartingToSpinSlot = false;
            public bool isFromAutoplay = false;
            public float roundTimeScale = 1f;
            public bool willIgnoreWaitingForBonusIconEffect = false;

            // public bool isReadyForNextRound = false;

            public bool isInBonusGame = false;
            public bool hasBonusGameStartedPlaying = false;

            public bool isSlotMainBoardSpinningEnded = false;
            public bool isSlotMainBoardTotalWonAnimationEnd = false;
            //�����S��
            public bool isSlotMainBoardGetSwildAnimationEnd = false;

            public decimal lastestBet = -1;
            public decimal lastestBetRate = 1;
            public int wonBonusSituationsCount = 0;
            public int gainedBonusRounds = 0;
            public RoundData pendingRoundDataWhenBonusGameTriggered = null;
            public decimal accumulatedBonusWin = 0;
            public bool hasAchievedMaxBonusRounds = false;

            //�����S��swild�����Ʋ֭p
            public int accumulatedSwildMultiplier = 1;

            public decimal BonusWinPerWonBonusLine => accumulatedBonusWin / wonBonusSituationsCount;
            
            public List<RoundResultData> pendingBonusGameResultData = new List<RoundResultData>();
            public bool HasBonusGamePending
            {
                get
                {
                    try
                    {
                        return pendingBonusGameResultData.Count > 0;
                    }
                    catch
                    {
                        pendingBonusGameResultData = new List<RoundResultData>();
                        return pendingBonusGameResultData.Count > 0;
                    }
                }
            }
            //public bool HasBonusGamePending => pendingBonusGameResultData.Count > 0;
        }

    }

}
