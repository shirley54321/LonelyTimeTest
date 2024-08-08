using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarAdapt : MonoBehaviour
{
    [SerializeField] private bool PaddingModify=false;
    public float modifyPresent=0.5f;
    public float modifySpacingPresent = 1f;

    private HorizontalLayoutGroup HorLayout;
    private VerticalLayoutGroup VerLayout;


    private bool isHor = false;

    private void Start()
    {
        LayoutGroup();
        ModifySpace();
        if(PaddingModify)
        {
            ModifyPadding();
        }
    }
    private void LayoutGroup()
    {
        if(GetComponent<HorizontalLayoutGroup>())
        {
            HorLayout = GetComponent<HorizontalLayoutGroup>();
            isHor = true;
        }
        else
        {
            VerLayout = GetComponent<VerticalLayoutGroup>();
            isHor = false;
        }
    }

    private void ModifySpace()
    {
        float NowWidth = Screen.width;
        float NowHight = Screen.height;
        float Screenpresent = NowHight /NowWidth ;
        float customerfactor = 1.1f;
        float present = Screenpresent* customerfactor;

        float physicscreen = 1.0f * Screen.width / Screen.height;

        //判斷是不是平板
        if (physicscreen > 1.7f)//手機
        {
            present *=1.5f;
        }
        else//平板
        {
            present *= modifySpacingPresent;
        }

        if (isHor)
        {
            HorLayout.spacing *= present;
        }
        else
        {
            VerLayout.spacing *= present;
        }
    }

    private void ModifyPadding()
    {
        float NowWidth = Screen.width;
        float NowHight = Screen.height;

        if (NowHight == 1080f && NowWidth == 2400) return;

        float Screenpresent = NowHight / NowWidth;
        float present = Screenpresent*modifyPresent;

        float physicscreen = 1.0f * Screen.width / Screen.height;
        //判斷是不是平板
        if (physicscreen > 1.7f)
        {
            present *= 2;
            //return;
        }

        if (isHor)
        {
            HorLayout.padding.left = (int)(HorLayout.padding.left * present);
            HorLayout.padding.right = (int)(HorLayout.padding.right * present);
            HorLayout.padding.top = (int)(HorLayout.padding.top * present);
            HorLayout.padding.bottom = (int)(HorLayout.padding.bottom * present);
        }
        else
        {
            VerLayout.padding.left = (int)(VerLayout.padding.left * present);
            VerLayout.padding.right = (int)(VerLayout.padding.right * present);
            VerLayout.padding.top = (int)(VerLayout.padding.top * present);
            VerLayout.padding.bottom = (int)(VerLayout.padding.bottom * present);
        }
    }
}
