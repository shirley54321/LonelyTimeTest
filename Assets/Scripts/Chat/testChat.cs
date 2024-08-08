using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class testChat : MonoBehaviour
{
    [SerializeField] private GameObject myMsg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddMyMsgToContainer(string myMsgText)
    {
        var objList = myMsg.GetComponentsInChildren<TMP_Text>(); //�j���]�t�o�Ӫ�component�A�]�t�ۤv�A�Ѧ�https://blog.csdn.net/virus2014/article/details/52964159
        List<string> strings = new List<string> { $"{DateTime.Now.Hour}:{DateTime.Now.Minute}", myMsgText };
        for (int i = 0; i < 2; i++)
        {
            objList[i].text = strings[i];
        }
    }
}
