using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseChatPanel : BasePanel
{
    public static bool IsChatPanelOpen=false;
    public void ClosePanel()
    {
        UIManager.Instance.ClosePanel(UIConst.ChatPanel);
        IsChatPanelOpen=false;
    }

    public void OpenPanel()
    {
        IsChatPanelOpen = true;
    }

}
