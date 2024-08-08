using LitJson;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
namespace SlotTemplate
{
    public class SlotGameFunction : MonoBehaviour
    {
        //public TextMeshProUGUI inputText;

        private static List<byte[]> results;
        public  static bool isDone { get; set; }
        public static void SlotMachineGame(SendDataInfo inputData)
        {
            isDone = false;
            string jsonData = LitJson.JsonMapper.ToJson(inputData);

            string _slotCode = "z9pdDyDUHEyf7PDL7oWppOue0cT6E3GTd9p4J7lgBHWVAzFucMpGHA==";
            var slotUrl = "https://k2slotgamesfunction.azurewebsites.net/api/GameTrigger";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-functions-key", _slotCode);
                using (HttpResponseMessage response = client.PostAsync(slotUrl, content).Result)
                using(HttpContent respContent = response.Content)
                {
                    var sentString = respContent.ReadAsStringAsync().Result;
                    
                    try
                    {
                        string decryptedData = Decrypt(sentString, "WinlandGame24426");
                        List<List<int>> nestedList = JsonMapper.ToObject<List<List<int>>>(decryptedData);
                        List<byte[]> results = new List<byte[]>();
                        foreach (var innerList in nestedList)
                        {
                            results.Add(innerList.Select(i => (byte)i).ToArray());
                        }
                        if(inputData.setting == 0)
                        {
                            GameCach.Instance.NormalResult_WU.AddRange(results);
                            isDone = true;
                        }
                        else if(inputData.setting == 1)
                        {
                            GameCach.Instance.BuffResult_WU.AddRange(results);
                            isDone = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error from slotfunction is: "+e.Message);
                    }
                }
            }
        }

        public static string Decrypt(string input, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(input);

                using (var msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
