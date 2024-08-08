using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGiftPanel : BasePanel
{
    // Start is called before the first frame update
    public void CloseGift()
    {
        UIManager.Instance.ClosePanel(UIConst.GiftPanel);
        ChangeUiRedPoint.IsGiftRedPointbotton(false);
    }
}
