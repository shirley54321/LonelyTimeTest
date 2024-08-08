using Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class refreshUserLevel : MonoBehaviour
{
    public GameObject LevelUPUI;
    public TextMeshProUGUI levelText; // 如果使用的是 UI TextMeshPro 元件
    [SerializeField] Slider _levelSlider;

    // Start is called before the first frame update
    void Start()
    {
        userLevel.refreshUserLevel();
    }

    // Update is called once per frame
    void Update()
    {
        LevelUPUI.SetActive(userLevel.test);

        //Debug.Log("" + userLevel.Experience.ToString());
        if (userLevel.Experience==-1)
        {
            userLevel.refreshUserLevel();
        }


        levelText.text = userLevel.Experience.ToString() + " / 90000";
        _levelSlider.value = userLevel.Experience / 90000;
    }

    public void refreshUserLevelBtn()
    {
        userLevel.refreshUserLevel();
        PlayerInfoManager.UpdateIngame();
    }
}
