using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "AudioClipsDB", menuName = "ScriptableObject/AudioClipsDB")]
    public class AudioClipsDB : ScriptableObject
    {
        [SerializeField] Item[] _items;
        public Item[] items => _items;

        public AudioClip GetAudioClipById(int id)
        {
            foreach (Item item in _items)
            {
                if (item.id == id)
                {
                    return item.audioClip;
                }
            }
            return null;
        }

        public AudioClip GetAudioClipByName(string name)
        {
            foreach (Item item in _items)
            {
                if (item.name == name)
                {
                    return item.audioClip;
                }
            }
            return null;
        }

        public AudioClip GetRandomAudioClip()
        {
            if (_items.Length == 0)
            {
                return null;
            }

            if (_items.Length == 1)
            {
                return _items[0].audioClip;
            }

            return _items[Random.Range(0, _items.Length)].audioClip;
        }

        [System.Serializable]
        public class Item
        {
            public int id;
            public string name;
            public AudioClip audioClip;
        }
    }
}
