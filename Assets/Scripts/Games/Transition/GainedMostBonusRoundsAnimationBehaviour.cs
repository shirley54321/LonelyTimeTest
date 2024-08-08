using System.Collections;
using UnityEngine;
using TMPro;

namespace SlotTemplate {

    public class GainedMostBonusRoundsAnimationBehaviour : SimpleWaitingForAdvanceBehaviour {


        // [SerializeField] float _autoAdvanceWaitingDuration = 1f;

        [SerializeField] GameObject _triggeringInfoPanel;


        // bool _autoAdvance = false;


        protected override void OnEnable () {
            base.OnEnable();
            _triggeringInfoPanel.SetActive(true);

            // StartCoroutine(Running());
        }

        // protected override void OnDisable () {
        //
        // }



        public override void SetParameters (object objectParameters) {
            if (objectParameters is Parameters) {
                Parameters parameters = (Parameters) objectParameters;

                _autoAdvance = parameters.autoAdvance;
            };
        }

        // public override void Advance () {
        //     gameObject.SetActive(false);
        // }


        // IEnumerator Running () {
        //     yield return new WaitForSeconds(_autoAdvanceWaitingDuration);
        //
        //     if (_autoAdvance) {
        //         Advance();
        //     }
        // }


        public struct Parameters {
            public bool autoAdvance;
        }

    }

}
