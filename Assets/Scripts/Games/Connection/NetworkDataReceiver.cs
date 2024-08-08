using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastDecimalDecoder;
using DG.Tweening.Core.Easing;
using System.Threading.Tasks;
using Games.SelectedMachine;
using System.IO;
using System.Linq;

namespace SlotTemplate
{
    public class NetworkDataReceiver : MonoBehaviour, IDBCollectionReceiver
    {
        /*==Events==*/
        public event EventHandler<RoundResultDataLoadedEventArgs> RoundResultDataLoaded;

        [Header("DBs")]
        [SerializeField] GameInfoDB _gameInfoDB;
        [SerializeField] BetRateDB _betRateDB;
        protected GameInfoDB gameInfoDB { get => _gameInfoDB; }
        protected BetRateDB betRateDB { get => _betRateDB; }

        public IConnectionHandler connectionHandler { get; protected set; }

        public int BonusIconId { get { return _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1; } }
        public int SwildIconId { get { return _gameInfoDB != null ? _gameInfoDB.wildIconId : -1; } }

        bool _isInited = false;
        protected bool IsInited { get => _isInited; set => _isInited = value; }

        // data from server of ("00" Play) format:
        //     [1 byte: Event ID] (0x00)
        //     [2 bytes: How many bonus rounds left]
        //     [? bytes: Icons ID showed on slot]
        //     [16 btyes: Total win]
        //     [19 bytes: Won lines info] * <won lines amount>
        //         [1 byte: Line index]
        //         [1 byte: Won icon ID on the line]
        //         [1 byte: <Won icon amount on the line> - 1]
        //         [16 bytes: Base win of the line]

        const int ushortDataBytesCount = 2;
        const int decimalDataBytesCount = 16;

        BonusGameInfoProcessing _bonusGameInfoProcessing;

        protected BonusGameInfoProcessing bonusGameInfoProcessing { get => _bonusGameInfoProcessing; set => _bonusGameInfoProcessing = value; }

