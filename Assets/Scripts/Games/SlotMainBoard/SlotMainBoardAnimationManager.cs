using System;
using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public class SlotMainBoardAnimationManager : MonoBehaviour, IDBCollectionReceiver {

        public event EventHandler<WonLineAnimationStartedEventArgs> WonLineAnimationStarted;
        public event EventHandler<WonLineAnimationEndedEventArgs> WonLineAnimationEnded;
        public event EventHandler<EventArgs> ClearedAllWinningAnimation;
        public event EventHandler<EventArgs> BonusGameMultiplierAnimatingStarted;

        public float TotalWonAnimationDuration => _totalWonAnimtaionDuration;

        public int BonusIconId => _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1;

        public static bool IsAnimationFinish = false;


        [SerializeField] float _totalWonAnimtaionDuration = 1f;
        [SerializeField] float _wonLinesShowCyclingIntervalDuration = 1f;
        [SerializeField] float _waitingForStopSwildAinmation = 1f;

        [Header("DBs")]
        [SerializeField] GameInfoDB _gameInfoDB;

        [Header("REFS")]
        [SerializeField] GameObject _winningAnimatingBackground;
        [SerializeField] LinesDisplayController _linesDisplayController;
        [SerializeField] ScaleAnimatedIconsController _scaleAnimatedIconsController;
        [SerializeField] AnimatedIconsController _animatedIconsController;
        [SerializeField] WaitingForBonusIconEffectsManager _waitingForBonusIconEffectsManager;
        [SerializeField] BonusGameMultiplierDisplayController _bonusGameMultiplierDisplayController;


        Coroutine _currentWinningAnimationWhenIdlePlaying;



        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB ?? _gameInfoDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _gameInfoDB = dbCollection.gameInfoDB;
        }


        void Start () {
            if (_gameInfoDB == null) {
                Debug.LogError($"{gameObject.name}: Missing \"Game Info DB\"");
            }
        }


        public void ClearAllWinningAnimations () {
            StopAllCoroutines();

            _winningAnimatingBackground.SetActive(false);
            _linesDisplayController.ClearLines();
            _scaleAnimatedIconsController.ClearScaleAnimatedIcons();
            _animatedIconsController.ClearAnimatedIcons();

            ClearedAllWinningAnimation?.Invoke(this, EventArgs.Empty);
        }

        public void PlayScaleAnimatedIcon (int[,] showedIconsId, Vector2Int[] coords) {
            _scaleAnimatedIconsController.Play(showedIconsId, coords);
        }
        public Coroutine StartGetSwildAnimationPlayingCoroutine(int[,] showedIconsId, RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo)
        {
            StopAllCoroutines();
            return StartCoroutine(GetSwildAinmationPlaying(showedIconsId, swildIconWinSituationInfo));
        }
        public Coroutine StartTotalWonAnimationPlayingCoroutine (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo) {
            StopAllCoroutines();
            return StartCoroutine(TotalWonAnimationPlaying(showedIconsId, wonSituationsInfo));
        }

        public void PlaySeparatedWinningAnimation (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo, decimal additionalWinPerWonBonusSituation = 0, int[] prioritizedIconsId = null) {
            StopAllCoroutines();
            StartCoroutine(SeparatedWinningAnimationPlaying(showedIconsId, wonSituationsInfo, additionalWinPerWonBonusSituation, prioritizedIconsId));
        }

        public void PlayWaitingForBonusEffect (int reelIndex, Transform reelTransform) {
            _waitingForBonusIconEffectsManager.PlayAt(reelIndex, reelTransform);
        }

        public void StopWaitingForBonusEffect (int reelIndex) {
            _waitingForBonusIconEffectsManager.StopAt(reelIndex);
        }

        public void ShowBonusGameMultiplier()//NoAnimation
        {
            _bonusGameMultiplierDisplayController.Show();

            BonusGameMultiplierAnimatingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void ShowBonusGameMultiplier (int multiplierNumber, bool isTheMaxNumber = false) {
            
            _bonusGameMultiplierDisplayController.Show(multiplierNumber, isTheMaxNumber);

            BonusGameMultiplierAnimatingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void HideBonusGameMultiplier () {
            _bonusGameMultiplierDisplayController.Hide();
            //_bonusGameMultiplierDisplayController.Show(1, true);
        }
        IEnumerator GetSwildAinmationPlaying(int[,] showedIconsId, RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo)
        {
            _animatedIconsController.PrepareAnimatedIcons(showedIconsId, swildIconWinSituationInfo);
            yield return new WaitForSeconds(_waitingForStopSwildAinmation);
        }
        IEnumerator TotalWonAnimationPlaying (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo) {

            if (wonSituationsInfo.Length > 0) {

                // -- Background --
                _winningAnimatingBackground.SetActive(true);

                // -- Lines --
                int[] linesIndex = RoundResultData.WonLineInfo.GetLinesIndexArray(RoundResultData.WonLineInfo.GetContainedWonLineInfos(wonSituationsInfo));

                if (!_linesDisplayController.IsPrepared) {
                    _linesDisplayController.PrepareLines(linesIndex);
                }

                _linesDisplayController.DisplayLines(linesIndex);

                // -- Animated Icons --
                if (!_animatedIconsController.IsPrepared) {
                    _animatedIconsController.PrepareAnimatedIcons(showedIconsId, wonSituationsInfo);
                }

                _animatedIconsController.DisplayAnimatedIconsOfWonSituations(showedIconsId, wonSituationsInfo);

                IsAnimationFinish = true;
                yield return new WaitForSeconds(_totalWonAnimtaionDuration);
                
            }
        }

        IEnumerator SeparatedWinningAnimationPlaying (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo, decimal additionalWinPerWonBonusSituation, int[] prioritizedIconsId = null) {

            if (wonSituationsInfo.Length > 0) {

                // -- Background --
                _winningAnimatingBackground.SetActive(true);

                // -- Prepare Lines --
                if (!_linesDisplayController.IsPrepared) {
                    _linesDisplayController.PrepareLines(RoundResultData.WonLineInfo.GetLinesIndexArray(RoundResultData.WonLineInfo.GetContainedWonLineInfos(wonSituationsInfo)));
                }
                // -- Prepare Animated Icons --
                if (!_animatedIconsController.IsPrepared) {
                    _animatedIconsController.PrepareAnimatedIcons(showedIconsId, wonSituationsInfo);
                }


                int playedLinesCount = 0;

                prioritizedIconsId ??= new int[0];
                for (int prioritizedIconIndex = 0 ; prioritizedIconIndex <= prioritizedIconsId.Length ; prioritizedIconIndex++) {

                    for (int i = 0 ; wonSituationsInfo.Length > 0 ; i = (i + 1) % wonSituationsInfo.Length) {

                        if (prioritizedIconIndex < prioritizedIconsId.Length) {
                            if (wonSituationsInfo[i].wonIconId != prioritizedIconsId[prioritizedIconIndex]) {
                                continue;
                            }
                        }
                        else {
                            if (Array.Exists(prioritizedIconsId, id => id == wonSituationsInfo[i].wonIconId)) {
                                continue;
                            }
                        }
                        decimal additionalWin = wonSituationsInfo[i].wonIconId == BonusIconId ? additionalWinPerWonBonusSituation : 0;

                        WonLineAnimationStarted?.Invoke(this, new WonLineAnimationStartedEventArgs{
                            wonIconId = wonSituationsInfo[i].wonIconId,
                            wonIconCount = wonSituationsInfo[i].WonIconCount,
                            baseWinScore = wonSituationsInfo[i].baseWinScore,
                            additionalWin = additionalWin
                        });

                        // -- Lines --
                        if (wonSituationsInfo[i] is RoundResultData.WonLineInfo)
                        {
                            _linesDisplayController.DisplayLines(new int[] { ((RoundResultData.WonLineInfo)wonSituationsInfo[i]).lineIndex });
                        }
                        else
                        {//原公版沒有此功能 在出現無須連線的bonus顯示會有上一條贏線ID出現
                            _linesDisplayController.DisplayLines(new int[] { ushort.MaxValue });
                        }
                        // -- Animated Icons --
                        _animatedIconsController.DisplayAnimatedIconsOfWonSituation(showedIconsId, wonSituationsInfo[i]);


                        playedLinesCount++;

                        yield return new WaitForSeconds(_wonLinesShowCyclingIntervalDuration);

                        WonLineAnimationEnded?.Invoke(this, new WonLineAnimationEndedEventArgs{
                            playedCount = playedLinesCount
                        });

                    }
                }
            }

        }
        // == Nested Classes ==
        public class WonLineAnimationStartedEventArgs : EventArgs {
            public int wonIconId;
            public int wonIconCount;
            public decimal baseWinScore;
            public decimal additionalWin;
        }

        public class WonLineAnimationEndedEventArgs : EventArgs {
            public int playedCount;
        }

    }
}
