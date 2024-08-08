using UnityEngine;
public class Setting : BasePanel
{
    public GameObject ToggleGroup;
    public GameObject UserData_Toggle;
    public GameObject HonorWall_Toggle;

    public GameObject Setting_Panel;
    public GameObject UserData_Panel;
    public GameObject Uninstall_Panel;
    public GameObject ChangeAccount_Panel;
    public GameObject BindAccount_Panel;
    public GameObject CancelAccount_Panel;

    public GameObject About_US;
    public GameObject Feedback_Panel;
    public GameObject RecordList_Panel;
    public GameObject RecordPage_Panel;

    public void ResetPanel() {
        Debug.Log("ResetPanal");
        ToggleGroup.SetActive(true);
        UserData_Toggle.SetActive(true);
        Setting_Panel.SetActive(true);

        About_US.SetActive(false);
        Feedback_Panel.SetActive(false);
        RecordList_Panel.SetActive(false);
        RecordPage_Panel.SetActive(false);
        
        HonorWall_Toggle.SetActive(false);
        UserData_Panel.SetActive(false);
        Uninstall_Panel.SetActive(false);
        ChangeAccount_Panel.SetActive(false);
        BindAccount_Panel.SetActive(false);
        CancelAccount_Panel.SetActive(false);    
    }
    public void CloseSettingPanel()
    {
        UIManager.Instance.ClosePanel(UIConst.SettingPanel);
    }
    public void CloseBankBookPanel()
    {
        UIManager.Instance.ClosePanel(UIConst.BankBookPanel);
    }

    public void OpenLinkAccountPanel()
    {
        InstanceRemindPanel.Instance.OpenPanel("敬請期待");
        // UIManager.Instance.OpenPanel(UIConst.LinkAccountPanel);
    }
}
