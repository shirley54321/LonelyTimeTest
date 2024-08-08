using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SlotTemplate {

    [DisallowMultipleComponent]
    public class ReelsController : MonoBehaviour, IDBCollectionReceiver {

        // const int _REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING = 3;
        // protected int REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING => _REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING;

        // == Events ==
        public event EventHandler<ReelController.ReelEventArgs> ReelSpinningStarted;
        public event EventHandler<ReelController.StoppingStartedEventArgs> ReelStoppingStarted;
        public event EventHandler<ReelController.ReelEventArgs> ReelStopped;
        public event EventHandler<ReelController.TargetIconsShowedEventArgs> ReelTargetIconsShowed;
        public event EventHandler<ReelController.WaitingForBonusIconEffectPlayedEventArgs> ReelWaitingForBonusIconEffectPlayed;

        public event EventHandler<WaitingForBonusIconOccurredEventArgs> WaitingForBonusIconOccurred;
        public event EventHandler<EventArgs> ReelsStopped;


        public int BonusIconId => _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1;
        protected int REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING => _gameInfoDB.requiredIconAmountForBonusOccurring;

        public int[] CurrentIdlingReelsIndex {
            get {
                List<int> resultList = new List<int>();

                ReelController[] reelControllers = GetAllReelControllers();
                foreach (ReelController reelController in reelControllers) {
                    if (reelController.IsIdle) {
                        resultList.Add(reelController.ReelIndex);
                    }
                }
                return resultList.ToArray();
            }
        }

        StatesController _statesController;
        protected StatesController statesController {
            get => _statesController;
            set => _statesController = value;
        }

        StatesParameters _statesParameters = new StatesParameters();
        protected StatesParameters statesParameters {
            get => _statesParameters;
            set => _statesParameters = value;
        }

        [SerializeField] SpinningInfo _spinningInfo;
        protected SpinningInfo spinningInfo {
            get => _spinningInfo;
            set => _spinningInfo = value;
        }

        [SerializeField] ElementsGeneratingInfo _elementsGeneratingInfo;
        protected ElementsGeneratingInfo elementsGeneratingInfo {
            get => _elementsGeneratingInfo;
            set => _elementsGeneratingInfo = value;
        }

        [SerializeField] ElementsPositioningInfo _elementsPositioningInfo;
        protected ElementsPositioningInfo elementsPositioningInfo {
            get => _elementsPositioningInfo;
            set => _elementsPositioningInfo = value;
        }

        [Header("DBs")]
        [SerializeField] GameInfoDB _gameInfoDB;
        protected GameInfoDB gameInfoDB {
            get => _gameInfoDB;
            set => _gameInfoDB = value;
        }



        // Implement the methods from "IDBCollectionReceiver"
        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB ?? _gameInfoDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB;
        }



        protected virtual void Awake () {
            _statesController = new StatesController(this, new State[] {
                new State("Idle", StateIdle),
                new State("Spinning", StateSpinning)
            } );
            _statesController.Run("Idle");


            ReelController[] reelControllers = GetAllReelControllers();

            for (int i = 0 ; i < reelControllers.Length ; i++) {
                reelControllers[i].ReelIndex = i;
            }
        }

        protected virtual void OnEnable () {
            foreach (ReelController reelController in GetAllReelControllers()) {
                reelController.SpinningStarted += OnReelSpinningStart;
                reelController.StoppingStarted += OnReelStoppingStart;
                reelController.Stopped += OnReelStop;
                reelController.TargetIconsShowed += OnReelTargetIconsShowed;
                reelController.WaitingForBonusIconEffectPlayed += OnWaitingForBonusIconEffectPlay;
            }
        }

        protected virtual void OnDisable () {
            foreach (ReelController reelController in GetAllReelControllers()) {
                reelController.SpinningStarted -= OnReelSpinningStart;
                reelController.StoppingStarted -= OnReelStoppingStart;
                reelController.Stopped -= OnReelStop;
                reelController.TargetIconsShowed -= OnReelTargetIconsShowed;
                reelController.WaitingForBonusIconEffectPlayed -= OnWaitingForBonusIconEffectPlay;
            }
        }

        protected virtual void Start () {
            if (_gameInfoDB == null) {
                Debug.LogError($"{gameObject.name}: Missing \"Game Info DB\"");
            }
        }


        // == Public Methods ==
        public virtual void ResetIcons (ReelController.GameModeType gameModeType) {
            ReelController[] reelControllers = GetAllReelControllers();
            foreach (ReelController reelController in reelControllers) {
                reelController.ResetWithRandomIcons(gameModeType);
            }
        }

        public virtual void SetShowedIconsId (int[,] showedIconsId) {
            for (int i = 0 ; i < showedIconsId.GetLength(0) ; i++) {
                if (i < transform.childCount) {

                    int[] showedIconsIdOnReel = new int[showedIconsId.GetLength(1)];

                    for (int j = 0 ; j < showedIconsId.GetLength(1) ; j++) {
                        showedIconsIdOnReel[j] = showedIconsId[i, j];
                    }

                    transform.GetChild(i).gameObject.GetComponent<ReelController>().SetShowedIconsId(showedIconsIdOnReel);
                }
            }
        }

        public virtual void Spin (ReelController.GameModeType gameModeType, bool willIgnoreWaitingForBonusIconEffect = false) {
            _statesParameters.isStartingSpin = true;
            _statesParameters.spinningGameModeType = gameModeType;
            _statesParameters.willIgnoreWaitingForBonusIconEffect = willIgnoreWaitingForBonusIconEffect;
        }

        public virtual void StopReel (int reelIndex, bool isFastSpeed = false) {
            ReelController reelController = GetReelController(reelIndex);
            if (reelController != null) {
                float speedRate = isFastSpeed ? _spinningInfo.spinFastSpeedRate : 1.5f;
                reelController.StopSpinningManually(speedRate);
            }
        }

        public virtual void StopAllReels (bool isFastSpeed = false) {
            ReelController[] reelControllers = GetAllReelControllers();
            foreach (ReelController reelController in reelControllers) {
                float speedRate = isFastSpeed ? _spinningInfo.spinFastSpeedRate : -1f;
                reelController.StopSpinningManually(speedRate);
                _statesParameters.isForceStop = true;
            }
        }

        public virtual void SetTargetIconsId (int[,] iconsId) {

            int bonusIconsCounter = 0;

            for (int i = 0 ; i < transform.childCount ; i++) {

                int[] targetIconsId = new int[iconsId.GetLength(1)];
                for (int j = 0 ; j < targetIconsId.Length ; j++) {
                    targetIconsId[j] = iconsId[i, j];
                }

                ReelController reelController = GetReelController(i);

                reelController.TargetIconsId = targetIconsId;

                if (bonusIconsCounter >= REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING - 1) {
                    reelController.SetIsWaitingWaitingForBonusIconEffect();
                }

                if (Array.Exists(targetIconsId, id => id == BonusIconId)) {
                    bonusIconsCounter++;
                }
            }

            _statesParameters.isTargetIconIdSet = true;
        }



        // == Private Methods ==
        // --- States ---
        protected virtual IEnumerator StateIdle (StatesController attachedStatesController) {
            _statesParameters.isStartingSpin = false;

            yield return new WaitUntil(() => _statesParameters.isStartingSpin);

            attachedStatesController.nextStateName = "Spinning";
        }

        protected virtual IEnumerator StateSpinning (StatesController attachedStatesController) {
            _statesParameters.reelSpinningStartedCount = 0;
            _statesParameters.reelSpinningStoppedCount = 0;
            _statesParameters.bonusIconAppearedCoordList.Clear();
            _statesParameters.isWaitingForBonusIconEffectPlayed = false;

            foreach (ReelController reelController in GetAllReelControllers()) {
                if (reelController.gameObject.activeSelf) {
                    reelController.Spin(_statesParameters.spinningGameModeType, _spinningInfo.spinSpeedRegular);
                }
            }

            float startTime = Time.time;
            while (!_statesParameters.isForceStop) {
                if (Time.time - startTime > _spinningInfo.minWaitingForStopDuration) {
                    break;
                }
                yield return null;
            }

            yield return new WaitUntil(() => _statesParameters.isTargetIconIdSet);


            foreach (ReelController reelController in GetAllReelControllers()) {
                if (reelController.gameObject.activeSelf) {
                    reelController.StopSpinningWithDelay();
                }
            }

            yield return new WaitUntil(() => _statesParameters.reelSpinningStartedCount == transform.childCount);


            float elapsedTime = 0f;

            // Waiting for all reels have stopped.
            while (true) {

                elapsedTime += Time.deltaTime;

                bool isAllReelsStopped = _statesParameters.reelSpinningStoppedCount >= transform.childCount;
                //Debug.Log("isAllReelsStopped" + isAllReelsStopped);
                if (elapsedTime >= 7f && !isAllReelsStopped)
                {
                    //StopAllReels();
                    Debug.Log("Threshold exceeded and reels have not stopped");
                    
                    GameStatesManager.IsbonusGame = false;
                    float AddNextReelDelay = 0;
                    foreach (Transform child in transform)
                    {
                        ReelController reelController = child.gameObject.GetComponent<ReelController>();
                        int reelIndex = reelController.ReelIndex;

                        //Debug.Log($"reelIndex: {reelIndex}, reelSpinStoppedCount: {_statesParameters.reelSpinningStoppedCount}");
                        if (reelIndex >= _statesParameters.reelSpinningStoppedCount)
                        {
                            //Debug.Log($"reelAmount: {_gameInfoDB.reelsAmount}, waitingForBonusReel: {_gameInfoDB.waitingForBonusReel[reelIndex]}");
                            if (_gameInfoDB.waitingForBonusReel.Length == _gameInfoDB.reelsAmount && !_gameInfoDB.waitingForBonusReel[reelIndex])
                            {
                                reelController.StopWaitingWaitingForBonusIconEffect();
                                yield return new WaitUntil(() => _statesParameters.reelSpinningStoppedCount > reelIndex);
                                continue;
                            }
                            else
                            {
                                reelController.ReadyToPlayWaitForBonusIconEffect(_spinningInfo.spinSlowSpeedRate, _spinningInfo.waitingForBonusIconEffectAdditionalDelay + AddNextReelDelay);
                                yield return new WaitUntil(() => _statesParameters.reelSpinningStoppedCount > reelIndex);
                                AddNextReelDelay += _spinningInfo.waitingForBonusIconEffectAdditionalDelay;
                            }
                        }
                    }
                    _statesParameters.isWaitingForBonusIconEffectPlayed = true;
                    if (_statesParameters.reelSpinningStoppedCount >= transform.childCount - 1)
                    {
                        StopWaitingAllWaitingForBonusIconEffect();
                    }
                    break;
                }

                if (!isAllReelsStopped && !_statesParameters.willIgnoreWaitingForBonusIconEffect && !_statesParameters.isWaitingForBonusIconEffectPlayed && _statesParameters.bonusIconAppearedCoordList.Count >= REQUIRED_ICON_AMOUNT_FOR_BONUS_OCCURRING - 1) {
                    // Waiting for bonus icon effect occurred
                    WaitingForBonusIconOccurred?.Invoke(this, new WaitingForBonusIconOccurredEventArgs{
                        appearedBonusIconCoords = _statesParameters.bonusIconAppearedCoordList.ToArray()
                    });
                    //公版滾輪慢速不會依序慢速，會一起計時停止時間
                    float AddNextReelDelay = 0;
                    foreach (Transform child in transform) {
                        ReelController reelController = child.gameObject.GetComponent<ReelController>();
                        int reelIndex = reelController.ReelIndex;

                        //Debug.Log($"reelIndex: {reelIndex}, reelSpinStoppedCount: {_statesParameters.reelSpinningStoppedCount}");
                        if (reelIndex >= _statesParameters.reelSpinningStoppedCount) {
                            //Debug.Log($"reelAmount: {_gameInfoDB.reelsAmount}, waitingForBonusReel: {_gameInfoDB.waitingForBonusReel[reelIndex]}");
                            if (_gameInfoDB.waitingForBonusReel.Length == _gameInfoDB.reelsAmount && !_gameInfoDB.waitingForBonusReel[reelIndex])
                            {
                                reelController.StopWaitingWaitingForBonusIconEffect();
                                yield return new WaitUntil(() => _statesParameters.reelSpinningStoppedCount > reelIndex);
                                continue;
                            }
                            else
                            {
                                reelController.ReadyToPlayWaitForBonusIconEffect(_spinningInfo.spinSlowSpeedRate, _spinningInfo.waitingForBonusIconEffectAdditionalDelay + AddNextReelDelay);
                                yield return new WaitUntil(() => _statesParameters.reelSpinningStoppedCount > reelIndex);
                                AddNextReelDelay += _spinningInfo.waitingForBonusIconEffectAdditionalDelay;
                            }    
                        }                        
                    }
                    _statesParameters.isWaitingForBonusIconEffectPlayed = true;

                }
                else if (_statesParameters.reelSpinningStoppedCount >= transform.childCount - 1) {
                    StopWaitingAllWaitingForBonusIconEffect();
                }

                if (isAllReelsStopped) {
                    break;
                }

                yield return null;
            }


            // Invoke Event
            if (ReelsStopped != null) {
                ReelsStopped(this, EventArgs.Empty);
            }

            _statesParameters.isForceStop = false;

            attachedStatesController.nextStateName = "Idle";
        }
        // --- States_end ---


        protected virtual ReelController GetReelController (int index) {
            if (index < transform.childCount) {
                return transform.GetChild(index).gameObject.GetComponent<ReelController>();
            }
            return null;
        }

        protected virtual ReelController[] GetAllReelControllers () {
            ReelController[] result = new ReelController[transform.childCount];
            for (int i = 0 ; i < result.Length ; i++) {
                result[i] = GetReelController(i);
            }
            return result;
        }

        protected virtual void StopWaitingAllWaitingForBonusIconEffect () {
            foreach (ReelController reelController in GetAllReelControllers()) {
                reelController.StopWaitingWaitingForBonusIconEffect();
            }
        }


        // == OnEvent ==
        protected virtual void OnReelSpinningStart (object sender, ReelController.ReelEventArgs args) {
            _statesParameters.reelSpinningStartedCount++;

            ReelSpinningStarted?.Invoke(this, args);
        }

        protected virtual void OnReelStoppingStart (object sender, ReelController.StoppingStartedEventArgs args) {
            ReelStoppingStarted?.Invoke(this, args);
        }

        protected virtual void OnReelStop (object sender, ReelController.ReelEventArgs args) {
            _statesParameters.reelSpinningStoppedCount++;

            ReelStopped?.Invoke(this, args);
        }

        protected virtual void OnReelTargetIconsShowed (object sender, ReelController.TargetIconsShowedEventArgs args) {
            for (int i = 0 ; i < args.targetIconsId.Length ; i++) {
                if (args.targetIconsId[i] == BonusIconId) {
                    _statesParameters.bonusIconAppearedCoordList.Add(new Vector2Int(args.reelIndex, i));
                }
            }

            ReelTargetIconsShowed?.Invoke(this, args);
        }

        protected virtual void OnWaitingForBonusIconEffectPlay (object sender, ReelController.WaitingForBonusIconEffectPlayedEventArgs args) {
            ReelWaitingForBonusIconEffectPlayed?.Invoke(this, args);
        }



        // == Nested Classes ==
        public class WaitingForBonusIconOccurredEventArgs : EventArgs {
            public Vector2Int[] appearedBonusIconCoords;
        }

        public class StopAllReelsEventArgs {
            public bool isFastSpeed;
        }

        [Serializable]
        public class ElementsGeneratingInfo {
            public GameObject reelPrefab;
            public GameObject iconCarrierPrefab;
            [Range(0, 10)]
            public int reelsAmount;
            [Range(0, 10)]
            public int showedIconsAmountPerReel;
        }

        [Serializable]
        public class ElementsPositioningInfo {
            [Range(0f, 10f)]
            public float intervalDistanceBetweenReels = 1f;
            [Range(0f, 10f)]
            public float intervalDistanceBetweenIconCarriers = 1f;
        }


        [Serializable]
        protected class SpinningInfo {
            public float spinSpeedRegular = 2f;
            public float spinFastSpeedRate = 0.5f;
            public float spinSlowSpeedRate = 0.5f;
            [Tooltip("注意：如果調太低會使第一輪將對較慢停下")]
            public float minWaitingForStopDuration = 0.3f;
            public float waitingForBonusIconEffectAdditionalDelay = 2f;
        }


        protected class StatesParameters {
            public bool isStartingSpin = false;
            public ReelController.GameModeType spinningGameModeType = ReelController.GameModeType.BaseGame;
            public bool willIgnoreWaitingForBonusIconEffect = false;

            public bool isTargetIconIdSet = false;
            public bool isForceStop = false;
            public int reelSpinningStartedCount = 0;
            public int reelSpinningStoppedCount = 0;
            public List<Vector2Int> bonusIconAppearedCoordList = new List<Vector2Int>();
            public bool isWaitingForBonusIconEffectPlayed = false;
        }

    }

}
