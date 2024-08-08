using System;
using System.Collections;
using UnityEngine;

namespace SlotTemplate
{
    public class ED_GameStatesManager : GameStatesManager
    {
        [SerializeField] ED_BonusSpecialRules ED_bonusSpecialRules;
        [SerializeField] ED_NextRoundBonusConuts ED_nextRoundBonusConuts;
        bool Bonus = false;
        protected override IEnumerator StateRoundRunning(StatesController attachedStatesController)
        {

            statesParameters.gainedBonusRounds = 0;

            bool isSpinSucceed;

            if (!statesParameters.isInBonusGame)
            {
                // Base game
                isSpinSucceed = SpinBaseGame();
                Bonus = false;
            }
            else
            {
                Debug.Log("11111");
                // Bonus game
                yield return new WaitUntil(() => statesParameters.pendingBonusGameResultData.Count > 0);

                Debug.Log("22222");
                Bonus = true;
                statesParameters.hasBonusGameStartedPlaying = true;
                foreach(var data in statesParameters.pendingBonusGameResultData)
                {
                    Debug.Log("這裡是panding bonus game result data   \n\n" + data.ToString());
                }
                RoundResultData roundResultData = statesParameters.pendingBonusGameResultData[0];
                if (roundResultData.bonusGameInfo.totalRoundsCount > 10 && Bonus)
                {
                    roundResultData.bonusGameInfo.totalRoundsCount -= 1;
                    Debug.Log("33333");
                }
                else if (roundResultData.bonusGameInfo.totalRoundsCount == 10 && !Bonus)
                {
                    roundResultData.gainedBonusRounds = 0;
                    Bonus = true;
                    Debug.Log("44444");
                }
                Debug.Log("55555");
                Debug.Log("Bonus Game bonusGameInfo: " + roundResultData.bonusGameInfo.ToString());
                statesParameters.pendingBonusGameResultData.RemoveAt(0);

                //如果當前回合 = 最大回合 而且 isFinalRound == false
                if (roundResultData.bonusGameInfo.roundNumber == roundResultData.bonusGameInfo.totalRoundsCount && !roundResultData.bonusGameInfo.isFinalRound)
                {
                    Debug.Log("66666");
                    roundResultData.gainedBonusRounds = statesParameters.pendingBonusGameResultData[0].gainedBonusRounds;
                    statesParameters.pendingBonusGameResultData[0].bonusGameInfo.totalRoundsCount += roundResultData.gainedBonusRounds;

                    if (statesParameters.pendingBonusGameResultData[0].bonusGameInfo.isFinalRound)
                        roundResultData.bonusGameInfo.isFinalRound = true;

                    Debug.Log("Bonus Game bonusGameInfo: " + roundResultData.bonusGameInfo.ToString());
                }
                else
                {
                    Debug.Log("77777");
                    roundResultData.gainedBonusRounds = 0;
                }
                Debug.Log("88888");
                Debug.Log("gainround是" + roundResultData.gainedBonusRounds);
                ED_nextRoundBonusConuts.CalculateBonusQuantity(roundResultData);
                ED_bonusSpecialRules.RecordLockedReel(roundResultData.gainedBonusRounds);
                
                isSpinSucceed = SpinBonusGame(roundResultData);
                Debug.Log("isSpinSuccess 是   " +  isSpinSucceed);
                // Multiplier Number Display
                bool isTheMaxNumber;
                int multiplierNumber = _bonusGameInfoDB.GetMultiplierNumberByBonusRoundNumber(roundResultData.bonusGameInfo.roundNumber, out isTheMaxNumber);
                _slotMainBoard.animManager.ShowBonusGameMultiplier(multiplierNumber, isTheMaxNumber);
            }

            attachedStatesController.nextStateName = "ReadyForSpin";

            if (isSpinSucceed)
            {
                if (!statesParameters.isInBonusGame)
                {
                    // Base game
                    _guiDisplayManager.SetWinScoreDisplay(0);
                }

                yield return new WaitUntil(() => statesParameters.isSlotMainBoardSpinningEnded);

                // decimal roundWin = 5000;
                decimal roundWin = _currentRoundData.resultData.totalWin;

                if (!_currentRoundData.resultData.HasWon)
                {
                    statesParameters.isSlotMainBoardTotalWonAnimationEnd = true;
                }


                Coroutine winScoreRasing = null;
                bool isDelayForReadyForNextRound = false;

                Action<float> scoreRaisingLinkElaspedTimeRateAction = null;

                if (_currentRoundData != null)
                {
                    if (!statesParameters.isInBonusGame)
                    {
                        // Base Game
                        if (_currentRoundData.resultData.IsBonusOccurred)
                        {
                            // Bonus game triggered
                            statesParameters.wonBonusSituationsCount = _currentRoundData.resultData.WonBonusSituationsCount;
                            statesParameters.gainedBonusRounds = _currentRoundData.resultData.gainedBonusRounds;
                            if (statesParameters.pendingBonusGameResultData[0].bonusGameInfo.totalRoundsCount == 10)
                            {

                            }
                            //Lock滾筒
                            ED_bonusSpecialRules.RecordLockedReel(statesParameters.gainedBonusRounds);
                            //

                            statesParameters.pendingRoundDataWhenBonusGameTriggered = _currentRoundData;
                            //公版進bonus時只加了bonus基本分(沒有乘倍率)
                            //statesParameters.accumulatedBonusWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate;
                            statesParameters.accumulatedBonusWin = 0;

                            winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(
                                0,
                                statesParameters.accumulatedBonusWin / (decimal)_currentRoundData.resultData.WonBonusSituationsCount
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

                        }


                    }
                    else
                    {
                        // Bonus Game
                        decimal displayedWin = roundWin / statesParameters.wonBonusSituationsCount;  // bonus_game_round_win = base_round_win * won_bonus_lines_count
                        bool withCoinsHoppingEffect = displayedWin >= _coinsHoppingEffectTriggeredInBonusGameWinThreshold;
                        winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(displayedWin, false, withCoinsHoppingEffect);

                        if (_currentRoundData.resultData.HasWon)
                        {
                            isDelayForReadyForNextRound = true;
                        }

                        if (_currentRoundData.resultData.bonusGameInfo.isFinalRound)
                        {
                            // The final round of bonus game
                            attachedStatesController.nextStateName = "ExitingBonusGame";
                        }
                        else //if (_currentRoundData.resultData.IsBonusOccurred)
                        {

                            statesParameters.gainedBonusRounds = _currentRoundData.resultData.gainedBonusRounds;

                            if (!statesParameters.hasAchievedMaxBonusRounds)
                            {
                                statesParameters.hasAchievedMaxBonusRounds = _currentRoundData.resultData.bonusGameInfo.totalRoundsCount + statesParameters.gainedBonusRounds == _bonusGameInfoDB.maxBonusRounds;

                                if (statesParameters.hasAchievedMaxBonusRounds)
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
                        Coroutine waitingForAdvance = StartCoroutine(WaitingForAdvanceToDoAction(() => statesParameters.IsAdvancing, _guiDisplayManager.WinScoreRaisingJumpToTheEnd));
                        yield return winScoreRasing;
                        if (waitingForAdvance != null)
                        {
                            StopCoroutine(waitingForAdvance);
                        }
                    }

                    yield return new WaitUntil(() => statesParameters.isSlotMainBoardTotalWonAnimationEnd);

                    if (isDelayForReadyForNextRound)
                    {
                        yield return new WaitForSeconds(_readyForNextRoundDelayDurationWhenWin);
                    }
                }

                InvokeRoundEnded( new RoundEndedEventArgs
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


            if (!statesParameters.isInBonusGame)
            {
                _baseGameAutoplayingHandler.SetReadyToStandBy();
            }

            if(ED_bonusSpecialRules.isAllFakeReel[1] == true) { 
                ED_bonusSpecialRules.OpenAndCloseFakeReel();
            }

            ED_nextRoundBonusConuts.ShowBonusCount();

            statesParameters.isSlotMainBoardSpinningEnded = false;
            statesParameters.isSlotMainBoardTotalWonAnimationEnd = false;
            _currentRoundData = null;
        }

        protected override IEnumerator WaitForExitingBonusGameTransition(bool isAutoPlay)
        {

            InvokeBonusGameResultShowingStarted();
            //新增功能?
            InvokeBonusGameTotalWinsDisplay();

            Debug.LogWarning("bonus game WaitForExitingBonusGameTransition");
            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.BonusGameResultShowing,
                new BonusGameResultShowingAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    bonusWinPerBonusLine = statesParameters.BonusWinPerWonBonusLine,
                    bonusLineCount = statesParameters.wonBonusSituationsCount
                }
            );

            InvokeBonusGameResultShowingEnded();

            ED_nextRoundBonusConuts.gameObject.SetActive(false);

            //新增功能?
            InvokeEnteringBonusGameEnd();

            InvokeBonusGameTransitionOutStarted();



            // transition fade in
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeOut);
            ED_bonusSpecialRules.LeaveBonusGameunLockReel();


            _backgroundManager.ChangeBackground("BaseGame");
            statesParameters.isInBonusGame = false;

            _slotMainBoard.RestoreBaseGameRoundFromPending(statesParameters.BonusWinPerWonBonusLine);
            _baseGameAutoplayingHandler.ResumeAutoPlay();
            _bonusGamePlayingHandler.StopAutoPlay();



            // transition fade out
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeIn);

        }

        protected override IEnumerator WaitForEnteringBonusGameTransition(bool isAutoPlay, int gainedRounds)
        {

            InvokeBonusGameTransitionInStarted();
            //新增功能?
            InvokeEnteringBonusGameStart();

            // transition in
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeOut);

            _backgroundManager.ChangeBackground("BonusGame");
            statesParameters.isInBonusGame = true;

            _slotMainBoard.Reset(ReelController.GameModeType.BonusGame);
            _guiDisplayManager.SwitchToBonusGameShowing(gainedRounds);
            _bonusGamePlayingHandler.PrepareToStart();


            Debug.LogWarning("bonus game WaitForEnteringBonusGameTransition");
            // transition out
            yield return _guiDisplayManager.GetTransitionRunning(TransitionsManager.TransitionType.FadeIn);


            ED_nextRoundBonusConuts.CloseShake();

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.BonusGameTriggering,
                new BonusGameTriggeringAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    gainedRounds = gainedRounds
                }
            );

            ED_nextRoundBonusConuts.gameObject.SetActive(true);


            //新增撥放擴張百搭出現動畫動畫



            //新增開啟擴張百搭Icon
            ED_bonusSpecialRules.OpenAndCloseFakeReel();


            Debug.LogWarning("bonus game WaitForEnteringBonusGameTransition");
            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.ClickToStartBonusGameShowing,
                new SimpleWaitingForAdvanceBehaviour.Parameters
                {
                    autoAdvance = isAutoPlay
                },
                () => statesParameters.IsAdvancing
            );

            ED_nextRoundBonusConuts.OpenShake();
        }


        protected override IEnumerator WaitForGainMoreBonusRoundsTransition(bool isAutoPlay, int gainedRounds)
        {

            yield return _guiDisplayManager.GetTransitionRunning(
                TransitionsManager.TransitionType.GainMoreBonusRounds,
                new ED_GainMoreBonusRoundsAnimationBehaviour.Parameters
                {
                    autoAdvance = true,
                    gainedRounds = gainedRounds
                }
            );

        }
    }
}