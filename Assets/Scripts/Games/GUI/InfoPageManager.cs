using System;
using UnityEngine;
using TMPro;
using Games.Data;
using Games.SelectedMachine;
using UnityEngine.UI;
using PlayFab;
using System.Collections.Generic;
using PlayFab.CloudScriptModels;
using LitJson;

namespace SlotTemplate
{

    public class InfoPageManager : MonoBehaviour
    {

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


        [SerializeField] Text lobbyNameTextContainer;
        [SerializeField] TextMeshProUGUI machineNumberTextContainer;
        [SerializeField] TextMeshProUGUI[] bonusGameTriggeringRoundsCostTextContainers;
        [SerializeField] BigWinOccurredDisplay[] bigWinOccurredDisplays;

        public static Games.Data.MachineInfo selectedMachine;
        public static Games.Data.GameId gameID;
        public static MachineInfoManager.MachineInfo.BigWinOccurredInfo[] bigWinArray = new MachineInfoManager.MachineInfo.BigWinOccurredInfo[4];

        public static InfoWinRecord infoWinRecords = new InfoWinRecord();

        private static bool isUpdate = true;


        //Old name is GameName but it prefer Jiggy [Has no Bug after change]. 
        //public string[] Jiggy = { "武媚娘", "阿拉丁", "楚河漢界", "舞師", "北極熊", "小丑把戲", "月狼", "埃及王朝", "龐貝" };

        public static void GetSelectedMachine(Games.Data.MachineInfo machineInfo, Games.Data.GameId gameId)
        {
            gameID = selectedMachine.GameId;
            if (machineInfo != null)
                Debug.Log("InfoPageManager: " + selectedMachine.MachineNumber);
            else
                Debug.Log("InfoPageManager: Not get data");
        }

        private void Start()
        {
            isUpdate = true;
            selectedMachine = MachineLobbyMediator.Instance.SelectedMachine;
        }

        void OnEnable()
        {
            RefreshMachineInfoPage();
        }

        void OnDisable()
        {
            if (selectedMachine != null)
            {
                if (gameManager.machineInfoManager != null)
                {
                    gameManager.machineInfoManager.MachineInfoChanged -= OnMachineInfoChanged;
                }
            }
        }

        public void winDataArrage(int thisMonth, int lastMonth, int rate, int index)
        {
            MachineInfoManager.MachineInfo.BigWinOccurredInfo temp = new MachineInfoManager.MachineInfo.BigWinOccurredInfo();
            temp.rateThreshold = rate;
            temp.occurredTimesThisMonth = thisMonth;
            temp.occurredTimesPreviousMonth = lastMonth;
            bigWinArray[index] = temp;
        }

        public void RefreshMachineInfoPage()
        {
            lobbyNameTextContainer.text = MachineLobbyMediator.Instance.GameProfile.GameName;
            machineNumberTextContainer.text = MachineLobbyMediator.Instance.SelectedMachine.MachineNumber.ToString();

            bonusGameTriggeringRoundsCostTextContainers[0].text = GameStatUpdate.Instance.Spinstatus[0] == -1 ? "-" :
                GameStatUpdate.Instance.Spinstatus[0].ToString();
            bonusGameTriggeringRoundsCostTextContainers[1].text = GameStatUpdate.Instance.Spinstatus[1] == -1 ? "-" :
                GameStatUpdate.Instance.Spinstatus[1].ToString();
            bonusGameTriggeringRoundsCostTextContainers[2].text = GameStatUpdate.Instance.Spinstatus[2] == -1 ? "-" :
                GameStatUpdate.Instance.Spinstatus[2].ToString();
            bonusGameTriggeringRoundsCostTextContainers[3].text = GameStatUpdate.Instance.Spinstatus[3] == -1 ? "-" :
                GameStatUpdate.Instance.Spinstatus[3].ToString();

            winDataArrage(GameStatUpdate.Instance.WinRecordState.ThisMothBigWin, GameStatUpdate.Instance.WinRecordState.LastMothBigWin, 100, 0);
            winDataArrage(GameStatUpdate.Instance.WinRecordState.ThisMonthMegaWin, GameStatUpdate.Instance.WinRecordState.LastMonthMegaWin, 300, 1);
            winDataArrage(GameStatUpdate.Instance.WinRecordState.ThisMonthSuperWin, GameStatUpdate.Instance.WinRecordState.LastMonthSuperWin, 500, 2);
            winDataArrage(GameStatUpdate.Instance.WinRecordState.ThisMonthDragonWin, GameStatUpdate.Instance.WinRecordState.LastMonthDragonWin , 1000, 3);

            foreach (MachineInfoManager.MachineInfo.BigWinOccurredInfo info in bigWinArray)
            {
                BigWinOccurredDisplay display = BigWinOccurredDisplay.GetByRateThresholdFromArray(bigWinOccurredDisplays, info.rateThreshold);
                if (display != null)
                {
                    display.occurredTimesThisMonthTextContainer.text = info.occurredTimesThisMonth == 0 ? "-" : $"{info.occurredTimesThisMonth}";
                    display.occurredTimesPreviousMonthTextContainer.text = info.occurredTimesPreviousMonth == 0 ? "-" : $"{info.occurredTimesPreviousMonth}";
                }
            }
        }


        void OnMachineInfoChanged(object sender, EventArgs args)
        {
            //MachineInfoRetrive();
            //RefreshMachineInfoPage();
        }

        public class InfoWinRecord
        {
            public int WithoutBonus { get; set; }
            public int FirstBonus { get; set; }
            public int SecondBonus { get; set; }
            public int ThirdBonus { get; set; }
            public int FourthBonus { get; set; }
            public int BigCount { get; set; }
            public int MegaCount { get; set; }
            public int SuperCount { get; set; }
            public int DragonCount { get; set; }
        }

        [Serializable]
        public class BigWinOccurredDisplay
        {
            public uint rateThreshold;
            public TextMeshProUGUI occurredTimesThisMonthTextContainer;
            public TextMeshProUGUI occurredTimesPreviousMonthTextContainer;

            public static BigWinOccurredDisplay GetByRateThresholdFromArray(BigWinOccurredDisplay[] displays, int rateThreshold)
            {
                foreach (var display in displays)
                {
                    if (display.rateThreshold == rateThreshold)
                    {
                        return display;
                    }
                }
                return null;
            }
        }

    }
}