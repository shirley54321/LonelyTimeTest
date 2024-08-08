using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRedPoint : MonoBehaviour
{

    public GameObject RedPointForChat;
    public GameObject RedPointForBag;
    public GameObject RedPointForGift;
    public GameObject RedPointForOption;
    public bool _RedPointForChat;
    public bool _RedPointForBag;
    public bool _RedPointForGift;
    public bool _RedPointForOption;

    private static UiRedPoint _instance;

    [SerializeField] private ChangeUiRedPoint _uiRedPoint;


    private void OnEnable()
    {
        if (_uiRedPoint == null)
        {
            // 在当前场景中查找具有 ChangeUiRedPoint 组件的对象
            _uiRedPoint = FindObjectOfType<ChangeUiRedPoint>();
        }

        if (_uiRedPoint != null)
        {
            //_uiRedPoint.UIRedPoint += OnUIRedPoint;
        }
        else
        {
            Debug.LogError("No ChangeUiRedPoint component found in the scene.");
        }
        ChangeUiRedPoint.AzureUpateNotOnline();
        //_uiRedPoint.UIRedPoint += OnUIRedPoint;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void OnUIRedPoint( string message)
    {
        switch (message)
        {
            case "chat1":
                _instance._RedPointForChat = true;
                break;
            case "chat0":
                _instance._RedPointForChat = false;
                break;
            case "Bag1":
                _instance._RedPointForBag=true;
                   break;
            case "Bag0":
                _instance._RedPointForBag=false;
                break;
            case "Gift1":
                _instance._RedPointForGift=true;
                break;
            case "Gift0":
                _instance._RedPointForGift=false;
                break;
            case "Option1":
                _instance._RedPointForOption=true;
                break;
            case "Option0":
               _instance._RedPointForOption=false;
                break;
        }

    }
    private void Update()
    {        
        if (!CloseChatPanel.IsChatPanelOpen)
        {
            RedPointForChat.SetActive(_RedPointForChat);
        }
        RedPointForBag.SetActive(_RedPointForBag);
        RedPointForGift.SetActive(_RedPointForGift);
        RedPointForOption.SetActive(_RedPointForOption);
    }
    private void OnDisable()
    {
       // _uiRedPoint.UIRedPoint -= OnUIRedPoint;
    }

   
}
