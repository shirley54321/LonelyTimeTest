using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class SlotMainBoard : MonoBehaviour, IDBCollectionReceiver {

        // Events
        public event EventHandler<StatesController.StateChangedEventArgs> StateChanged;
        public event EventHandler<SpinningStartEventArgs> SpinningStarted;
        public event EventHandler<EventArgs> ReelsStopped;
        public event EventHandler<EventArgs> SpinningEnded;
        public event EventHandler<EventArgs> BonusGameTriggered;
        public event EventHandler<TotalWonAnimationStartedEventArgs> TotalWonAnimtaionStarted;
        public event EventHandler<EventArgs> TotalWonAnimationEnded;
        public event EventHandler<WonLineAnimationStartedEventArgs> WonLineAnimationStarted;
        public event EventHandler<SlotMainBoardAnimationManager.WonLineAnimationEndedEventArgs> WonLineAnimationEnded;

        public event EventHandler<ReelController.ReelEventArgs> ReelSpinningStarted;
        public event EventHandler<ReelController.StoppingStartedEventArgs> ReelStoppingStarted;
        public event EventHandler<ReelController.ReelEventArgs> ReelStopped;
        public event EventHandler<ReelController.WaitingForBonusIconEffectPlayedEventArgs> WaitingForBonusIconEffectsPlayed;

        //�����S��
        public event EventHandler<EventArgs> GetSwildAnimationEnded;

        public string CurrentStateName => _statesController != null ? _statesController.currentStateName : "";
        //�����S��_BonusIconId
        public int BonusIconId => _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1;

        [Header("DBs")]
        //�����S��_gameInfoDB
        [SerializeField] GameInfoDB _gameInfoDB;
        [SerializeField] SlotLinesDB _slotLinesDB;


        StatesController _statesController;
        StatesParameters _statesParameters = new StatesParameters();

        SlotMainBoardAnimationManager _animManager;
        public SlotMainBoardAnimationManager animManager {
            get {
                if (_animManager == null) {
                    _animManager = GetComponent<SlotMainBoardAnimationManager>();
                }
                return _animManager;
            }
        }

        [Header("REFS")]
        [SerializeField] ReelsController _reelsController;
        RoundData _currentRoundData = null;
        BaseGameRoundData _pendingBaseGameRoundData = null;


        #if UNITY_EDITOR
            [Header("Debug Showing")]
            [SerializeField] string currentStateName;
        #endif


        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB ?? _slotLinesDB;
            _gameInfoDB = dbCollection.gameInfoDB ?? _gameInfoDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB;
            _gameInfoDB = dbCollection.gameInfoDB;
        }


        public event EventHandler<EventArgs> StopAutoplayButtonClicked;

        void Awake () {
            _statesController = new StatesController(this, new State[] {
                new State("Idle", StateIdle),
                new State("Spinning", StateSpinnning),
                new State("TotalWonAnimationPlaying", StateWinningAnimationPlaying)
            } );
            _statesController.StateChanged += (sender, args) => {
                if (StateChanged != null) {
                    StateChanged(this, args);
                }
            };
            _statesController.Run("Idle");
        }

        void OnEnable () {

            if (animManager != null) {
                animManager.WonLineAnimationStarted += OnWonLineAnimationStart;
                animManager.WonLineAnimationEnded += OnWonLineAnimationEnded;
            }

            if (_reelsController != null) {
                _reelsController.ReelSpinningStarted += OnReelSpinningStart;
                _reelsController.ReelStoppingStarted += OnReelStoppingStart;
                _reelsController.ReelStopped += OnReelStop;
                _reelsController.ReelWaitingForBonusIconEffectPlayed += OnWaitingForBonusIconEffectPlay;
                _reelsController.WaitingForBonusIconOccurred += OnWaitingForBonusIconOccurred;
                _reelsController.ReelsStopped += OnReelsStopped;
            }
        }

        void OnDisable () {
            _statesController.Shutdown();

            if (animManager != null) {
                animManager.WonLineAnimationStarted -= OnWonLineAnimationStart;
                animManager.WonLineAnimationEnded -= OnWonLineAnimationEnded;
            }

            if (_reelsController != null) {
                _reelsController.ReelSpinningStarted -= OnReelSpinningStart;
                _reelsController.ReelStoppingStarted -= OnReelStoppingStart;
                _reelsController.ReelStopped -= OnReelStop;
                _reelsController.ReelWaitingForBonusIconEffectPlayed -= OnWaitingForBonusIconEffectPlay;
                _reelsController.WaitingForBonusIconOccurred -= OnWaitingForBonusIconOccurred;
                _reelsController.ReelsStopped -= OnReelsStopped;
            }
        }

        void Start () {
            Reset(ReelController.GameModeType.BaseGame);
        }


        #if UNITY_EDITOR
            void Update () {
                currentStateName = _statesController.currentStateName;
            }
        #endif


        public void Reset (ReelController.GameModeType gameModeType) {
            animManager.ClearAllWinningAnimations();
            _statesController.Shutdown();
            _statesController.Run("Idle");
            _reelsController.ResetIcons(gameModeType);
        }

        public void Spin (float appliedAdditionalTimeScale = 1f, bool willIgnoreWaitingForBonusIconEffect = false) {
            _statesParameters.isStartToSpin = true;
            _statesParameters.roundAppliedAdditionalTimeScale = appliedAdditionalTimeScale;
            _statesParameters.willIgnoreWaitingForBonusIconEffect = willIgnoreWaitingForBonusIconEffect;
        }

        public void StopAllSpinning (bool isFastSpeed = false) {
            StopAutoplayButtonClicked?.Invoke(this, EventArgs.Empty);
            _reelsController.StopAllReels(isFastSpeed);
        }

        public void RestoreBaseGameRoundFromPending (decimal additionalWinPerWonBonusLine = 0) {

            animManager.ClearAllWinningAnimations();
            _statesController.Shutdown();

            _currentRoundData = _pendingBaseGameRoundData;
            _reelsController.SetShowedIconsId(_currentRoundData.resultData.showedIconsId);

            _pendingBaseGameRoundData = null;

            animManager.PlaySeparatedWinningAnimation(_currentRoundData.resultData.showedIconsId, _currentRoundData.resultData.wonSituationsInfo, additionalWinPerWonBonusLine);
            _statesController.Run("Idle");
        }


        // Message from GameStatesManager
        void ReceiveRoundData (RoundData data) {
            _currentRoundData = data;
            _reelsController.SetTargetIconsId(_currentRoundData.resultData.showedIconsId);

            if (_currentRoundData is BaseGameRoundData && _currentRoundData.resultData.IsBonusOccurred) {
                SlotTemplate.PlayerStatus.Instance.UpdateVirtualCoin();//計算本場龍幣增加或減少的部分
                //Time.timeScale = 0f;
                //Debug.Log("遊戲已暫停");

                _pendingBaseGameRoundData = (BaseGameRoundData) _currentRoundData;
            }
        }


        // --- States ---
        IEnumerator StateIdle (StatesController attachedStatesController) {

            yield return new WaitUntil(() => _statesParameters.isStartToSpin);

            attachedStatesController.nextStateName = "Spinning";
            animManager.ClearAllWinningAnimations();
        }

        IEnumerator StateSpinnning (StatesController attachedStatesController) {

            TimeScaleController.AdditionalAppliedTimeScale = _statesParameters.roundAppliedAdditionalTimeScale;

            bool isBonusGame = _currentRoundData is BonusGameRoundData;

            SpinningStarted?.Invoke(this, new SpinningStartEventArgs{
                isBonusGame = isBonusGame
            });

            ReelController.GameModeType gameModeType = isBonusGame ? ReelController.GameModeType.BonusGame : ReelController.GameModeType.BaseGame;
            _reelsController.Spin(gameModeType, _statesParameters.willIgnoreWaitingForBonusIconEffect);

            yield return new WaitUntil(() => _statesParameters.isSpinningEnded);

            _statesParameters.isSpinningEnded = false;

            if (_currentRoundData.resultData.HasWon || _currentRoundData.resultData.WonSwildOccurred) {
                attachedStatesController.nextStateName = "TotalWonAnimationPlaying";
            }
            else {
                attachedStatesController.nextStateName = "Idle";
            }


            TimeScaleController.AdditionalAppliedTimeScale = 1f;

            _statesParameters.isStartToSpin = false;
            _statesParameters.roundAppliedAdditionalTimeScale = 1f;
            _statesParameters.willIgnoreWaitingForBonusIconEffect = false;

            SpinningEnded?.Invoke(this, EventArgs.Empty);
        }

        IEnumerator StateWinningAnimationPlaying (StatesController attachedStatesController) {
            decimal remainedWinScoreShowed = 0;
            if (_currentRoundData is BonusGameRoundData) {
                BonusGameRoundData currentBonusGameRoundData = ((BonusGameRoundData) _currentRoundData);
                remainedWinScoreShowed = currentBonusGameRoundData.previousAccumulatedWin / currentBonusGameRoundData.wonBonusLinesCountWhenBonusGameTriggered;
            }

            decimal totalWin;
            if (_currentRoundData is BonusGameRoundData) {
                totalWin = _currentRoundData.resultData.totalWin / ((BonusGameRoundData) _currentRoundData).wonBonusLinesCountWhenBonusGameTriggered;
            }
            else {
                totalWin = _currentRoundData.resultData.totalWin;
            }

            

            if (_currentRoundData is BonusGameRoundData && _gameInfoDB.swildOccurredType == GameInfoDB.SwildOccurredType.hasSwild )
            {
                if (_currentRoundData.resultData.WonSwildOccurred)
                {
                    RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo = _currentRoundData.resultData.swildIconWinSituationInfo;
                    yield return animManager.StartGetSwildAnimationPlayingCoroutine(_currentRoundData.resultData.showedIconsId, swildIconWinSituationInfo);
                    GetSwildAnimationEnded?.Invoke(this, EventArgs.Empty);
                    animManager.ClearAllWinningAnimations();
                    yield return new WaitForSeconds(0.5f);
                }
            }

            

            TotalWonAnimtaionStarted?.Invoke(this, new TotalWonAnimationStartedEventArgs{
                totalWin = totalWin,
                remainedWinScoreShowed = remainedWinScoreShowed,
                animDuration = animManager.TotalWonAnimationDuration,
                containedAnimatedIconsId = _currentRoundData.resultData.GetContainedWonIconId(_slotLinesDB)
            });


            bool willPlaySeparateWonLines;
            RoundResultData.WonSituationInfo[] wonSituationsInfo;

            if (_currentRoundData is BaseGameRoundData && _currentRoundData.resultData.IsBonusOccurred) {
                SlotTemplate.PlayerStatus.Instance.UpdateVirtualCoin();//計算本場龍幣增加或減少的部分
                //Time.timeScale = 0f;
                //Debug.Log("遊戲已暫停"); 

                // Bonus game triggered
                willPlaySeparateWonLines = false;
                wonSituationsInfo = _currentRoundData.resultData.WonBonusSituationsInfo;

                BonusGameTriggered?.Invoke(this, EventArgs.Empty);
            }
            else {
                willPlaySeparateWonLines = true;
                wonSituationsInfo = _currentRoundData.resultData.wonSituationsInfo;
            }
            yield return animManager.StartTotalWonAnimationPlayingCoroutine(_currentRoundData.resultData.showedIconsId, wonSituationsInfo);

            TotalWonAnimationEnded?.Invoke(this, EventArgs.Empty);

            attachedStatesController.nextStateName = "Idle";


            if (willPlaySeparateWonLines) {
                // ==vv== abandoned ==vv==
                // int[] prioritizedIconsId = null;
                // if (_currentRoundData.resultData.IsBonusOccurred) {
                //     prioritizedIconsId = new int[] {_currentRoundData.resultData.bonusIconId};
                // }
                animManager.PlaySeparatedWinningAnimation(_currentRoundData.resultData.showedIconsId, _currentRoundData.resultData.wonSituationsInfo);
            }
        }

        // --- States_end ---


        // OnEvents
        void OnReelSpinningStart (object sender, ReelController.ReelEventArgs args) {
            ReelSpinningStarted?.Invoke(this, args);
        }

        void OnReelStoppingStart (object sender, ReelController.StoppingStartedEventArgs args) {
            ReelStoppingStarted?.Invoke(this, args);
        }

        void OnReelStop (object sender, ReelController.ReelEventArgs args) {

            animManager.StopWaitingForBonusEffect(args.reelIndex);

            ReelStopped?.Invoke(this, args);
        }

        void OnWaitingForBonusIconEffectPlay (object sender, ReelController.WaitingForBonusIconEffectPlayedEventArgs args) {

            animManager.PlayWaitingForBonusEffect(args.reelIndex, args.reelTransform);

            if (!_statesParameters.isWaitingForBonusIconEffectsPlayed) {

                WaitingForBonusIconEffectsPlayed?.Invoke(this, args);

                _statesParameters.isWaitingForBonusIconEffectsPlayed = true;
            }
        }

        void OnWaitingForBonusIconOccurred (object sender, ReelsController.WaitingForBonusIconOccurredEventArgs args) {
            // -- Abandoned --
            // _animManager.PlayScaleAnimatedIcon(_currentRoundData.resultData.showedIconsId, args.appearedBonusIconCoords);
        }

        void OnReelsStopped (object sender, EventArgs args) {
            _statesParameters.isSpinningEnded = true;
            _statesParameters.isWaitingForBonusIconEffectsPlayed = false;

            ReelsStopped?.Invoke(this, EventArgs.Empty);
        }

        void OnWonLineAnimationStart(object sender, SlotMainBoardAnimationManager.WonLineAnimationStartedEventArgs args) {

            decimal wonLineScore;
            if (_currentRoundData is BonusGameRoundData)
            {
                wonLineScore = args.baseWinScore * _currentRoundData.betRate + args.additionalWin;
            }
            else 
            {
                wonLineScore = args.wonIconId == BonusIconId ? args.additionalWin : args.baseWinScore * _currentRoundData.betRate + args.additionalWin;
            }

            WonLineAnimationStarted?.Invoke(this, new WonLineAnimationStartedEventArgs{
                wonIconId = args.wonIconId,
                wonIconCount = args.wonIconCount,
                winScore = wonLineScore
            });
        }

        void OnWonLineAnimationEnded (object sender, SlotMainBoardAnimationManager.WonLineAnimationEndedEventArgs args) {
            WonLineAnimationEnded?.Invoke(this, args);
        }




        public class SpinningStartEventArgs : EventArgs {
            public bool isBonusGame;
        }

        public class TotalWonAnimationStartedEventArgs : EventArgs {
            public decimal totalWin;
            public decimal remainedWinScoreShowed;
            public float animDuration;
            public int[] containedAnimatedIconsId;
        }

        public class WonLineAnimationStartedEventArgs : EventArgs {
            public int wonIconId;
            public int wonIconCount;
            public decimal winScore;
        }


        class StatesParameters {
            public bool isStartToSpin = false;
            public float roundAppliedAdditionalTimeScale = 0.5f;
            public bool willIgnoreWaitingForBonusIconEffect = false;

            public bool isSpinningEnded = false;
            public bool isWaitingForBonusIconEffectsPlayed = false;
        }

    }
}