        // Implement the methods from "IDBCollectionReceiver"
        public void OnDBsAssignedByDBCollection(GameDBManager.DBCollection dBCollection)
        {
            _gameInfoDB = dBCollection.gameInfoDB ?? _gameInfoDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty(GameDBManager.DBCollection dBCollection)
        {
            _gameInfoDB = dBCollection.gameInfoDB;
        }

        protected virtual void Awake()
        {
            if (!_isInited) { this.enabled = false; }
        }

        protected virtual void OnEnable()
        {
            if (connectionHandler != null)
                connectionHandler.DataReceived += OnDataReceived;
        }

        protected virtual void OnDisable()
        {
            if (connectionHandler != null)
                connectionHandler.DataReceived -= OnDataReceived;
        }

        protected virtual void Start()
        {
            if (gameInfoDB == null)
                Debug.LogError($"{gameObject.name}: Missing \"Game Info DB\"");
        }

        public virtual void Init(IConnectionHandler connectionHandler)
        {
            this.connectionHandler = connectionHandler;
            _isInited = true;

            this.enabled = false;
            this.enabled = true;
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            if (data[0] == 0)
            {
                RoundResultData roundResultData = ReadDataOfSequentialData(data);
                RoundResultDataLoaded?.Invoke(this, new RoundResultDataLoadedEventArgs { roundResultData = roundResultData });
            }
        }
        public static int NoResultBonusRoundsLeft = 11;
        int withIsbonusGameFalse = 0;
        bool NewBounsStart= true;
        protected virtual RoundResultData ReadDataOfSequentialData(byte[] data)
        {
            RoundResultData roundResultData = new RoundResultData();
            roundResultData.bonusIconId = BonusIconId;
            roundResultData.swildIconId = SwildIconId;
            int startIndex = 1;

            /*Read Bonus rounds left*/
            ushort bonusRoundsLeft = BitConverter.ToUInt16(data, startIndex);
            //Debug.Log($"Bonus Round Left: {bonusRoundsLeft}");

            startIndex += ushortDataBytesCount;

            /*Get into bonus game*/
            if ((bonusRoundsLeft != ushort.MaxValue || GameStatesManager.IsbonusGame)&& BitConverter.ToUInt16(data, 1)!=64250) //start round at 10
            {
                if(GameStatesManager.IsbonusGame)
                {
                    NoResultBonusRoundsLeft--;
                    bonusRoundsLeft = (ushort)NoResultBonusRoundsLeft;
                    if (BitConverter.ToUInt16(data, 1) != ushort.MaxValue&& BitConverter.ToUInt16(data, 1) <25 && NewBounsStart)
                    {
                        NoResultBonusRoundsLeft += BitConverter.ToUInt16(data, startIndex);
                        NewBounsStart = false;
                    }

                }
                if (_bonusGameInfoProcessing == null) // round 10 th
                {
                    withIsbonusGameFalse = NoResultBonusRoundsLeft;
                    Debug.Log($"Go to Bonus Game start: {string.Join(",", data)}");
                    GameStatUpdate.Instance.UpdateSpin(true);
                    roundResultData.gainedBonusRounds = bonusRoundsLeft;
                    _bonusGameInfoProcessing = new BonusGameInfoProcessing(bonusRoundsLeft);
                }
                else //round 9 to 0
                {

                    Debug.Log($"Go to Bonus Game continue: {string.Join(",", data)}");

                    /*Anothor Bonus Game round*/
                    _bonusGameInfoProcessing.currentRoundNumber++;
                    roundResultData.bonusGameInfo.roundNumber = _bonusGameInfoProcessing.currentRoundNumber;
                    Debug.Log(_bonusGameInfoProcessing.currentRoundNumber);
                    //Update Bonus total round when occure free game inside
                    _bonusGameInfoProcessing.currentTotalRounds += _bonusGameInfoProcessing.pendingAddedRounds;
                    roundResultData.bonusGameInfo.totalRoundsCount = _bonusGameInfoProcessing.currentTotalRounds;

                    _bonusGameInfoProcessing.pendingAddedRounds = bonusRoundsLeft - (_bonusGameInfoProcessing.previousRoundsLeft - 1);
                    roundResultData.gainedBonusRounds = _bonusGameInfoProcessing.pendingAddedRounds;

                    if (bonusRoundsLeft == 0)
                    {
                        roundResultData.bonusGameInfo.isFinalRound = true;
                       GameStatesManager.IsbonusGame = false;
                        
                    }
                    if (_bonusGameInfoProcessing.currentRoundNumber > withIsbonusGameFalse-3 && GameStatesManager.IsbonusGame)
                    {
                        GameStatesManager.IsbonusGame = false;
                        roundResultData.bonusGameInfo.isFinalRound = true;
                    }
                }
                _bonusGameInfoProcessing.previousRoundsLeft = bonusRoundsLeft;
                
                GameCach.Instance.NormalResult_WU.RemoveAt(0);
                ConnectionScript.Instance.BonusPlay(roundResultData.gainedBonusRounds);
            }
            else
            {
                NewBounsStart = true;
                if (_bonusGameInfoProcessing != null)
                    _bonusGameInfoProcessing = null;
                else
                {
                    Debug.Log("bonusGameInfoProcessing is empty now");
                }
                GameStatUpdate.Instance.UpdateSpin(false);
            }



            /*Start read iconId from result byte array according to DB*/
            int[,] showedIconsId = new int[_gameInfoDB.reelsAmount, _gameInfoDB.showedIconsAmountPerReel];

            for (int i = 0; i < showedIconsId.GetLength(0); i++)
            {
                for (int j = showedIconsId.GetLength(1) - 1; j >= 0; j--)
                {
                    showedIconsId[i, j] = (int)data[startIndex];
                    startIndex++;
                }
            }
            roundResultData.showedIconsId = showedIconsId;

            roundResultData.totalWin = DecimalDecoder.ToDecimal(data, startIndex) * PlayerStatus.Instance.BetRate;

            //Debug.Log("Total win need to show:" + roundResultData.totalWin);

            if (roundResultData.totalWin != 0 && bonusRoundsLeft == ushort.MaxValue)
                _ = StartCoroutine(PlayerStatus.Instance.Reward(roundResultData.totalWin));

            startIndex += decimalDataBytesCount;

            /*Checked the process of winning*/
            List<RoundResultData.WonSituationInfo> wonSituationsInfoList = new List<RoundResultData.WonSituationInfo>();

            while (startIndex < data.Length)
            {
                /*Read From Data Array*/
                int lineIndex = (int)data[startIndex++];
                int wonIconId = (int)data[startIndex++];
                int wonIconCount = (int)data[startIndex++] + 1;

                decimal baseWinScore = DecimalDecoder.ToDecimal(data, startIndex);

                RoundResultData.WonSituationInfo wonSituationInfo;
                int[] reelsIndexWithWonIconAppeared = new int[wonIconCount];

                if (lineIndex == byte.MaxValue && _gameInfoDB.bonusOccurredType == GameInfoDB.BonusOccurredType.AnyPlace)
                {
                    // Scattered-triggered icons occurred
                    showedIconsId = roundResultData.showedIconsId;
                    int counter = 0;

                    for (int i = 0; i < showedIconsId.GetLength(0); i++)
                    {
                        for (int j = 0; j < showedIconsId.GetLength(1); j++)
                        {
                            if (showedIconsId[i, j] == wonIconId)
                            {
                                reelsIndexWithWonIconAppeared[counter] = i;
                                counter++;

                                if (counter >= reelsIndexWithWonIconAppeared.Length)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    wonSituationInfo = new RoundResultData.ScatteredIconWinSituationInfo
                    {
                        wonIconId = wonIconId,
                        reelsIndexWithWonIconAppeared = reelsIndexWithWonIconAppeared,
                        baseWinScore = baseWinScore
                    };
                }
                else
                {
                    int firstReelIndex = 0;
                    if (wonIconId < _gameInfoDB.firstReelIndicesWhichContainsTheIcon.Length)
                    {
                        firstReelIndex = _gameInfoDB.firstReelIndicesWhichContainsTheIcon[wonIconId];
                    }

                    for (int i = 0; i < reelsIndexWithWonIconAppeared.Length; i++)
                    {
                        reelsIndexWithWonIconAppeared[i] = firstReelIndex + i;
                    }

                    wonSituationInfo = new RoundResultData.WonLineInfo
                    {
                        wonIconId = wonIconId,
                        reelsIndexWithWonIconAppeared = reelsIndexWithWonIconAppeared,
                        baseWinScore = baseWinScore,
                        lineIndex = lineIndex
                    };
                }

                wonSituationsInfoList.Add(wonSituationInfo);

                startIndex += decimalDataBytesCount;
            }

            roundResultData.wonSituationsInfo = wonSituationsInfoList.ToArray();

            RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo = null;
            if (_gameInfoDB.swildOccurredType == GameInfoDB.SwildOccurredType.hasSwild)
            {
                int swildIconId = _gameInfoDB.swildIconId;
                List<int> reelsIndexWithWonIconAppearedList = new List<int>();

                for (int i = 0; i < showedIconsId.GetLength(0); i++)
                {
                    for (int j = 0; j < showedIconsId.GetLength(1); j++)
                    {
                        if (showedIconsId[i, j] == swildIconId)
                        {
                            reelsIndexWithWonIconAppearedList.Add(i);
                            break;
                        }
                    }
                }
                if (reelsIndexWithWonIconAppearedList.Count > 0)
                {
                    swildIconWinSituationInfo = new RoundResultData.SwildIconWinSituationInfo
                    {
                        swildIconId = swildIconId,
                        reelsIndexWithWonIconAppeared = reelsIndexWithWonIconAppearedList.ToArray()
                    };
                }
            }
            roundResultData.swildIconWinSituationInfo = swildIconWinSituationInfo;
            return roundResultData;
        }

        //Setting Area
        public class RoundResultDataLoadedEventArgs : EventArgs
        {
            public RoundResultData roundResultData;
        }

        protected class BonusGameInfoProcessing
        {
            public ushort previousRoundsLeft = ushort.MaxValue;
            public int currentTotalRounds = 0;
            public int pendingAddedRounds = 0;
            public int currentRoundNumber = 0;

            public BonusGameInfoProcessing(int totalRounds)
            {
                currentTotalRounds = totalRounds;
            }
        }
    }
}


