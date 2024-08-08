using Games.SelectedMachine;
using LitJson;
using PlayFab;
using PlayFab.CloudScriptModels;
using SlotTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameStatUpdate : MonoBehaviour
{
    public int[] Spinstatus { set; get; }

    public int MachineId { set; get; }
    public List<string> ElementSet { set; get; }
    public WinRecord WinRecordState { set; get; }

    #region start singleton 
    private static GameStatUpdate instance;
    public static GameStatUpdate Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameStatUpdate>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(GameStatUpdate).Name);
                    instance = obj.AddComponent<GameStatUpdate>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance exists, destroy this one.
            Destroy(gameObject);
        }
    }
    #endregion end singleton

    private void Start()
    {
        ElementSet = new List<string>();
        MachineId = MachineLobbyMediator.Instance.SelectedMachineId;
        ElementSet.Add(MachineId.ToString());
        AzureUpdateGameState(0, ElementSet);
    }

    public void UpdateSpin(bool isFreeGame)
    {
        if (!isFreeGame)
        {
            if (Spinstatus[0] == -1)
            {
                Spinstatus[0] = Spinstatus[0]+2;
            }
            else
            {
                Spinstatus[0]++;
            }
        }
        else
        {
            Spinstatus[4] = Spinstatus[3];
            Spinstatus[3] = Spinstatus[2];
            Spinstatus[2] = Spinstatus[1];
            Spinstatus[1] = Spinstatus[0];
            Spinstatus[0] = 0;
        }
    }

    public void AzureUpdateGameState(int command, List<string> elements)
    {
        string elementsEncode = EncodeString(JsonMapper.ToJson(elements));

        //Start call Azure Function 
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GameDataUpdate",
            FunctionParameter = new Dictionary<string, object>()
                    {
                        {"Command", command},
                        {"Data", elementsEncode},
                    },
            GeneratePlayStreamEvent = false
        }, (ExecuteFunctionResult result) =>
        {
            string results = DecodeString(result.FunctionResult.ToString());
            
            switch(command)
            {
                case 0:
                    Spinstatus = JsonMapper.ToObject<int[]>(results);
                    break;
                case 1:
                    ClearStat();
                    break;
                case 2:
                    WinRecordState = JsonMapper.ToObject<WinRecord>(results);
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            ElementSet = new List<string>();
        }, (PlayFabError error) =>
        {
            Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        });
    }

    public void UpdateWinRecord(BigWinType winType, int machineId)
    {
        switch(winType)
        {
            case BigWinType.BigWin:
                WinRecordState.ThisMothBigWin += 1;
                break;
            case BigWinType.MegaWin:
                WinRecordState.ThisMonthMegaWin += 1;
                break;
            case BigWinType.SuperWin:
                WinRecordState.ThisMonthSuperWin += 1;
                break;
            case BigWinType.DragonWin:
                WinRecordState.ThisMonthDragonWin += 1;
                break;
        }

        ElementSet = new List<string>
        {
            machineId.ToString(),
            WinRecordState.ThisMothBigWin.ToString(),
            WinRecordState.ThisMonthMegaWin.ToString(),
            WinRecordState.ThisMonthSuperWin.ToString(),
            WinRecordState.ThisMonthDragonWin.ToString()
        };

        AzureUpdateGameState(4, ElementSet);
        Invoke("WaitAzureUpdateGameState", 2f);
    }
    private void WaitAzureUpdateGameState()
    {
        Instance.ElementSet = new List<string>
            {
                MachineLobbyMediator.Instance.SelectedMachineId.ToString()
            };
        Instance.AzureUpdateGameState(2, Instance.ElementSet);
    }

    private string EncodeString(string value)
    {
        byte[] textByte = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(textByte);
    }

    private string DecodeString(string value)
    {
        byte[] textByte = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(textByte);
    }

    public void ClearStat()
    {
        Array.Clear(Spinstatus, 0, Spinstatus.Length);
        for(int i = 0;i<Spinstatus.Length;i++)
        {
            if (Spinstatus[i] == 0)
            {
                Spinstatus[i] = -1;
            }
        }
    }

    public class WinRecord
    {
        public int LastMothBigWin { get; set; }
        public int LastMonthMegaWin { get; set; }
        public int LastMonthSuperWin { get; set; }
        public int LastMonthDragonWin { get; set; }
        public int ThisMothBigWin { get; set; }
        public int ThisMonthMegaWin { get; set; }
        public int ThisMonthSuperWin { get; set; }
        public int ThisMonthDragonWin { get; set; }

    }
}
