using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public void SetBasicInfo(string lobbyName, ushort machineId)
{
    CurrentMachineInfo.lobbyName = lobbyName;
    CurrentMachineInfo.machineId = machineId;
}*/
namespace SlotTemplate
{
    [DisallowMultipleComponent]
    public class MachineInfoManager : MonoBehaviour
    {
        public event EventHandler<EventArgs> MachineInfoChanged;


        public IMachineDetail CurrentMachineDetail => gameManager != null ? gameManager.CurrentMachineDetail : null;


        MachineInfo _currentMachineInfo = null;


        public MachineInfo CurrentMachineInfo
        {
            get
            {
                if (_currentMachineInfo == null)
                {
                    if (gameManager != null)
                    {
                        _currentMachineInfo = new MachineInfo();
                        UpdateAllInfo();
                    }
                }
                return _currentMachineInfo;
            }
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


        void OnEnable()
        {
            if (gameManager != null && gameManager.IsInited)
            {
                //gameManager.MachineDetailUpdated += OnMachineDetailUpdated;
                gameManager.gameStatesManager.RoundEnded += OnRoundEnded;
                gameManager.gameStatesManager.BigWinAnimationEnded += OnBigWinAnimationEnded;

                gameManager.FetchMachineDetail();
            }
            else
            {
                this.enabled = false;
            }
        }

        void OnDisable()
        {
            if (gameManager != null && gameManager.IsInited)
            {
                //gameManager.MachineDetailUpdated -= OnMachineDetailUpdated;
                gameManager.gameStatesManager.RoundEnded -= OnRoundEnded;
                gameManager.gameStatesManager.BigWinAnimationEnded -= OnBigWinAnimationEnded;
            }
        }



        public void SetBasicInfo(string lobbyName, ushort machineId)
        {
            CurrentMachineInfo.lobbyName = lobbyName;
            CurrentMachineInfo.machineId = machineId;
        }



        void UpdateAllInfo()
        {
            UpdateBonusGameTriggeringRoundsCosts();
            UpdateBigWinOccureedInfos();
        }


        void UpdateBonusGameTriggeringRoundsCosts()
        {
            IMachineDetail machineDetail = CurrentMachineDetail;

            /*CurrentMachineInfo.bonusGameTriggeringRoundsCostCounts = new uint[] {
                machineDetail.NGC,
                machineDetail.FirstLBNGC,
                machineDetail.SecondLBNGC,
                machineDetail.ThirdLBNGC
            };*/

            MachineInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        void UpdateBigWinOccureedInfos()
        {
            IMachineDetail machineDetail = CurrentMachineDetail;

            /*CurrentMachineInfo.bigWinOccurredInfos = new MachineInfo.BigWinOccurredInfo[] {
                new MachineInfo.BigWinOccurredInfo {
                    rateThreshold = 1000,
                    occurredTimesThisMonth = machineDetail.CurrentMO1000,
                    occurredTimesPreviousMonth = machineDetail.LastMO1000
                },
                new MachineInfo.BigWinOccurredInfo {
                    rateThreshold = 500,
                    occurredTimesThisMonth = machineDetail.CurrentMO500,
                    occurredTimesPreviousMonth = machineDetail.LastMO500
                },
                new MachineInfo.BigWinOccurredInfo {
                    rateThreshold = 300,
                    occurredTimesThisMonth = machineDetail.CurrentMO300,
                    occurredTimesPreviousMonth = machineDetail.LastMO300
                },
                new MachineInfo.BigWinOccurredInfo {
                    rateThreshold = 100,
                    occurredTimesThisMonth = machineDetail.CurrentMO100,
                    occurredTimesPreviousMonth = machineDetail.LastMO100
                }
            };*/

            MachineInfoChanged?.Invoke(this, EventArgs.Empty);
        }


        // == OnEvents ==
        void OnMachineDetailUpdated(IMachineDetail machineDetail)
        {
            UpdateBonusGameTriggeringRoundsCosts();
        }

        void OnBigWinAnimationEnded(object sender, EventArgs args)
        {
            UpdateBigWinOccureedInfos();
        }

        void OnRoundEnded(object sender, GameStatesManager.RoundEndedEventArgs args)
        {
            if (!args.isBonusRound || args.isFinalBonusRound)
            {
                gameManager.FetchMachineDetail();
            }
        }


        public class MachineInfo
        {
            public string lobbyName = "";
            public ushort machineId = 0;
            public uint[] bonusGameTriggeringRoundsCostCounts = null;
            public BigWinOccurredInfo[] bigWinOccurredInfos = null;

            public struct BigWinOccurredInfo
            {
                public int rateThreshold;
                public int occurredTimesThisMonth;
                public int occurredTimesPreviousMonth;
            }
        }
    }
}
