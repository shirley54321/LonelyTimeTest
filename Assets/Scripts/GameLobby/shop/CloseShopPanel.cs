using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseShopPanel : BasePanel
{
    public void CloseShop()
    {
        UIManager.Instance.ClosePanel(UIConst.ShopPanel);
    }
    public void RestDragonCoin()
    {
        
        SlotTemplate.PlayerStatus.Instance.GetVirtualCoin();//§ó·sÂÂÀs¹ô

    }
}
