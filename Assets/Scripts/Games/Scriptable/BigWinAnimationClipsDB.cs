using UnityEngine;

namespace SlotTemplate
{

    [CreateAssetMenu(fileName = "Big Win Animation Clips DB", menuName = "ScriptableObject/Big Win Animation Clips DB")]
    public class BigWinAnimationClipsDB : ScriptableObject
    {

        [SerializeField] Item[] items;

        public AnimationClip GetAnimationClipById(int id)
        {
            Debug.Log($"The Win IconID: {id}");
            foreach (Item item in items)
            {
                if (item.id == id)
                {
                    return item.animClip;
                }
            }
            return null;
        }

        public AnimationClip GetAnimationClipByType(BigWinType bigWinType, StepType stepType)
        {
            foreach (Item item in items)
            {
                if (item.bigWinType == bigWinType && item.stepType == stepType)
                {
                    Debug.Log($"bigWinType: {bigWinType}, itemType: {item.bigWinType}, IconID: {item.id}");
                    return item.animClip;
                }
            }
            return null;
        }

        [System.Serializable]
        public class Item
        {
            public int id;
            public BigWinType bigWinType;
            public StepType stepType;
            public AnimationClip animClip;
        }

        public enum StepType
        {
            In,
            Loop,
            Out
        }

    }

}
