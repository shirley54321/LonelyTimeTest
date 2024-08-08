using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "BigWinAsset", menuName = "ScriptableObject/BigWinAsset")]
    public class BigWinAsset : ScriptableObject
    {
        [SerializeField] Item[] items;

        public AnimationClip GetAnimationClipType(BigWinType bigWinType, StepType stepType)
        {
            string animationName = "";

            foreach (Item item in items)
            {
                if (item.bigWinType == bigWinType && item.stepType == stepType)
                {
                    animationName = item.bigWinType + "_Animation_" + item.stepType;
                    //assetBundleName = "../Animation/Game/BigWinAnimations/";

                    AnimationClip myAnimationClip = Resources.Load<AnimationClip>(animationName);

                    if (myAnimationClip == null)
                    {
                        Debug.LogError("Failed to load AssetBundle!");
                    }
                    else
                    {
                        Debug.Log($"Name: {myAnimationClip.name}");
                        return myAnimationClip;
                    }

                    /*AnimationClip myAnimationClip = Resources.Load(assetBundleName+animationName, typeof(AnimationClip)) as AnimationClip;

                    if (myAnimationClip != null)
                    {
                        return myAnimationClip;
                    }
                    else
                    {
                        Debug.LogError($"Animation Clip not found at path: {animationName}");
                    }*/
                }
            }
            return null;
        }

        [System.Serializable]
        public class Item
        {
            public BigWinType bigWinType;
            public StepType stepType;
        }

        public enum StepType
        {
            In,
            Loop,
            Out
        }
    }

}