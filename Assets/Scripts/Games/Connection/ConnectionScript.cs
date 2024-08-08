using System;
using System.Collections.Concurrent;
using UnityEngine;
using Games.SelectedMachine;
using Player;
using UnityEngine.Purchasing;
using Unity.VisualScripting;
using LitJson;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlotTemplate
{
    public class ConnectionScript : MonoBehaviour, IConnectionHandler
    {
        public event Action<byte[]> DataReceived;
        public event Action MachineEntered;
        public event Action MachineEnteringFailed;

        public BetRateDB BetRateDB;
        public IMachineDetail CurrentMachineDetail { get; private set; }
        public bool BonusGameDebug = true;
        public bool ResultDebug = true;

        #region Singleton
        /// <summary>
        /// 單例的儲存位置
        /// </summary>
        private static ConnectionScript mInstance;
        /// <summary>
        /// 單例的存取點
        /// </summary>
        public static ConnectionScript Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<ConnectionScript>();
                }
                return mInstance;
            }
        }
        #endregion

        void Awake() 
        {
            mInstance = this;
            MachineEntered?.Invoke();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                try
                {
                    FreeGameDebug();
                    Debug.Log("Finish preparing Free game");
                }
                catch (Exception e)
                {
                    Debug.Log($"{e.Message}");
                }

                BonusGameDebug = true;
            }
        }
        public void EnterMachine()
        {
            //Get Spin Record
            GameStatUpdate.Instance.ElementSet = new List<string>
            {
                MachineLobbyMediator.Instance.SelectedMachineId.ToString()
            };
            GameStatUpdate.Instance.AzureUpdateGameState(0, GameStatUpdate.Instance.ElementSet);

            //Get Win Record 
            GameStatUpdate.Instance.ElementSet = new List<string>
            {
                MachineLobbyMediator.Instance.SelectedMachineId.ToString()
            };
            GameStatUpdate.Instance.AzureUpdateGameState(2, GameStatUpdate.Instance.ElementSet);
        }

        public void InitGame()
        {
            FindAnyObjectByType<GameManager>().InitGame(this, "MAJAJA", 2430, "<PlayerName>", (decimal)1000, 1);
        }
        public void InitBonusGame()
        {
            FindAnyObjectByType<GameManager>().InitBonusGame(this, "MAJAJA", 2430, "<PlayerName>", (decimal)1000, 1);
        }

        public void MachinePlay(decimal bet, bool status)
        {
           
            /*if (GameCach.Instance.NormalResult_WU.Count != 0 && !ResultDebug)
            {
                GameCach.Instance.CheckResultLeft();
                ResultDebug = true;
            }

            if (BonusGameDebug)
            {
                try
                {
                    FreeGameDebug();
                    Debug.Log("Finish preparing Free game");
                }
                catch (Exception e)
                {
                    Debug.Log($"{e.Message}");
                }

                BonusGameDebug = false;
            }*/

            try
            {
                if(BitConverter.ToUInt16(GameCach.Instance.NormalResult_WU[0], 1)  != ushort.MaxValue)
                {
                    if (BitConverter.ToUInt16(GameCach.Instance.NormalResult_WU[1], 1) >30)
                    {
                        Debug.Log("假的bonus");
                        try
                        {
                            for (int i = 0; i < BitConverter.ToUInt16(GameCach.Instance.NormalResult_WU[0], 1) + 1; i++)
                            {
                                GameCach.Instance.NormalResult_WU[i][1] = (byte)((BitConverter.ToUInt16(GameCach.Instance.NormalResult_WU[0], 1) + 2) - i - 2); // 設置第 2 個位元組
                                GameCach.Instance.NormalResult_WU[i][2] = 0; // 設置第 3 個位元組
                                if (GameCach.Instance.NormalResult_WU[i][1] > 30)
                                {
                                    GameCach.Instance.NormalResult_WU[i][1] = 0;
                                }
                                Debug.Log("GameCach" + GameCach.Instance.NormalResult_WU[i][1] + GameCach.Instance.NormalResult_WU[i][2]);
                            }
                            //GameCach.Instance.NormalResult_WU[0][1] = 250; // 設置第 2 個位元組
                            //GameCach.Instance.NormalResult_WU[0][2] = 250; // 設置第 3 個位元組
                        }
                        catch (Exception e) { 
                            
                            Debug.Log(e);
                                                
                        }
                        
                    }
                }
                if(GameStatesManager.IsbonusGame)
                {
                    for (int i = 0; i < NetworkDataReceiver.NoResultBonusRoundsLeft; i++)
                    {
                        GameCach.Instance.NormalResult_WU[i][1] = (byte)(NetworkDataReceiver.NoResultBonusRoundsLeft -i - 2); // 設置第 2 個位元組
                        GameCach.Instance.NormalResult_WU[i][2] = 0; // 設置第 3 個位元組
                        if(GameCach.Instance.NormalResult_WU[i][1]<0)
                        {
                            GameCach.Instance.NormalResult_WU[i][1] = 0;
                        }
                    }
                    GameStatesManager.IsbonusGame = false;
                }
                byte[] data = GameCach.Instance.NormalResult_WU[0];

                if (data != null )
                {
                    DataReceived?.Invoke(data);
                }
                else
                {
                    Debug.Log("Data is null");
                }

                GameCach.Instance.NormalResult_WU.RemoveAt(0);
                //GameCach.Instance.UpdateCatch();
            }
            catch(Exception ex)
            {
                Debug.Log("connection script:" + ex.Message+"  "+ GameCach.Instance.NormalResult_WU.Count);
            }

            StartCoroutine(CheckResult((int)MachineLobbyMediator.Instance.SelectedGameId));
        }
        public void BonusPlay(int bonusround)
        {
            try
            {
                
                byte[] data = GameCach.Instance.NormalResult_WU[0];
                Debug.Log($"GameCach.Instance.NormalResult_WU: {string.Join(",", data)}");

                InitBonusGame();
                if (DataReceived != null)
                {
                    DataReceived?.Invoke(data);
                }
                else
                {
                    Debug.Log("DataReceived sub is null");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("connection script:" + ex.Message);
            }
        }

        public void CheckUpdate()
        {
            GameCach.Instance.NormalResult_WU.RemoveAt(0);
            //GameCach.Instance.UpdateCatch();
        }

        private void FreeGameDebug()
        {
            List<byte[]> bonusText = ReadBonusText();
            if (bonusText.Count > 0)
            {
                GameCach.Instance.NormalResult_WU = bonusText;
            }
        }

        private List<byte[]> ReadBonusText()
        {
            Debug.Log("Read Bonus"+ MachineLobbyMediator.Instance.SelectedGameId); 
            string path = $"C:/Users/User/Desktop/K2新/K2Longlaitan_Client-dev2.0 (8)/K2Longlaitan_Client-dev2.0/Assets/Scripts/Games/Connection/Result/FreeGame/" + MachineLobbyMediator.Instance.SelectedGameId +".txt";
            List<string> lines = File.ReadAllLines(path).ToList();
            List<byte[]> _bonusList = new List<byte[]>();

            try
            {
                foreach (string line in lines)
                {
                    string[] result = line.Split(":");
                    Debug.Log($"Result Range: {result.Length}");
                    string resultString = result[1].Trim();
                    byte[] byteResult = ParseByteArray(resultString);
                    _bonusList.Add(byteResult);
                    Debug.Log($"Check Bonus Result: {string.Join(",", byteResult)}");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("connection script Read file:" + ex.Message);
            }

            return _bonusList;
        }

        static byte[] ParseByteArray(string byteArrayString)
        {
            // Remove brackets and split by ","
            string[] byteStrings = byteArrayString.Trim('[',']').Split(',');

            // Parse each string to byte
            return byteStrings.Select(s => byte.Parse(s.Trim())).ToArray();
        }

        IEnumerator CheckResult(int gameId)
        {
            yield return null;

            if (GameCach.Instance.NormalResult_WU.Count <= 5)
                GameCach.Instance.GainMoreResult(0, gameId, GameCach.Instance.NormalResult_WU.Count);
        }
    }
}