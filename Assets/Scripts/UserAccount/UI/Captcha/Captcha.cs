using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Captcha : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI captchaTextUINumberOne;
    [SerializeField] private TextMeshProUGUI captchaTextUINumberTwo;
    [SerializeField] private TextMeshProUGUI captchaTextUINumberThree;
    [SerializeField] private TextMeshProUGUI captchaTextUINumberFour;

    [SerializeField] private TMP_FontAsset SEGO;
    [SerializeField] private TMP_FontAsset RUBI;
    [SerializeField] private TMP_FontAsset COUR;
    [SerializeField] private TMP_FontAsset ANTO;
    [SerializeField] private TMP_FontAsset BANG;
    [SerializeField] private TMP_FontAsset ELEC;

    public string captchaTextOne;
    public string captchaTextTwo;
    public string captchaTextThree;
    public string captchaTextFour;
    public string captchaNumberCheck;

    // Start is called before the first frame update
    void Start()
    {
        //自動生成圖片
        GenerateCaptchaTextOne();
        GenerateCaptchaTextTwo();
        GenerateCaptchaTextThree();
        GenerateCaptchaTextFour();

        //更新驗證碼UI
        UpdateCaptchaUIOne();
        UpdateCaptchaUITwo();
        UpdateCaptchaUIThree();
        UpdateCaptchaUIFour();

        //更改字體樣式
        RandomUseStyleOne();
        RandomUseStyleTwo();
        RandomUseStyleThree();
        RandomUseStyleFour();

        //隨機變換字體
        ChangeFont();

        //檢查驗證碼
        //CheckCaptcha();
    }

    public void ChangeCaptcha()
    {
        //自動生成圖片
        GenerateCaptchaTextOne();
        GenerateCaptchaTextTwo();
        GenerateCaptchaTextThree();
        GenerateCaptchaTextFour();

        //更新驗證碼UI
        UpdateCaptchaUIOne();
        UpdateCaptchaUITwo();
        UpdateCaptchaUIThree();
        UpdateCaptchaUIFour();

        //更改字體樣式
        RandomUseStyleOne();
        RandomUseStyleTwo();
        RandomUseStyleThree();
        RandomUseStyleFour();

        //隨機變換字體
        ChangeFont();
    }
    public void GenerateCaptchaTextOne()
    {
        string possibleCharacters = "1234567890";
        System.Random random = new System.Random();
        //new char[i] -> 任意更改長度
        char[] chars = new char[1];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = possibleCharacters[random.Next(possibleCharacters.Length)];
        }
        captchaTextOne = new string(chars);
    }

    public void GenerateCaptchaTextTwo()
    {

        string possibleCharacters = "1234567890";
        System.Random random = new System.Random();
        char[] chars = new char[1];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = possibleCharacters[random.Next(possibleCharacters.Length)];

        }
        captchaTextTwo = new string(chars);

    }

    public void GenerateCaptchaTextThree()
    {

        string possibleCharacters = "1234567890";
        System.Random random = new System.Random();
        char[] chars = new char[1];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = possibleCharacters[random.Next(possibleCharacters.Length)];

        }
        captchaTextThree = new string(chars);

    }

    public void GenerateCaptchaTextFour()
    {

        string possibleCharacters = "1234567890";
        System.Random random = new System.Random();
        char[] chars = new char[1];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = possibleCharacters[random.Next(possibleCharacters.Length)];

        }
        captchaTextFour = new string(chars);

    }

    public void UpdateCaptchaUIOne()
    {
        if (captchaTextUINumberOne != null)
        {
            captchaTextUINumberOne.text = captchaTextOne;
        }

        else
        {
            Debug.LogError("Text UI element not assigned!");
        }

        //隨機更改文字大小
        captchaTextUINumberOne.fontSize = Random.Range(40, 60);
        //隨機更改文字顏色
        captchaTextUINumberOne.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 0.4f), Random.Range(0.0f, 1.0f), 255);
    }

    public void UpdateCaptchaUITwo()
    {
        if (captchaTextUINumberTwo != null)
        {
            captchaTextUINumberTwo.text = captchaTextTwo;
        }

        else
        {
            Debug.LogError("Text UI element not assigned!");
        }

        captchaTextUINumberTwo.fontSize = Random.Range(40, 60);
        captchaTextUINumberTwo.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 0.4f), Random.Range(0.0f, 1.0f), 255);
    }

    public void UpdateCaptchaUIThree()
    {
        if (captchaTextUINumberThree != null)
        {
            captchaTextUINumberThree.text = captchaTextThree;
        }

        else
        {
            Debug.LogError("Text UI element not assigned!");
        }

        captchaTextUINumberThree.fontSize = Random.Range(40, 60);
        captchaTextUINumberThree.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 0.4f), Random.Range(0.0f, 1.0f), 255);
    }

    public void UpdateCaptchaUIFour()
    {
        if (captchaTextUINumberFour != null)
        {
            captchaTextUINumberFour.text = captchaTextFour;
        }

        else
        {
            Debug.LogError("Text UI element not assigned!");
        }


        captchaTextUINumberFour.fontSize = Random.Range(40, 60);
        captchaTextUINumberFour.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 0.4f), Random.Range(0.0f, 1.0f), 255);
    }

    public void RandomUseStyleOne()
    {
        captchaTextUINumberOne.fontStyle = FontStyles.Normal;
        var RandomNumber = Random.Range(1, 5);
        var RandomItalic = Random.Range(1, 5);

        captchaTextUINumberOne.fontStyle = FontStyles.Bold;

        //隨機更改字體樣式
        if (RandomItalic == RandomNumber)
        {
            captchaTextUINumberOne.fontStyle = FontStyles.Italic;
        }
    }

    public void RandomUseStyleTwo()
    {
        captchaTextUINumberTwo.fontStyle = FontStyles.Normal;
        var RandomNumber = Random.Range(1, 5);
        var RandomItalic = Random.Range(1, 5);

        captchaTextUINumberTwo.fontStyle = FontStyles.Bold;

        if (RandomItalic == RandomNumber)
        {
            captchaTextUINumberTwo.fontStyle = FontStyles.Italic;
        }
    }

    public void RandomUseStyleThree()
    {
        captchaTextUINumberOne.fontStyle = FontStyles.Normal;
        var RandomNumber = Random.Range(1, 5);
        var RandomItalic = Random.Range(1, 5);

        captchaTextUINumberThree.fontStyle = FontStyles.Bold;

        // H   ܴ     
        if (RandomItalic == RandomNumber)
        {
            captchaTextUINumberThree.fontStyle = FontStyles.Italic;
        }
    }

    public void RandomUseStyleFour()
    {
        captchaTextUINumberOne.fontStyle = FontStyles.Normal;
        var RandomNumber = Random.Range(1, 5);
        var RandomItalic = Random.Range(1, 5);

        captchaTextUINumberFour.fontStyle = FontStyles.Bold;

        // H   ܴ     
        if (RandomItalic == RandomNumber)
        {
            captchaTextUINumberFour.fontStyle = FontStyles.Italic;
        }
    }

    public void ChangeFont()
    {

        captchaTextUINumberOne.font = SEGO;
        captchaTextUINumberTwo.font = SEGO;
        captchaTextUINumberThree.font = SEGO;
        captchaTextUINumberFour.font = SEGO;

        var RandomNumber = Random.Range(1, 6);

        //隨機更改字體
        if (RandomNumber == 1)
        {
            captchaTextUINumberOne.font = RUBI;
            captchaTextUINumberTwo.font = RUBI;
            captchaTextUINumberThree.font = RUBI;
            captchaTextUINumberFour.font = RUBI;
        }

        else if (RandomNumber == 2)
        {
            captchaTextUINumberOne.font = COUR;
            captchaTextUINumberTwo.font = COUR;
            captchaTextUINumberThree.font = COUR;
            captchaTextUINumberFour.font = COUR;
        }

        else if (RandomNumber == 3)
        {
            captchaTextUINumberOne.font = ANTO;
            captchaTextUINumberTwo.font = ANTO;
            captchaTextUINumberThree.font = ANTO;
            captchaTextUINumberFour.font = ANTO;
        }

        else if (RandomNumber == 4)
        {
            captchaTextUINumberOne.font = BANG;
            captchaTextUINumberTwo.font = BANG;
            captchaTextUINumberThree.font = BANG;
            captchaTextUINumberFour.font = BANG;
        }

        else if (RandomNumber == 5)
        {
            captchaTextUINumberOne.font = ELEC;
            captchaTextUINumberTwo.font = ELEC;
            captchaTextUINumberThree.font = ELEC;
            captchaTextUINumberFour.font = ELEC;
        }

        else if (RandomNumber == 6)
        {
            captchaTextUINumberOne.font = SEGO;
            captchaTextUINumberTwo.font = SEGO;
            captchaTextUINumberThree.font = SEGO;
            captchaTextUINumberFour.font = SEGO;
        }
    }

    //public void CheckCaptcha()
    //{
    //    captchaNumberCheck = captchaTextOne + captchaTextTwo + captchaTextThree + captchaTextFour;
    //    captchaTextCheck.text = "";

    //    // P _ r  O _    
    //    if (captchaTextCheck.text.Length != 0)
    //    {
    //        // P _ r  O _ ۦP
    //        if (string.Compare(captchaNumberCheck, captchaTextCheck.text) == 0)
    //        {
    //            Debug.Log("   Ҧ  \");
    //        }
    //        else
    //        {
    //            InstanceRemindPanel.Instance.OpenPanel("   ҽX   ~");
    //        }
    //    }
    //    else
    //    {
    //        InstanceRemindPanel.Instance.OpenPanel(" п J   ҽX");
    //    }
    //}

}
