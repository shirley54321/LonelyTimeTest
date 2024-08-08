using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VipIconData", menuName = "FakeData/VipIconData")]
public class VipIconData : ScriptableObject
{
    public List<Sprite> icons;

    public Sprite GetIcon(int vipLevel)
    {
        int index = vipLevel - 1;
        
        if (index > icons.Count)
        {
            Debug.Log($"Can't found Icon {vipLevel}");
            return icons[0];
        }
        else
        {
            return icons[index];
        }
    }
}
