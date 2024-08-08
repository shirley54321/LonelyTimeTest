using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SlotTemplate
{
    public class CT_GameStatesManager : GameStatesManager
    {
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

                // Multiplier Number Display
                bool isTheMaxNumber;
                int multiplierNumber = _bonusGameInfoDB.GetMultiplierNumberByBonusRoundNumber(roundResultData.bonusGameInfo.roundNumber, out isTheMaxNumber);
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

                            statesParameters.pendingRoundDataWhenBonusGameTriggered = _currentRoundData;

                            //公版進bonus時只加了bonus基本分(沒有乘倍率)
                            statesParameters.accumulatedBonusWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate;
                            //statesParameters.accumulatedBonusWin = 0;
                            basedWin = RoundResultData.WonSituationInfo.GetTotalBaseWinOfWonSituations(_currentRoundData.resultData.WonBonusSituationsInfo) * statesParameters.lastestBetRate;
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
    }
}
