using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;

public class UndermaintenanceNotification : MonoBehaviour
{
    public GameObject UnderMaintennancePanel;
    public static bool isMaintenant = false;

    // Start is called before the first frame update
    void Start()
    {
        CheckMaintenance();
    }

    public void CheckMaintenance()
    {
        DateTime now = DateTime.Now;
        string dodo = "v_gfQv6gWJJ8REGOEMKWNo7GQrvXJuK_NE6UChTUDZg-AzFuB4vIDw==";
        var functionUrl = "https://slotmachinegame20231023142048.azurewebsites.net/api/MaintainanceNotification";

        var content = new StringContent(now.ToString(), Encoding.UTF8, "application/json");

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("x-functions-key", dodo);
            using (HttpResponseMessage response = client.PostAsync(functionUrl, content).Result)
            using (HttpContent respContent = response.Content)
            {
                // ... Read the response as a string.
                var result = respContent.ReadAsStringAsync().Result;

                try
                {
                    bool azureResponse = bool.Parse(result);
                    UnderMaintennanceNotify(azureResponse);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error: {ex.Message}");
                }

            }
        }
    }

    public void UnderMaintennanceNotify(bool underMaintennance)
    {
        if (underMaintennance)
        {
            UnderMaintennancePanel.SetActive(true);
            isMaintenant = true;
        }
        else
        {
            UnderMaintennancePanel.SetActive(false);
            isMaintenant = false;
        }
    }

    public void CloseGame()
    {
        Debug.Log("Go out the game");
        Application.Quit();
    }
}
