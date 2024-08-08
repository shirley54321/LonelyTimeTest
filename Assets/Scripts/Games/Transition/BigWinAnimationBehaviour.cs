using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Threading.Tasks;
using Games.SelectedMachine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlotTemplate {

    public class BigWinAnimationBehaviour : OneShotAnimationBehaviour, IDBCollectionReceiver {

        [SerializeField] bool _willSwitchTitleContinuously = false;
        [SerializeField] float _waitingDurationForAutoPlay = 1f;

        [Header("DBs")]
        [SerializeField] BigWinAnimationClipsDB _bigWinAnimationClipsDB;
        [SerializeField] BigWinWinRateStepsDB _bigWinWinRateStepsDB;
        [SerializeField] BigWinScoreRaisingDurationsDB _bigWinScoreRaisingDurationsDB;

        [Header("REFS")]
        [SerializeField] TextMeshProUGUI _winNumberTextContainer;
        [SerializeField] GameObject WinPanel;
        [SerializeField] Animator WinGraphicAnimation;
        [SerializeField] GameObject Confetti;
        [SerializeField] GameObject ShareButtonGroup;

        decimal _targetScore = 1000;
        decimal _bet = 1;
        bool _isAutoPlay = false;

        BigWinType _targetWinType = BigWinType.None;
        BigWinType _currentPlayingWinType = BigWinType.None;

        Action _reachedTargetScoreCallback = null;
        Action<float> _linkElapsedTimeRate = null;


        StatesParameters _statesParameters = new StatesParameters();
        Coroutine _currentAnimationSwitching = null;


        private void Update()
        {
            if (_winNumberTextContainer.text.Length > 13) _winNumberTextContainer.fontSize = 100;
            else _winNumberTextContainer.fontSize = 150;
        }

        // Implement the methods from "IDBCollectionReceiver"
        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _bigWinAnimationClipsDB = dbCollection.bigWinsAnimationClipsDB ?? _bigWinAnimationClipsDB;
            _bigWinWinRateStepsDB = dbCollection.bigWinWinRateStepsDB ?? _bigWinWinRateStepsDB;
            _bigWinScoreRaisingDurationsDB = dbCollection.bigWinScoreRaisingDurationsDB ?? _bigWinScoreRaisingDurationsDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _bigWinAnimationClipsDB = dbCollection.bigWinsAnimationClipsDB;
            _bigWinWinRateStepsDB = dbCollection.bigWinWinRateStepsDB;
            _bigWinScoreRaisingDurationsDB = dbCollection.bigWinScoreRaisingDurationsDB;
        }


        protected override void OnEnable () 
        {
            StartCoroutine(NumberRaising(_targetScore, _bet, CheckForCurrentNumberWinType, _linkElapsedTimeRate));
        }

        protected override void OnDisable () {
            _targetScore = 0;
            _bet = 1;
            _statesParameters = new StatesParameters();
        }

        public override void SetParameters (object objectParameters) {
            Parameters parameters = (Parameters) objectParameters;
            _targetScore = parameters.targetScore;
            _bet = parameters.bet;
            _isAutoPlay = parameters.isAutoPlay;
            _reachedTargetScoreCallback = parameters.reachedTargetScoreCallback;
            _linkElapsedTimeRate = parameters.linkElapsedTimeRateAction;
        }


        public override void Advance () 
        {
            if(!_statesParameters.hasReachedTargetScore)
                _statesParameters.isJumpingToTargetScore = true;
            Task task = BigWinDisplayOff();
        }



        void PlayAnimationOfType (BigWinType winType) 
        {
            float duration = 0f;
            duration = _bigWinScoreRaisingDurationsDB.GetDurationByType(winType);

            Debug.Log("Win Type: " + winType);

            //Save Win Type to DB
            int machineId = MachineLobbyMediator.Instance.SelectedMachineId;
            GameStatUpdate.Instance.UpdateWinRecord(winType, machineId);

            if (winType == BigWinType.None)
            {
                gameObject.SetActive(false);
            }   
            else
            {
                if(winType == BigWinType.DragonWin) { Confetti.SetActive(true); }
                WinPanel.SetActive(true);
                AnimatorOverrideController overrideController = new AnimatorOverrideController(WinGraphicAnimation.runtimeAnimatorController);
                overrideController["Win_Start"] = _bigWinAnimationClipsDB.GetAnimationClipByType(winType, BigWinAnimationClipsDB.StepType.In);
                overrideController["Win_Loop"] = _bigWinAnimationClipsDB.GetAnimationClipByType(winType, BigWinAnimationClipsDB.StepType.Loop);

                WinGraphicAnimation.runtimeAnimatorController = overrideController;

                StartCoroutine(WaitingBigWinEnd(overrideController, winType, duration));

                _currentPlayingWinType = winType;
            }
        }

        IEnumerator WaitingBigWinEnd(AnimatorOverrideController overrideController, BigWinType winType, float duration)
        {
            yield return new WaitForSeconds(duration);
            overrideController["Win_End"] = _bigWinAnimationClipsDB.GetAnimationClipByType(winType, BigWinAnimationClipsDB.StepType.Out);
            WinGraphicAnimation.SetFloat("WaitingForEnding", 6f);


            WinGraphicAnimation.runtimeAnimatorController = overrideController;

            yield return new WaitForSeconds(0.5f);

            if (winType == BigWinType.DragonWin) {
                Confetti.SetActive(false); }

            WinPanel.SetActive(false);
            gameObject.SetActive(false);

        }

        void CheckForCurrentNumberWinType (BigWinType winType)
        {
            Debug.Log("CheckForCurrentNumberWinType");
            PlayAnimationOfType(winType);
            _statesParameters.currentPlayingWinType = winType;

            //OpenShareButtonOrNot();//不知道分享按鈕是第幾階段地所以先關閉
        }

        /// <summary>
        /// 是Super或Dragon Win才會打開分享按鈕
        /// </summary>
        void OpenShareButtonOrNot()
        {
            switch(_statesParameters.currentPlayingWinType)
            {
                case BigWinType.SuperWin:
                    ShareButtonGroup.SetActive(true);
                    ShareButtonGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "SUPER WIN 分享";
                    break;
                case BigWinType.DragonWin:
                    ShareButtonGroup.SetActive(true);
                    ShareButtonGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "DRAGON WIN 分享";
                    break;
                default:
                    ShareButtonGroup.SetActive(false);
                    break;
            }
        }

        IEnumerator NumberRaising (decimal targetScore, decimal bet, Action<BigWinType> checkForCurrentNumberWinType = null, Action<float> linkElapsedTimeRateAction = null)
        {
            _targetWinType = _bigWinWinRateStepsDB.GetTargetType( (float) (targetScore / bet) );

            float duration = _bigWinScoreRaisingDurationsDB.GetDurationByType(_targetWinType);

            float startTime = Time.time;
            float elapsedTimeRate = 0f;


            if (_willSwitchTitleContinuously) {

                Debug.Log("Is switch title to continue");
                BigWinType initWinType = _bigWinWinRateStepsDB.GetLowestType();

                while (elapsedTimeRate < 1f) {

                    decimal currentNumber;

                    if (_statesParameters.isJumpingToTargetScore) {
                        elapsedTimeRate = 0.5f;
                    }
                    else {
                        elapsedTimeRate = (Time.time - startTime) / duration;
                    }

                    currentNumber = targetScore * (decimal) Mathf.Min(elapsedTimeRate, 1f);
                    if (_winNumberTextContainer != null) {
                        _winNumberTextContainer.text = currentNumber.ToString("0");
                    }

                    linkElapsedTimeRateAction?.Invoke(elapsedTimeRate);


                    float currentNumberWinRate = (float) (currentNumber / bet);

                    BigWinType currentWinType = _bigWinWinRateStepsDB.GetTargetType(currentNumberWinRate);
                    if (currentWinType == BigWinType.None) {
                        currentWinType = initWinType;
                    }

                    if (checkForCurrentNumberWinType != null) {
                        checkForCurrentNumberWinType(currentWinType);
                    }

                    yield return null;
                }
            }
            else {
                Debug.Log("NumberRaising");

                PlayAnimationOfType(_targetWinType);

                while (elapsedTimeRate < 1f) {

                    if (_statesParameters.isJumpingToTargetScore) {
                        elapsedTimeRate = 1f;
                    }
                    else {
                        elapsedTimeRate = (Time.time - startTime) / duration;
                    }

                    if (_winNumberTextContainer != null) {
                        _winNumberTextContainer.text = (targetScore * (decimal) Mathf.Min(elapsedTimeRate, 1f)).ToString("0");
                    }

                    linkElapsedTimeRateAction?.Invoke(elapsedTimeRate);

                    yield return null;
                }
            }

            OnReachedTargetScore();

        }

        IEnumerator WaitingToAutoPlay (float duration) {
            yield return new WaitForSeconds(duration);
            Advance();
        }

        IEnumerator GoOutBigWin()
        {
            float transitionDuration = 0.5f;
            float elapsedTime = 0f;
            Vector2 initialScale = transform.localScale;
            Vector2 targetScale = Vector2.zero;

            while (elapsedTime < transitionDuration)
            {
                float scaleDown = Mathf.Lerp(1f, 0f, elapsedTime / transitionDuration);
                transform.localScale = initialScale * scaleDown;
                elapsedTime += Time.deltaTime;
            }

            transform.localScale = targetScale;

            gameObject.SetActive(false);

            //Turn back scale
            float scale = 1f;
            transform.localScale = Vector2.Lerp(targetScale, initialScale, scale);
            yield return null;

            transform.localScale = initialScale;
        }
        public async Task BigWinDisplayOff() 
        {
            TaskCompletionSource<bool> buttonPressedTask;
            // 初始化 TaskCompletionSource
            buttonPressedTask = new TaskCompletionSource<bool>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                // 設置 TaskCompletionSource 的結果為 true
                buttonPressedTask.SetResult(true);
            });

            _winNumberTextContainer.text = _targetScore.ToString();
            //等待按下按鈕
            await buttonPressedTask.Task;
            StartCoroutine(GoOutBigWin());
        }
        async void OnReachedTargetScore () {
            _statesParameters.hasReachedTargetScore = true;
            _reachedTargetScoreCallback?.Invoke();

            if (_isAutoPlay) {
                StartCoroutine(WaitingToAutoPlay(_waitingDurationForAutoPlay));
            }

            await BigWinDisplayOff();
        }

        void OnAnimationEventReceived (object sender, AnimationEventReceiver.AnimationEventReceivedEventArgs args) {
            
            if (args.stringParameter == "BigWinAnimationEnded") {
                if ( _statesParameters.hasReachedTargetScore) {
                    gameObject.SetActive(false);
                }
                else {
                    _statesParameters.isAnimationEnded = true;
                }
            }
            /*_currentPlayingWinType == _targetWinType &&*/
        }


        public struct Parameters {
            public decimal targetScore;
            public decimal bet;
            public bool isAutoPlay;
            public Action reachedTargetScoreCallback;
            public Action<float> linkElapsedTimeRateAction;
        }

        public class StatesParameters {
            public bool isJumpingToTargetScore = false;
            public BigWinType currentPlayingWinType = BigWinType.None;
            public bool isAnimationEnded = false;
            public bool hasReachedTargetScore = false;
        }

    }

}
