using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftCenterScripts : MonoBehaviour
{
    public GiftCenter _GiftCenter;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void YesButton()
    {
        _GiftCenter.giftHistClasses[num].step = 2;
        StartCoroutine(_GiftCenter.WriteGiftCenter());
        _GiftCenter.AzureUpate(_GiftCenter.giftHistClasses[num].senderID, _GiftCenter.giftHistClasses[num].number, 2,num);
        _GiftCenter.RestGiftCenter(0);
    }
    public void NoButton()
    {
        _GiftCenter.giftHistClasses[num].step = 5;
        StartCoroutine(_GiftCenter.WriteGiftCenter());
        _GiftCenter.AzureUpate(_GiftCenter.giftHistClasses[num].senderID, _GiftCenter.giftHistClasses[num].number, 5, num);
        _GiftCenter.RestGiftCenter(0);
    }
    public void SMSCheckButton()//SMS verification completed
    {
        _GiftCenter.giftHistClasses[num].step = 3;
        StartCoroutine(_GiftCenter.WriteGiftCenter());
        _GiftCenter.AzureUpate(_GiftCenter.giftHistClasses[num].receiverID, _GiftCenter.giftHistClasses[num].number, 3,num);
        _GiftCenter.RestGiftCenter(0);
    }
    public void CheckButton()//領取完成Collection completed

    {
        _GiftCenter.giftHistClasses[num].step = 4;//"16602815EADA97B5"
        StartCoroutine(_GiftCenter.WriteGiftCenter());
        _GiftCenter.AzureUpate(_GiftCenter.giftHistClasses[num].senderID, _GiftCenter.giftHistClasses[num].number, 4,num);
        _GiftCenter.RestGiftCenter(0);
    }
    public void GiftLog()
    {
    }
}
