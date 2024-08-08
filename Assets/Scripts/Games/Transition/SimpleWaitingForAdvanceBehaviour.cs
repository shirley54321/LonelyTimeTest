using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public class SimpleWaitingForAdvanceBehaviour : OneShotAnimationBehaviour {

        protected bool _autoAdvance = false;
        [SerializeField] protected float _autoAdvanceWaitingDuration = 1f;


        protected override void OnEnable () {

            if (_autoAdvance) {
                StartCoroutine(WaitForAutoAdvance(_autoAdvanceWaitingDuration));
            }

        }

        protected override void OnDisable () {

        }

        public override void Advance () {
            gameObject.SetActive(false);
        }

        public override void SetParameters (object objectParameters) {
            if (objectParameters is Parameters) {
                _autoAdvance = ((Parameters) objectParameters).autoAdvance;
            }
        }


        IEnumerator WaitForAutoAdvance (float waitingDuration) {
            yield return new WaitForSeconds(waitingDuration);
            Advance();
        }


        public struct Parameters {
            public bool autoAdvance;
        }
    }
}
