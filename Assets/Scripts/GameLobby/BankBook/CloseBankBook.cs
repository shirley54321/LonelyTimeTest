using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBankBook : BasePanel
{
    public void CloseBankBookPanel()
    {
        UIManager.Instance.ClosePanel(UIConst.BankBookPanel);
    }
}
