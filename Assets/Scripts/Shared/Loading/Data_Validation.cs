using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Loading
{
    public class Data_Validation : MonoBehaviour
    {
        public string hash = "NULL";
        private string Nowsystem="NULL";
        public static bool canlogin = false;

        public static Data_Validation instance;
        public static Data_Validation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Data_Validation>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(Data_Validation)} does not exist in the scene, yet its method is being called.\n" +
                                        $"Please add {typeof(Data_Validation)} to the scene.");
                    }
                    DontDestroyOnLoad(instance);
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
                Destroy(gameObject);
            }
        }


        public async Task<string> GenerateSHA256Hash()
        {
            RuntimePlatform platform = Application.platform;
            string The_Path = Application.dataPath;
            byte[] combinedBytes = new byte[0];

            if(platform==RuntimePlatform.WindowsEditor)
            {
                Debug.Log("當前系統為WindowsEditor遊玩模式");
                The_Path=The_Path+"//Scripts";
                Nowsystem="Windows";
                if (Directory.Exists(The_Path)){
                    string[] files = Directory.GetFiles(The_Path, "*", SearchOption.AllDirectories);
                    int totalFiles = files.Length;
                    int currentFileIndex = 0;
                    
                    foreach (string filePath in files)
                    {
                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        combinedBytes = CombineArrays(combinedBytes, fileBytes);

                        currentFileIndex++;
                        }
                    hash = await CalculateSHA256Async(combinedBytes);
                    await checkdata(hash,Nowsystem);
                }
                else{
                    Debug.Log("找不到 The_Path :"+The_Path);
                }
            }
            else if(platform==RuntimePlatform.OSXEditor)
            {
                Debug.Log("當前系統為IOS編輯模式");
                canlogin = true;
            }            

            else if(platform==RuntimePlatform.Android)
            {
                Debug.Log("當前系統為Android遊玩模式");
                byte[] apkBytes = ReadFileBytes(The_Path);
                if (apkBytes != null && apkBytes.Length > 0)
                {
                    hash = await CalculateSHA256Async(apkBytes);
                    if (Application.installerName == "com.android.vending") {
                        Debug.Log("Application.installerName :"+Application.installerName);
                        Debug.Log("Application.installerMode :"+Application.installMode);
                        Debug.Log("遊戲透過Google Play Store安裝");
                        Nowsystem="Google";
                        await checkdata(hash,Nowsystem);
                    }           
                    else{
                        Debug.Log("Application.installerName :"+Application.installerName);
                        Debug.Log("Application.installerMode :"+Application.installMode);
                        Debug.Log("遊戲透過APK包安裝");
                        Nowsystem="APK";
                        await checkdata(hash,Nowsystem);                        
                    }         
                }
                else{
                    Debug.Log("apkBytes : 沒有找到檔案 不能計算hash");
                }
            }
            else if(platform==RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("當前系統為IOS遊玩模式");
                Nowsystem="IOS";
                if (Directory.Exists(The_Path)){
                    string[] files = Directory.GetFiles(The_Path, "*", SearchOption.AllDirectories);
                    int totalFiles = files.Length;
                    int currentFileIndex = 0;
                    
                    foreach (string filePath in files)
                    {
                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        combinedBytes = CombineArrays(combinedBytes, fileBytes);
                        currentFileIndex++;
                    }
                    hash = await CalculateSHA256Async(combinedBytes);
                    await checkdata(hash,Nowsystem);
                }
                else{
                    Debug.Log("找不到 The_Path :"+The_Path);
                }
            }
            else{
                Debug.Log("無法判斷當前系統是甚麼");
            }
            return hash;
        }
        private byte[] ReadFileBytes(string filePath)
        {
            byte[] fileBytes = null;
            try
            {
                fileBytes = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("讀不到APK檔案: " + e.Message);
            }
            return fileBytes;
        }
        public static byte[] CombineArrays(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }

        public async Task<string> CalculateSHA256Async(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(data));
                return BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();
            }
        }
        private async Task checkdata(string hash,string Nowsystem)
        {
            if (hash!="NULL" || Nowsystem!="NULL")
            {
                string baseUrlA = "https://playeronlinestatus.azurewebsites.net/api/Check_APP_DATA";
                string codeA = "WMLAjJZ5f2zVMH1tnbaGBhZD4NPGe-HikTxAdOlduJj8AzFuzmMO_Q==";
                string col1 = hash;
                string col2 = Nowsystem;
                string fullUrl = baseUrlA + "?code=" + codeA + "&param1=" + col1+ "&param2=" + col2;

                UnityWebRequest request = UnityWebRequest.Get(fullUrl);
                await Task.Yield();
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                Debug.Log("資料檢查完成");
                string responseText = request.downloadHandler.text;
                Debug.Log("responseText : "+responseText);
                if (responseText == "OK")
                {
                    canlogin = true;
                }
                else
                {
                    canlogin = false;
                }
            }
            else
            {
                Debug.Log("hash or Nowsystem=NULL");
                Debug.Log("hash : "+hash);
                Debug.Log("Nowsystem :"+Nowsystem);
            }
        }

    }
}
