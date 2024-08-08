using System;
using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public abstract class OneShotAnimationBehaviour : MonoBehaviour {

        protected abstract void OnEnable ();
        protected abstract void OnDisable ();

        public abstract void SetParameters (object paramters);
        public abstract void Advance ();

        public virtual void Play () {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }


        public IEnumerator WaitToTheEnd () {
            yield return new WaitUntil(() => !gameObject.activeSelf);
        }


        public IEnumerator PlayAndCheckForAdvancing (Func<bool> getAdvancingCondition = null) {

            Play();

            while (gameObject.activeSelf) {
                if (getAdvancingCondition != null && getAdvancingCondition()) {
                    Advance();
                }

                yield return null;
            }

        }

    }

}
