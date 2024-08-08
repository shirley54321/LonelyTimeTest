using Newtonsoft.Json.Bson;
using Player;
using UnityEngine;
using Loading;
using System.Collections;
using System.Collections.Generic;

public class PanelOnen : BasePanel
{
    #region Instance(Singleton)
    private static PanelOnen instance;

    public static PanelOnen Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PanelOnen>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject with {typeof(PanelOnen)} does not exist in the scene, " +
                                   $"yet its method is being called.\n" +
                                   $"Please add {typeof(PanelOnen)} to the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (gameObject.name != "UICanvas") return;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject.transform.root);//全場可用
    }
    #endregion
    public void OpenSettingPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.SettingPanel);
    }

    public void OpenChatPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.ChatPanel);
        CloseChatPanel.IsChatPanelOpen = true;
    }

    public void OpenShopPanel()
    {
        // LoadingManger.Instance.Open_Loading_animator();
        // Debug.Log("Time.time:" + Time.time);
        // if (Time.time <= 5) //  PlayFab API 請求速率限制問題，不能讓玩家一開始就點商城
        // {
        //     Debug.Log(Time.time + "商城還沒準備好");
        //    return;
        //}
        // LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
        StartCoroutine(OpenShopPanel_COROTINE());
    }
    
    IEnumerator OpenShopPanel_COROTINE()
    {
        int i = 0;

        while(Time.time <= 6)
        {
            Debug.Log(Time.time + "商城還沒準備好");
            yield return new WaitForSeconds(0.5f);
            if(i == 0) LoadingManger.Instance.Open_Loading_animator();//第一次進入此迴圈時會開啟LOADING
            i++;
        }

        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
        UIManager.Instance.OpenPanel(UIConst.ShopPanel);
        SlotTemplate.PlayerStatus.Instance.UpdateVirtualCoin();
        InventoryManager.Instance.UpdateDragonCoin();
        yield return null;
    }


    public void openBankBookPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.BankBookPanel);
    }
    public void OpenVIPPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.VIPPanel);
    }


    public void OpenBagPagePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.BagPagePanel);
    }
    public void OpenClassificationGamePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.ClassificationPanel);
    }

    public void OpenGiftPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.GiftPanel);
        ChangeUiRedPoint.IsGiftRedPointbotton(false);
    }

    public void OpenFriendListPanel()
    {
        UIManager.Instance.OpenPanel(UIConst.FriendListPanel);
    }
    public void UnfinishClick()
    {

        InstanceRemindPanel.Instance.OpenPanel("敬請期待");
    }
}
