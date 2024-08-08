using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimatorOverrider : MonoBehaviour {

        Animator _animator;
        public Animator animator {
            get {
                if (_animator == null) {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
            private set {
                _animator = value;
            }
        }

        void Awake () {
            animator = GetComponent<Animator>();
        }


        // Override the animation clips of an Animator.
        public void OverrideAnimationClipsAndRestart (AnimationClip[] animClips) {
            /*
            To learn more about "AnimatorOverrideController", see:
            - Unity Manual - Animator Override Controller: https://docs.unity3d.com/Manual/AnimatorOverrideController.html
            - Unity Scripting API - AnimatorOverrideController: https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
            */

            if(animClips != null)
            {

                try
                {
                    AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

                    if (animatorOverrideController.animationClips[0] == null)
                        Debug.Log("The previous is null");

                    var animsOverriding = new List<KeyValuePair<AnimationClip, AnimationClip>>();

                    for (int i = 0; i < Math.Min(animatorOverrideController.animationClips.Length, animClips.Length); i++)
                    {
                        animsOverriding.Add(new KeyValuePair<AnimationClip, AnimationClip>(animatorOverrideController.animationClips[i], animClips[i]));
                    }

                    animatorOverrideController.ApplyOverrides(animsOverriding);
                    animator.runtimeAnimatorController = animatorOverrideController;

                    // Re-enable the game object to let the animator start from begining.
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Errer At Animator Overrider: {e}");
                }

                animator = GetComponent<Animator>();
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            }
            else
            {
                Debug.Log($"The animation clip not reachs");
            }
            
        }

    }

}
