using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseHamburgerPanel : MonoBehaviour
{
    public GameObject HamburgerPanel;

    public void hamburgerPanel()
    {
        if (HamburgerPanel != null)
        {
            if (HamburgerPanel.activeSelf)
            {
                HamburgerPanel.SetActive(false);
            }
            else
            {
                HamburgerPanel.SetActive(true);
            }
        }
    }

    public void UnfinishClick()
    {

        InstanceRemindPanel.Instance.OpenPanel("·q½Ð´Á«Ý");
    }
}
