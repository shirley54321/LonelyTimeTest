using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class TransitionsManager : MonoBehaviour {

        [SerializeField] TransitionInfo[] _transitionInfos;



        public OneShotAnimationBehaviour TransitionStart (TransitionType type, object? parameters = null, Func<bool> getAdvancingCondition = null) {

            OneShotAnimationBehaviour transition = TransitionInfo.GetTransitionByType(_transitionInfos, type);

            if (parameters != null) {
                transition.SetParameters(parameters);
            }

            StartCoroutine(transition.PlayAndCheckForAdvancing(getAdvancingCondition));

            return transition;
        }

        public IEnumerator WaitForTransitionEnd (OneShotAnimationBehaviour transition) {
            yield return StartCoroutine(transition.WaitToTheEnd());
        }


        public IEnumerator TransitionRunning (TransitionType type, object? parameters = null, Func<bool> getAdvancingCondition = null) {
            yield return WaitForTransitionEnd(TransitionStart(type, parameters, getAdvancingCondition));
        }


        // == Nested Classes ==
        [Serializable]
        public class TransitionInfo {
            public TransitionType type;
            public OneShotAnimationBehaviour transition;

            public static OneShotAnimationBehaviour GetTransitionByType (TransitionInfo[] infos, TransitionType type) {
                foreach (TransitionInfo info in infos) {
                    if (info.type == type) {
                        return info.transition;
                    }
                }
                return null;
            }
        }


        public enum TransitionType {
            BigWinAnimation,
            FadeOut,
            FadeIn,
            BonusGameTriggering,
            BonusGameResultShowing,
            GainMoreBonusRounds,
            GainedMaxBonusRounds,
            ClickToStartBonusGameShowing
        }

    }

}
