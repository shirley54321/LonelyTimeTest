using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.Events;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;

public class NetworkDetection : MonoBehaviour
{
    public float CheckTime = 0f;
	public const int NotReachable = 0;                   // 沒有網路
	public const int ReachableViaLocalAreaNetwork = 1;   // 網路Wifi,網路線。
	public const int ReachableViaCarrierDataNetwork = 2; // 網路3G,4G。

    public  GameObject animationLoading;
    public  float loadingTime = .0f;
    private  bool onceAnimation = false;

	public float startLoadingTime = .0f;
	public bool checkStartStatus = true;
	public bool checkEndStatus = true;

	private bool serverErrorDetect = false;

    private void Awake()
    {
		DontDestroyOnLoad(gameObject);

		int SMM_num = 0;
		foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("ServerMessageManager"))
		{
			SMM_num++;
		}


		//Debug.Log("SMM_num" + SMM_num);

		if(SMM_num > 1) Destroy(gameObject);
	}
    // Use this for initialization
    void Start()
	{
		// IPhone, Android
		//int nStatus = 0;
		int nStatus = ConnectionStatus();
		Debug.Log("ConnectionStatus : " + nStatus);
		if (nStatus > 0)
			Debug.Log("有連線狀態");
		else
		{ 
			Debug.Log("無連線狀態");
			//TODO  跳提示信息
			//ServerEventHandler.CallClient_connection_error_event();
		}
        //HeartBeat();
        //this.StartCoroutine(PingConnect());
	}

    private void Update()
    {
        CheckTime += Time.unscaledDeltaTime;
        startLoadingTime += Time.deltaTime;

        if (onceAnimation)
        {
            loadingTime += Time.deltaTime;
        }
		//每三秒進行一次heartbeat，偵測到error後會彈出提示視窗，同時不會再不斷執行heartbeat，直到關掉提示視窗
		if (CheckTime >= 15f&&!serverErrorDetect)
		{
			CheckTime = 0f;
			//Debug.Log("heartbeat");
			HeartBeat();
		}

	}

    public int ConnectionStatus()
	{

		int nStatus;


		if (Application.internetReachability == NetworkReachability.NotReachable)
		
			nStatus = NotReachable;
			
		
		else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			nStatus = ReachableViaLocalAreaNetwork;
		else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			nStatus = ReachableViaCarrierDataNetwork;
		else
			nStatus = -1;

		if( nStatus <= 0) 
		{

			ServerEventHandler.CallClient_connection_error_event();
        }


		return nStatus;
	}
    /// <summary>
	/// 由於playfab沒有內建的heartbeat功能，所以向playfab要求時間做為heartbeat
	/// </summary>
	public void HeartBeat()
    {
		var request = new GetTimeRequest();
		PlayFabClientAPI.GetTime(request, result => 
			{
				//Debug.Log("成功連線"); 
				serverErrorDetect = false;
			},
			error => 
			{
				//Debug.Log("連線失敗");
				serverErrorDetect = true;
				ServerEventHandler.CallClient_connection_error_event(); 
			}
		);
    }
	/// <summary>
	/// 舊的heartbeat，可能因為android跟IOS不允許ping導致建置後似乎無法運作，現已不使用
	/// </summary>
	/// <returns></returns>
    IEnumerator PingConnect()
	{
		//Google IP
		string googleTW = "172.217.163.36";
		bool google_access = true;
		//YahooTW IP
		string yahooTW = "180.222.106.11";
		bool yahoo_access = true;
		//Ping網站
		Ping ping = new Ping(googleTW);

		int nTime = 0;

        while (!ping.isDone)
		{
			yield return new WaitForSeconds(0.1f);

			if (nTime > 20) // time 2 sec, OverTime
			{
				nTime = 0;
				Debug.Log("Google連線失敗 : " + ping.time);
				google_access = false;
			}
			nTime++;
			if (google_access == false) break;
		}
		yield return ping.time;
		//123456
		ping = new Ping(yahooTW);
		nTime = 0;
		while (!ping.isDone)
		{
			yield return new WaitForSeconds(0.1f);

			if (nTime > 20) // time 2 sec, OverTime
			{
				nTime = 0;
				Debug.Log("Yahoo連線失敗 : " + ping.time);
				yahoo_access = false;
			}
			nTime++;
			if (yahoo_access == false) break;
		}
		yield return ping.time;

		if (yahoo_access == false && google_access == false)
		{
            onceAnimation = true;
            animationLoading.SetActive(true);
            if (loadingTime > 1f)
            {
				animationLoading.SetActive(false);
                ServerEventHandler.CallClient_connection_error_event();
				loadingTime = 0f;
                onceAnimation = false;
            }
        } 
		else {
            animationLoading.SetActive(false);
            Debug.Log("連線成功"); 
		}
	}

}
