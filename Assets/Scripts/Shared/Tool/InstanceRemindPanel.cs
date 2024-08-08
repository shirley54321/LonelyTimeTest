using System.Collections;
using System.Collections.Generic;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;

public class InstanceRemindPanel : MonoBehaviour
{
    #region Instance(Singleton)
    private static InstanceRemindPanel instance;

    public static InstanceRemindPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InstanceRemindPanel>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject with {typeof(InstanceRemindPanel)} does not exist in the scene, " +
                                   $"yet its method is being called.\n" +
                                   $"Please add {typeof(InstanceRemindPanel)} to the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject.transform.root);//全場可用
    }


    #endregion

    public RemindPanel Remind_Panel;
    public TextMeshProUGUI Remind_Text;
    public void OpenRemindPanel()
    {
        Remind_Panel.OpenPanel();
    }
    
    public void OpenPanel(string info)
    {
        Remind_Panel.OpenPanel();
        Remind_Text.text = info;
    }

    public void ClosePanel()
    {
        Remind_Panel.ClosePanel();
    }
}
