using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserAccount.UI;

public class AboutUsPanelSetting : MonoBehaviour
{
    public GameObject InformationPanel;
    public UserRuleUI userRule;
    public TextAsset StartText;
    public void OpenInformationPanel()
    {
        InformationPanel.SetActive(true);
        userRule.OnToggleChanged(StartText);
    }

    public void CloseInformationPanel()
    {
        InformationPanel.SetActive(false);
    }


    public void UnfinishClick()
    {

        InstanceRemindPanel.Instance.OpenPanel("·q½Ð´Á«Ý");
    }
}
