using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "AnimationClipsDB", menuName = "ScriptableObject/AnimationClipsDB")]
    public class AnimationClipsDB : ScriptableObject
    {
        [SerializeField] Item[] items;

        public AnimationClip GetAnimationClipById(int id)
        {
            foreach (Item item in items)
            {
                if (item.id == id)
                {
                    return item.animClip;
                }
            }
            return null;
        }

        public AnimationClip GetAnimationClipByName(string name)
        {
            foreach (Item item in items)
            {
                if (item.name == name)
                {
                    return item.animClip;
                }
            }
            return null;
        }

        [System.Serializable]
        public class Item
        {
            public int id;
            public string name;
            public AnimationClip animClip;
        }
    }
}

