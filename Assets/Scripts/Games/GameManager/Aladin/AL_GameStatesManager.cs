using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SlotTemplate
{
    public class AL_GameStatesManager : GameStatesManager
    {
        public float waitingForSwildTrigger = 0.5f;

        public GameObject elves;

        protected override IEnumerator StateRoundRunning(StatesController attachedStatesController)
        {
            statesParameters.gainedBonusRounds = 0;

            bool isSpinSucceed;

            if (!statesParameters.isInBonusGame)
            {
                // Base game
                isSpinSucceed = SpinBaseGame();
            }
            else
            {
                // Bonus game
                yield return new WaitUntil(() => statesParameters.pendingBonusGameResultData.Count > 0);

                statesParameters.hasBonusGameStartedPlaying = true;

                RoundResultData roundResultData = statesParameters.pendingBonusGameResultData[0];
                statesParameters.pendingBonusGameResultData.RemoveAt(0);

                isSpinSucceed = SpinBonusGame(roundResultData);

                _slotMainBoard.animManager.ShowBonusGameMultiplier();
                elves.SetActive(true);
            }

            attachedStatesController.nextStateName = "ReadyForSpin";

            if (isSpinSucceed)
            {
                if (!statesParameters.isInBonusGame)
                {
                    _guiDisplayManager.SetWinScoreDisplay(0);
                }
                yield return new WaitUntil(() => statesParameters.isSlotMainBoardSpinningEnded);

                //����swild�ʵe(�����S��)
                if (_currentRoundData.resultData.WonSwildOccurred)
                {
                    statesParameters.isSlotMainBoardGetSwildAnimationEnd = false;
                    yield return new WaitUntil(() => statesParameters.isSlotMainBoardGetSwildAnimationEnd);
                    statesParameters.accumulatedSwildMultiplier += _currentRoundData.resultData.swildIconWinSituationInfo.reelsIndexWithWonIconAppeared.Length;
                    bool isTheMaxNumber;
                    int addMmultiplierCount = _bonusGameInfoDB.GetMultiplierNumberByBonusRoundNumber(statesParameters.accumulatedSwildMultiplier, out isTheMaxNumber);
                    _slotMainBoard.animManager.ShowBonusGameMultiplier(addMmultiplierCount, isTheMaxNumber);
                    yield return new WaitForSeconds(waitingForSwildTrigger);
                }

                // The winning score per round
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
                        if (_currentRoundData.resultData.IsBonusOccurred)
                        {
                            // Bonus game triggered get into this just one time
                            statesParameters.wonBonusSituationsCount = _currentRoundData.resultData.WonBonusSituationsCount;
                            statesParameters.gainedBonusRounds = _currentRoundData.resultData.gainedBonusRounds;
                            statesParameters.pendingRoundDataWhenBonusGameTriggered = _currentRoundData;
                            
                            //�����ibonus�ɥu�[�Fbonus�򥻤�(�S�������v)
                            statesParameters.accumulatedBonusWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate;
                            basedWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate; ;
                            basedWin=0;
                            winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(
                                basedWin, false, false
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

                                _guiDisplayManager.SetWinScoreDisplay(roundWin);
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
                        decimal displayedWin = 0;

                        displayedWin = (roundWin / statesParameters.wonBonusSituationsCount);

                        // Parn Debug 2024 02 20
                        Debug.Log($"roundWin: {roundWin}, BonusWonSituationCount: {statesParameters.wonBonusSituationsCount}");

                        bool withCoinsHoppingEffect = displayedWin >= _coinsHoppingEffectTriggeredInBonusGameWinThreshold;
                        
                        winScoreRasing = _guiDisplayManager.PlayingWinScoreRaisingAnimation(displayedWin, false ,withCoinsHoppingEffect);

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

                InvokeRoundEnded(new RoundEndedEventArgs
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

            statesParameters.isSlotMainBoardSpinningEnded = false;
            statesParameters.isSlotMainBoardTotalWonAnimationEnd = false;
            _currentRoundData = null;
        }

        protected override IEnumerator StateExitingBonusGame(StatesController attachedStatesController)
        {
            Debug.Log("Exiting Bonus Game");

            statesParameters.hasAchievedMaxBonusRounds = false;
            _slotMainBoard.animManager.HideBonusGameMultiplier();
            elves.SetActive(false);
            yield return WaitForExitingBonusGameTransition(_baseGameAutoplayingHandler.IsActive);


            decimal totalBonusWin = statesParameters.accumulatedBonusWin;

            _ = StartCoroutine(PlayerStatus.Instance.Reward(totalBonusWin));

            RoundResultData pendingBaseRoundResultData = statesParameters.pendingRoundDataWhenBonusGameTriggered.resultData;
            
            //Total based win situation mean the bonus icon score 
            decimal totalRoundWin = totalBonusWin + RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(pendingBaseRoundResultData.WonNonBonusSituationsInfo) * pendingBaseRoundResultData.BetRate;
            
            if (_baseGameAutoplayingHandler.IsActive)
            {
                bool isContinue = _baseGameAutoplayingHandler.CheckForConitune(
                    new AutoplayHandler.StoppingConditionDeterminingData
                    {
                        estimatedWonRate = (float)(totalRoundWin / statesParameters.lastestBet),
                        estimatedBalanceAfterRoundEnded = _playerStatus.Balance + totalRoundWin,
                        isTriggeredBonusGame = false
                    },
                    true
                );
                yield return new WaitUntil(() => _baseGameAutoplayingHandler.IsActive == isContinue);  // Wait for _baseGameAutoplayingHandler to change state
            }


            Action<float> scoreRaisingLinkElaspedTimeRateAction = elapsedTimeRate => _playerStatus.AdditionalShowedBalance = decimal.Floor((decimal)Mathf.Lerp(0, (float)totalRoundWin, elapsedTimeRate));


            if (CheckForBigWinAnimation(totalBonusWin, statesParameters.lastestBet))
            {

                _guiDisplayManager.SetWinScoreDisplay(0);

                yield return BigWinAnimationPlaying(
                    totalRoundWin,
                    statesParameters.lastestBet,
                    _baseGameAutoplayingHandler.IsActive,
                    () => _playerStatus.ApplyAdditionalShowedBalanceToBalance(),
                    scoreRaisingLinkElaspedTimeRateAction
                );

                _guiDisplayManager.SetWinScoreDisplay(totalRoundWin);
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

            statesParameters.wonBonusSituationsCount = 0;
            statesParameters.accumulatedBonusWin = 0;
            statesParameters.accumulatedSwildMultiplier = 1;
            attachedStatesController.nextStateName = "ReadyForSpin";
        }
    }
    
}
