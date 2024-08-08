using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowExp : MonoBehaviour
{
    public GameObject bubble;
    public float displayDuration = 5f;
    [SerializeField]
    private TextMeshProUGUI experience;
    //private Slider experienceBar;

    private bool bubbleActive = false;
    private float timer = 0f;

    void Start()
    {
        // 初始时隐藏气泡框
        bubble.SetActive(false);
    }

    /*public void UpdateLevelUI(decimal nowExperience) // TODO 修改傳入方式
    {
        experience.text =
           // $"{nowExperience} / {nextLevelExperience}";
           $"{nowExperience} / {nextLevelExperience}";
        // experienceBar.fillAmount = (float) nowExperience / (float) nextLevelExperience;
        float fillAmount = ((float)nowExperience % 90000) / (float)90000;
        Debug.Log($"(float)nowExperience%90000) {(float)nowExperience % 90000}, fillAmount {fillAmount}");
        experienceBar.value = fillAmount;

    }*/

    void Update()
    {
        // 检查是否需要隐藏气泡框
        if (bubbleActive)
        {
            timer += Time.deltaTime;
            if (timer >= displayDuration)
            {
                HideBubble();
            }
        }
    }

    public void OnNameClick()
    {
        // 显示气泡框
        bubble.SetActive(true);
        bubbleActive = true;

        // 重置计时器
        timer = 0f;
    }

    private void HideBubble()
    {
        // 隐藏气泡框
        bubble.SetActive(false);
        bubbleActive = false;
    }

}
