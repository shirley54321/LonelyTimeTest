using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Manages UI panels in the game.
/// </summary>
public class UIManager
{
    #region Instance

    // 單例模式確保只有一個UIManager實例存在。
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    #endregion

    // 儲存UI prefab路徑
    private Dictionary<string, string> pathDict;

    // 儲存已經打開的面板
    private Dictionary<string, BasePanel> IsOpenPanel;

    // 儲存已經實例化的面板
    private Dictionary<string, GameObject> IsCreatePanel;

    /// <summary>
    /// Private constructor to prevent instantiation.
    /// Initializes dictionaries and sets default values.
    /// </summary>
    private UIManager()
    {
        InitDicts();
    }

    // UI根節點的參考
    private Transform _uiRoot;

    /// <summary>
    /// Gets the UI root transform from the scene.
    /// </summary>
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("UICanvas"))
                {
                    _uiRoot = GameObject.Find("UICanvas").transform;
                }
                
                return _uiRoot;
            }
            return _uiRoot;
        }
    }

    /// <summary>
    /// Initializes dictionaries with default values.
    /// </summary>
    private void InitDicts()
    {
        IsCreatePanel = new Dictionary<string, GameObject>();
        IsOpenPanel = new Dictionary<string, BasePanel>();

        // 使用預設路徑初始化路徑
        pathDict = new Dictionary<string, string>()
        {
            {UIConst.SettingPanel,"Setting_Panel"},
            {UIConst.LocalPlayerPage,"LocalPlayerInfoPage"},
            {UIConst.OtherPlayerPage,"OtherPlayerInfoPage"},
            {UIConst.ChatPanel, "ChatPagePanel"},
            {UIConst.BankBookPanel, "Bank BookPanel" },
            {UIConst.ShopPanel, "Shop" },
            {UIConst.LinkAccountPanel, "LinkAccountPanel"},
            {UIConst.VIPPanel,"VIP_Panel"},
            {UIConst.ClassificationPanel,"Classification_Panel" },
            {UIConst.BagPagePanel,"BagPagePanel" },
            {UIConst.GiftPanel,"Gift_Panel" },
            {UIConst.FriendListPanel,"FriendListPanel" }
        };
    }

    /// <summary>
    /// Opens a UI panel with the specified name.
    /// </summary>
    /// <param name="name">Name of the panel to open.</param>
    /// <returns>Returns the opened BasePanel instance.</returns>

    public BasePanel OpenPanel(string name)
    {

        // 檢查面板是否已經打開
        BasePanel panel = null;
        if (IsOpenPanel.TryGetValue(name, out panel))
        {
            Debug.Log(name + " is already open");
            return null;
        }
        // 獲取prefab的路徑
        string path = string.Empty;
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.Log(name + " path not found");
            return null;
        }

        // 載入或獲取prefab
        GameObject panelPrefab = null;
        if (!IsCreatePanel.TryGetValue(name, out panelPrefab) || panelPrefab == null)
        {
            string RealPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(RealPath);
            //panelPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + RealPath + ".prefab", typeof(GameObject)) as GameObject;
            Debug.Log($"panelPrefab {panelPrefab}, UIRoot {UIRoot}");
            panelPrefab = GameObject.Instantiate(panelPrefab, UIRoot, false);
            IsCreatePanel[name] = panelPrefab;
        }
        if (name == "Setting_Panel")
        {
            Setting settingPanelInstance = panelPrefab.GetComponent<Setting>();
            if (settingPanelInstance != null)
            {
                settingPanelInstance.ResetPanel();
            }
        }
        else if(name == "VIP_Panel"){
            VIPPanel VIPPanelPanelInstance = panelPrefab.GetComponent<VIPPanel>();
            if (VIPPanelPanelInstance != null)
            {
                VIPPanelPanelInstance.SetInfo();
            }
        }
        //把層級位置調到最上層
        int i = IsCreatePanel.Count;
        // panelPrefab.transform.SetSiblingIndex(i - 1);
        panelPrefab.transform.SetAsLastSibling();
        // 實例化面板並添加到已打開的面板字典中
        panel = panelPrefab.GetComponent<BasePanel>();
        IsOpenPanel.Add(name, panel);
        panel.OpenPanel(name);

        return panel;
    }

    /// <summary>
    /// Closes the UI panel with the specified name.
    /// </summary>
    /// <param name="name">Name of the panel to close.</param>
    public void ClosePanel(string name)
    {
        BasePanel panel = null;
        if (!IsOpenPanel.TryGetValue(name, out panel))
        {
            Debug.Log(name + " panel not open");
            return;
        }

        panel.ClosePanel();
        IsOpenPanel.Remove(name);
    }

    public void ClearDictionary()
    {
        IsCreatePanel.Clear();
    }
}
