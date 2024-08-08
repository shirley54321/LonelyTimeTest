using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "SpritesDB", menuName = "ScriptableObject/SpritesDB")]
    public class SpritesDB : ScriptableObject
    {
        [SerializeField] Item[] items;

        public Sprite GetSpriteById(int id)
        {
            foreach(Item item in items)
            {
                if (item.id == id)
                    return item.sprite;
            }
            return null;
        }

        public Sprite GetSpriteByName(string name)
        {
            foreach (Item item in items)
            {
                if (item.name == name)
                    return item.sprite;
            }
            return null;
        }

        [System.Serializable]
        public class Item
        {
            public int id;
            public string name;
            public Sprite sprite;
        }
    }
}

