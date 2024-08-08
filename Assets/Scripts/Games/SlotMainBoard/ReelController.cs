using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotTemplate {

    // This object will be located at the middle of the showing column.
    public class ReelController : MonoBehaviour, IDBCollectionReceiver {

        // == static Variables ==
        static readonly Vector3 _spinDirection = Vector3.down;


        // == Events ==
        public event EventHandler<ReelEventArgs> SpinningStarted;
        public event EventHandler<StoppingStartedEventArgs> StoppingStarted;
        public event EventHandler<ReelEventArgs> Stopped;
        public event EventHandler<TargetIconsShowedEventArgs> TargetIconsShowed;
        public event EventHandler<WaitingForBonusIconEffectPlayedEventArgs> WaitingForBonusIconEffectPlayed;


        // == Variables ==
        public int BonusIconId => _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1;


        public int ReelIndex { get; set; } = -1;
        public int ShowedIconsCount => transform.childCount;

        public bool IsIdle => _statesController.currentStateName == "Idle";

        public float RegularSpinningSpeed { get; private set; } = 0f;
        public float CurrentSpeedRate { get; private set; } = 1f;

        public int[] TargetIconsId { get; set; }


        int _currentPositionOnFakeIconsIdSheetOfBaseGame = -1;
        int _currentPositionOnFakeIconsIdSheetOfBonusGame = -1;
        GameModeType _currentGameModeType = GameModeType.BaseGame;


        StatesController _statesController;
        StatesParameters _statesParameters = new StatesParameters();

        float _iconsIntervalLocalDistance = 0f;
        float _topToBottomEdgeLocalDistance = 0f;

        [Tooltip("Abadoned")]
        [SerializeField] int[] _possibleIconsId;
        [SerializeField] float _stoppingDelayDuration = -1f;
        [SerializeField] StoppingEffectInfo _stoppingEffectInfo;

        [Header("DBs")]
        [SerializeField] GameInfoDB _gameInfoDB;
        [SerializeField] FakeReelIconIdsTableDB _fakeReelIconIdsTableDB;


        public int[] fakeReelIconsIdSheetOfBaseGame {
            get {
                return _fakeReelIconIdsTableDB.TableOfBaseGame[ReelIndex];
            }
        }

        public int[] fakeReelIconsIdSheetOfBonusGame {
            get {
                return _fakeReelIconIdsTableDB.TableOfBonusGame[ReelIndex];
            }
        }

        //原公版沒有-------
        public GameModeType currentGameModeType
        {
            get { return _currentGameModeType; }
        }

        public Vector3 spinDirection
        {
            get { return _spinDirection; }
        }

        public float topToBottomEdgeLocalDistance
        {
            get { return _topToBottomEdgeLocalDistance; }
        }

        public float iconsIntervalLocalDistance
        {
            get { return _iconsIntervalLocalDistance; }
        }
        public List<int> pendingTargetIconsId
        {
            get { return _pendingTargetIconsId; }
            set { _pendingTargetIconsId = value; }
        }

        public bool isPreparingToStopManually
        {
            get { return _statesParameters.isPreparingToStopManually; }
        }

        public bool isStartingToGoStopping
        {
            get { return _statesParameters.isStartingToGoStopping; }
            set { _statesParameters.isStartingToGoStopping = value; }
        }

        public static float AddiotionalReelDelay { get; internal set; }
        //---------


        List<int> _pendingTargetIconsId;


        #if UNITY_EDITOR
            [Header("Debug Options")]
            [SerializeField] bool _showDebugLog = false;
        #endif


        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB ?? _gameInfoDB;
            _fakeReelIconIdsTableDB = dbCollection.fakeReelIconIdsTableDB ?? _fakeReelIconIdsTableDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB;
            _fakeReelIconIdsTableDB = dbCollection.fakeReelIconIdsTableDB;
        }


        // == Unity Messages ==
        void Awake () {
            if (transform.childCount > 1) {
                _iconsIntervalLocalDistance = Mathf.Abs(Vector3.Dot(transform.GetChild(0).localPosition - transform.GetChild(1).localPosition, _spinDirection));
            }

            _topToBottomEdgeLocalDistance = _iconsIntervalLocalDistance * transform.childCount;;

            _statesController = new StatesController(this, new State[] {
                new State("Idle", StateIdle),
                new State("Spinning", StateSpinning),
                new State("Stopping", StateStopping)  // The pulling back animation
            } );
            _statesController.Run("Idle");
        }

        void OnEnable () {
            _statesController.StateChanged += OnStatesControllerStateChanged;
        }

        void OnDisable () {
            _statesController.StateChanged -= OnStatesControllerStateChanged;
        }


        // == Public Methods ==
        public void ResetWithRandomIcons (GameModeType gameModeType) {
            _currentGameModeType = gameModeType;
            SetShowedIconsId(GetSeriesOfRandomIconsIdOfGameModeType(_currentGameModeType, ShowedIconsCount));
        }

        public virtual void SetShowedIconsId (int[] showedIconsId) {
            IconCarrier[] iconCarriers = GetAllIconCarriers();
            for (int i = 0 ; i < iconCarriers.Length ; i++) {
                if (i < showedIconsId.Length && iconCarriers[i] != null) {
                    iconCarriers[i].CurrentIconId = showedIconsId[i];
                }
            }
        }

        public void Spin (GameModeType gameModeType, float speed, float speedRate = 1f) {
            _currentGameModeType = gameModeType;
            RegularSpinningSpeed = speed;
            CurrentSpeedRate = speedRate;
            _statesParameters.isStartingToSpin = true;
        }

        public void StopSpinningWithDelay () {
            _statesParameters.isStartingToStopWithDelay = true;
        }

        public void StopSpinningManually (float speedRate = -1f) {
            if (_statesController.currentStateName == "Spinning") {

                if (!_statesParameters.isPreparingToStopManually) {
                    StartCoroutine(PreparingToStopManually());
                    _statesParameters.isPreparingToStopManually = true;
                }

                if (speedRate > 0) {
                    CurrentSpeedRate = speedRate;
                }
            }
        }

        public void SetIsWaitingWaitingForBonusIconEffect () {
            _statesParameters.isWaitingForWaitingForBonusIconEffect = true;
        }

        public void ReadyToPlayWaitForBonusIconEffect (float speedRate = -1f, float additionalDelay = 0f) {

            _statesParameters.isWaitingForWaitingForBonusIconEffect = false;

            if (_statesController.currentStateName != "Idle") {

                if (speedRate > 0) {
                    CurrentSpeedRate = speedRate;
                }

                _statesParameters.additionalStoppingDelay = additionalDelay;
                _statesParameters.isWaitingForBonusIconEffectPlayed = true;

                // Invoke event
                if (WaitingForBonusIconEffectPlayed != null) {
                    WaitingForBonusIconEffectPlayed(this, new WaitingForBonusIconEffectPlayedEventArgs{
                        reelIndex = ReelIndex,
                        reelTransform = transform
                    });
                }
            }
        }

        public void StopWaitingWaitingForBonusIconEffect () {
            _statesParameters.isWaitingForWaitingForBonusIconEffect = false;
        }


        // == Private Methods ==
        // --- States ---
        IEnumerator StateIdle (StatesController attachedStatesController) {
            _statesParameters.isStartingToSpin = false;

            yield return new WaitUntil(() => _statesParameters.isStartingToSpin);

            attachedStatesController.nextStateName = "Spinning";
        }

        IEnumerator StateSpinning (StatesController attachedStatesController) {
            _pendingTargetIconsId = null;
            _statesParameters.isPreparingToStopManually = false;
            _statesParameters.isStartingToStopWithDelay = false;
            _statesParameters.additionalStoppingDelay = 0f;
            _statesParameters.isWaitingForBonusIconEffectPlayed = false;

            SpinningStarted?.Invoke(this, GetReelEventArgs());


            float stopWithDelayStartTime = -1f;
            while (!_statesParameters.isStartingToGoStopping) {

                MoveIcons(_spinDirection * RegularSpinningSpeed * CurrentSpeedRate * Time.deltaTime);

                if (stopWithDelayStartTime == -1f) {
                    if (_statesParameters.isStartingToStopWithDelay) {
                        stopWithDelayStartTime = Time.time;
                    }
                }
                else {

                    //Debug.Log($"AddiotionalReelDelay: " + AddiotionalReelDelay);
                    if (!_statesParameters.isWaitingForWaitingForBonusIconEffect && Time.time - stopWithDelayStartTime > (_stoppingDelayDuration+AddiotionalReelDelay) + _statesParameters.additionalStoppingDelay) {
                        StopSpinningManually();
                    }
                }

                yield return null;
            }


            _statesParameters.isStartingToGoStopping = false;

            attachedStatesController.nextStateName = "Stopping";
        }

        IEnumerator StateStopping (StatesController attachedStatesController) {

            ResetIconCarriersPosition();
            SetShowedIconsId(TargetIconsId);

            StoppingStarted?.Invoke(this, new StoppingStartedEventArgs {
                reelIndex = ReelIndex,
                isBonusIconAppeared = Array.Exists(TargetIconsId, id => id == BonusIconId)
            });


            Vector3 startingLocalPos = transform.localPosition;

            // -- Drag --
            float duration = _stoppingEffectInfo.dragDuration / (CurrentSpeedRate > 0 ? CurrentSpeedRate : 1f);
            float startTime = Time.time;
            float elapsedRate = 0f;

            while (elapsedRate <= 1f) {

                yield return null;

                elapsedRate = (Time.time - startTime) / duration;
                transform.localPosition = startingLocalPos + _spinDirection * Mathf.Lerp(0f, _stoppingEffectInfo.dragLocalDistance, elapsedRate);
            }

            // -- Pulled back --
            duration = _stoppingEffectInfo.pulledBackDuration / (CurrentSpeedRate > 0 ? CurrentSpeedRate : 1f);
            startTime = Time.time;
            elapsedRate = 0f;

            while (elapsedRate <= 1f) {

                yield return null;

                elapsedRate = (Time.time - startTime) / duration;
                transform.localPosition = startingLocalPos + _spinDirection * Mathf.Lerp(_stoppingEffectInfo.dragLocalDistance, 0f, elapsedRate);
            }

            ResetIconCarriersPosition();

            Stopped?.Invoke(this, new ReelEventArgs{
                reelIndex = ReelIndex
            });


            TargetIconsShowed?.Invoke(this, new TargetIconsShowedEventArgs{
                reelIndex = ReelIndex,
                targetIconsId = TargetIconsId
            });

            TargetIconsId = null;

            attachedStatesController.nextStateName = "Idle";
        }
        // --- States_end ---



        IEnumerator PreparingToStopManually () {

            while (TargetIconsId == null) {
                yield return null;
            }

            _pendingTargetIconsId = new List<int>(TargetIconsId);
        }


        protected virtual void ResetIconCarriersPosition () {
            for (int i = 0 ; i < transform.childCount ; i++) {
                transform.GetChild(i).localPosition = _spinDirection * (_topToBottomEdgeLocalDistance / 2 -_iconsIntervalLocalDistance * (i + 1));
            }

        }

        protected virtual void MoveIcons (Vector3 movement) {
            List<IconCarrier> iconCarriersWhoOverTheLimit = new List<IconCarrier>();

            for (int i = 0 ; i < transform.childCount ; i++) {
                Transform child = transform.GetChild(i);

                child.localPosition += movement;

                if (Vector3.Dot(child.localPosition, _spinDirection) > _topToBottomEdgeLocalDistance / 2) {
                    iconCarriersWhoOverTheLimit.Add(child.gameObject.GetComponent<IconCarrier>());
                }
            }

            foreach (IconCarrier iconCarrier in iconCarriersWhoOverTheLimit) {
                OnIconCarrierSpinnedOverTheLimit(iconCarrier);
            }
        }
        
        protected virtual void OnIconCarrierSpinnedOverTheLimit (IconCarrier iconCarrier) {
            float deltaDistanceFromBottom = Vector3.Dot(iconCarrier.transform.localPosition, _spinDirection) - _topToBottomEdgeLocalDistance / 2;
            iconCarrier.transform.localPosition = _spinDirection * (deltaDistanceFromBottom % _topToBottomEdgeLocalDistance - _topToBottomEdgeLocalDistance / 2);
            iconCarrier.transform.SetAsLastSibling();

            int newId = -1;


            if (_statesParameters.isPreparingToStopManually && _pendingTargetIconsId != null ) {

                if (_pendingTargetIconsId.Count > 0) {

                    newId = _pendingTargetIconsId[0];
                    _pendingTargetIconsId.RemoveAt(0);
                }
                else if (!_statesParameters.isStartingToGoStopping) {

                    _statesParameters.isStartingToGoStopping = true;
                    return;
                }
            }
            else {
                // newId = GetRandomIconId();
                newId = GetNextIconIdOfGameModeType(_currentGameModeType);
            }

            iconCarrier.CurrentIconId = newId;
        }



        int[] GetSeriesOfRandomIconsIdOfGameModeType (GameModeType type, int length) {
            if (type == GameModeType.BaseGame) {
                return GetSeriesOfRandomIconsIdFromSheet(fakeReelIconsIdSheetOfBaseGame, length, ref _currentPositionOnFakeIconsIdSheetOfBaseGame);
            }
            else if (type == GameModeType.BonusGame) {
                return GetSeriesOfRandomIconsIdFromSheet(fakeReelIconsIdSheetOfBonusGame, length, ref _currentPositionOnFakeIconsIdSheetOfBonusGame);
            }

            int[] emptyResult = new int[length];
            for (int i = 0 ; i < emptyResult.Length ; i++) {
                emptyResult[i] = -1;
            }
            return emptyResult;
        }

        int[] GetSeriesOfRandomIconsIdFromSheet (int[] iconsIdSheet, int length, ref int currentPositionOnSheet) {
            int[] iconsId = new int[length];

            int startIndex = Random.Range(0, iconsIdSheet.Length);

            for (int i = 0 ; i < iconsId.Length ; i++) {
                iconsId[i] = GetNextIconIdOnFakeIconsIdSheet(iconsIdSheet, ref currentPositionOnSheet);
            }

            return iconsId;
        }

        protected int GetNextIconIdOfGameModeType (GameModeType type) {
            if (type == GameModeType.BaseGame) {
                return GetNextIconIdOnFakeIconsIdSheet(fakeReelIconsIdSheetOfBaseGame, ref _currentPositionOnFakeIconsIdSheetOfBaseGame);
            }
            else if (type == GameModeType.BonusGame) {
                return GetNextIconIdOnFakeIconsIdSheet(fakeReelIconsIdSheetOfBonusGame, ref _currentPositionOnFakeIconsIdSheetOfBonusGame);
            }

            return -1;
        }

        int GetNextIconIdOnFakeIconsIdSheet (int[] iconsIdSheet, ref int currentPositionOnSheet) {
            if (currentPositionOnSheet < 0) {
                currentPositionOnSheet = Random.Range(0, iconsIdSheet.Length);
            }
            else {
                currentPositionOnSheet = (currentPositionOnSheet + 1) % iconsIdSheet.Length;
            }
            return iconsIdSheet[currentPositionOnSheet];
        }


        ReelEventArgs GetReelEventArgs () {
            return new ReelEventArgs {
                reelIndex = ReelIndex
            };
        }

        IconCarrier GetIconCarrier (int index) {
            if (index < transform.childCount) {
                return transform.GetChild(index).gameObject.GetComponent<IconCarrier>();
            }
            return null;
        }

       protected virtual IconCarrier[] GetAllIconCarriers () {
            List<IconCarrier> iconCarrierList = new List<IconCarrier>();
            foreach (Transform child in transform) {
                iconCarrierList.Add(child.gameObject.GetComponent<IconCarrier>());
            }
            return iconCarrierList.ToArray();
        }


        // == Events received ==
        void OnStatesControllerStateChanged (object sender, StatesController.StateChangedEventArgs args) {
            #if UNITY_EDITOR
                if (_showDebugLog) {
                    Debug.Log(gameObject.name + "_ReelController_State: " + args.newStateName);
                }
            #endif
        }

        // == Nested Classes ==
        public class ReelEventArgs : EventArgs {
            public int reelIndex;
        }

        public class StoppingStartedEventArgs : ReelEventArgs {
            public bool isBonusIconAppeared;
        }

        public class TargetIconsShowedEventArgs : ReelEventArgs {
            public int[] targetIconsId;
        }

        public class WaitingForBonusIconEffectPlayedEventArgs : ReelEventArgs {
            public int reelIndex;
            public Transform reelTransform;
        }

        [Serializable]
        class StoppingEffectInfo {
            public float dragLocalDistance = 0f;
            public float dragDuration = 0f;
            public float pulledBackDuration = 0f;
        }

        class StatesParameters {
            public bool isStartingToSpin = false;
            public bool isStartingToStopWithDelay = false;
            public bool isStartingToGoStopping = false;
            public bool isPreparingToStopManually = false;
            public bool isWaitingForWaitingForBonusIconEffect = false;
            public float additionalStoppingDelay = 0f;
            public bool isWaitingForBonusIconEffectPlayed = false;
        }


        public enum GameModeType {
            BaseGame,
            BonusGame
        }

    }

}
