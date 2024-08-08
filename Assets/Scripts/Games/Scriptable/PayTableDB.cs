using System;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Pay Table DB", menuName = "ScriptableObject/Pay Table DB")]
    public class PayTableDB : ScriptableObject
    {
        public IconPaysInfo[] iconPaysInfos;

        public int[] GetPaysInfoById(int id)
        {
            foreach (IconPaysInfo info in iconPaysInfos)
            {
                if (info.id == id)
                {
                    return info.paysInfo;
                }
            }
            return null;
        }

        public int[] GetPaysInfoByName(string name)
        {
            foreach (IconPaysInfo info in iconPaysInfos)
            {
                if (info.name == name)
                {
                    return info.paysInfo;
                }
            }
            return null;
        }

        [Serializable]
        public class IconPaysInfo
        {
            public int id;
            public string name;
            public int[] paysInfo;
        }
    }
}

