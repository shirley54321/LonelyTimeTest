using Games.SelectedMachine;
using LitJson;
using SlotTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCach : MonoBehaviour
{
    private List<byte[]> _normalResult_WU;
    public List<byte[]> NormalResult_WU
    { 
        get { return _normalResult_WU; } 
        set { _normalResult_WU = value; }
    }

    private List<byte[]> _buffResult_WU;
    public List<byte[]> BuffResult_WU
    { 
        get { return _buffResult_WU; } 
        set { _buffResult_WU = value; } 
    }

    //Use of singleton. 
    private static GameCach instance;

    public static GameCach Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameCach>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(GameCach).Name);
                    instance = obj.AddComponent<GameCach>();
                }
            }
            return instance;
        }
    }

    private bool _isCall = false;
    private void Awake()
    {
        // Ensure the singleton instance is set properly even if the object is created in the scene.
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

        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Scence Name is: " + currentScene);
        if (currentScene == "MachineLobby" && _isCall == false)
        {
            if (GameCach.Instance.NormalResult_WU.Count > 0 || GameCach.Instance.NormalResult_WU != null)
            {
                GameCach.Instance.NormalResult_WU.Clear();
            }

            _isCall = true;
            GameCach.Instance.GainMoreResult(0, (int)MachineLobbyMediator.Instance.SelectedGameId, GameCach.Instance.NormalResult_WU.Count);
        }
    }

    private void Start()
    {
        NormalResult_WU = new List<byte[]>();
        BuffResult_WU = new List<byte[]>();

        //Retrive Normal Data Result for WuMayNaing
        string jsonNormal = PlayerPrefs.GetString("NormalResult_WU");
        //NormalResult_WU = JsonMapper.ToObject<List<byte[]>>(jsonNormal);
        //Retrive Buff Result for WuMayNaing
        string jsonBuff = PlayerPrefs.GetString("BuffResult_WU");
        //BuffResult_WU = JsonMapper.ToObject<List<byte[]>>(jsonBuff);

        #region Debug field 
        NormalResult_WU.Clear();
        //NormalResult_WU.RemoveRange(0, 40);
        #endregion Debug field
    }

    public void GainMoreResult(int setting, int gameId, int amount)
    {
        //Call Azure Function here
        SendDataInfo sendDataInfo = new SendDataInfo();
        sendDataInfo.GameID = gameId;
        sendDataInfo.EDLockReelStatus = 1;
        sendDataInfo.IconCount = 1;
        sendDataInfo.BonusCount = -1;
        sendDataInfo.ResultAmount = amount;
        sendDataInfo.setting = setting;

        Debug.Log("Gain more result Working");
        new Task(() => { SlotGameFunction.SlotMachineGame(sendDataInfo); }).Start();
        //UpdateCatch();
    }

    public void UpdateCatch()
    {
        //WuMayNaing Normal
        string jsonResult = JsonMapper.ToJson(NormalResult_WU);
        PlayerPrefs.SetString("NormalResult_WU", jsonResult);

        //WuMayNaing Buff
        /*string jsonResultBuff = JsonMapper.ToJson(BuffResult_WU);
        PlayerPrefs.SetString("BuffResult_WU", jsonResultBuff);*/

        PlayerPrefs.Save();
    }

    public void CheckResultLeft()
    {
        string directoryPath = "C:/Users/user/Documents/K2Longlaitan_Client/Assets/Scripts/Games/Connection/Result";
        string fileName = "result.txt";

        try
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath)) File.Delete(filePath);

            using(StreamWriter sw = new StreamWriter(filePath))
            {
                int i = 0;
                sw.WriteLine($"Total Number is: {NormalResult_WU.Count}");
                foreach(byte[] item in NormalResult_WU)
                {
                    string byteString = JsonMapper.ToJson(item);
                    sw.WriteLine($"item number {i++} : {byteString}");
                }
            }

            Debug.Log("Success Storing file");
        }catch (Exception ex)
        {
            Debug.Log($"Error occurred while saving the byte arrays to file: {ex.Message}");
        }
    }
}
