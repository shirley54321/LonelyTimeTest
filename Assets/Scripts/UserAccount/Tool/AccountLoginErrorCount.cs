using LitJson;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveErrorCount
{
    public int ErrorCount { get; set; }
    public DateTime ErrorTime { get; set; }
}
public class AccountLoginErrorCount : MonoBehaviour
{

    public bool isOverCount=false;

    private int Count=0;
    private DateTime LastTimeError ;

#if UNITY_EDITOR
    private readonly string filePath = Application.dataPath+ "/ErrorCountFiles" + "/LoginErrorCount.json";//路徑
#else
    private readonly string filePath = Application.persistentDataPath + "/LoginErrorCount.json";//路徑
#endif
     public IEnumerator CheckTimes()
    {
        Debug.Log(filePath);
        bool isFin = false;
        isOverCount = false;
        if(!File.Exists(filePath))
        {
            isFin = true;
        }
        else
        {
            Load();
            TimeSpan DiffTime = DateTime.Now.Subtract(LastTimeError);

            if (DiffTime.Minutes > 5)
            {
                File.Delete(filePath);
                Count = 0;
                Debug.Log("json delete");
            }
            else
            {
                if(Count>=6)
                {
                    isOverCount = true;
                }
            }
            isFin = true;
        }

        yield return new WaitUntil(()=>isFin);
    }

    public void PlusCount()
    {
        Count++;
        Save();
    }
    void Save()
    {
        SaveErrorCount saveErrorCount = new SaveErrorCount();
        saveErrorCount.ErrorCount = Count;
        saveErrorCount.ErrorTime = DateTime.Now;
        string saveJsonStr = JsonMapper.ToJson(saveErrorCount);//利用JsonMapper將錯誤次數轉換成Json格式
        Debug.Log(saveJsonStr);

        //創建一個StreamWriter，呼叫其中的方法来將資料寫入到文件中
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);//寫入創建好的文件中,呼叫文件的讀寫函數
        sw.Close();//關閉StreamWriter
        if (File.Exists(filePath))
        {
            Debug.Log("Json保存成功！！！");
        }
        else
        {
            Debug.Log("Json文件保存失敗！！");
        }
        
    }

    void Load()
    {
        StreamReader streamReader = new StreamReader(filePath);//創建一個讀取的StreamWrite用来讀取文件的内容
        string jsonStr = streamReader.ReadToEnd();//將文件讀取到末端用字串進行儲存
        streamReader.Close();//讀取完畢後關閉
        SaveErrorCount ErrorCount = JsonMapper.ToObject<SaveErrorCount>(jsonStr);
        Count = ErrorCount.ErrorCount;
        LastTimeError = ErrorCount.ErrorTime;
        Debug.Log("讀取到的錯誤次數"+ErrorCount.ErrorCount);
    }
}
