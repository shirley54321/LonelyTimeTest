using System.Collections;
using UnityEngine;
using TMPro;

namespace SlotTemplate {

    public class GainMoreBonusRoundsAnimationBehaviour : SimpleWaitingForAdvanceBehaviour {

        int _gainedRounds = 0;
        public int GainedRounds {
            get {
                return _gainedRounds;
            }
            private set {
                _gainedRounds = value;
                string totalnum = _gainedRounds.ToString();
                string values = string.Empty;
                foreach (var num in totalnum)
                {
                    values += $"<sprite={num}>";
                }
                _roundsTextContainer.text = values;
            }
        }


        // [SerializeField] float _autoAdvanceWaitingDuration = 1f;

        [SerializeField] GameObject _triggeringInfoPanel;
        [SerializeField] TextMeshProUGUI _roundsTextContainer;



        // bool _autoAdvance = false;


        protected override void OnEnable () {
            base.OnEnable();
            _triggeringInfoPanel.SetActive(true);

            // StartCoroutine(Running());
        }

        protected override void OnDisable () {
            base.OnDisable();
            GainedRounds = 0;
        }



        public override void SetParameters (object objectParameters) {
            if (objectParameters is Parameters) {
                Parameters parameters = (Parameters) objectParameters;

                _autoAdvance = parameters.autoAdvance;
                GainedRounds = parameters.gainedRounds;
            }
        }


        public struct Parameters {
            public bool autoAdvance;
            public int gainedRounds;
        }

    }

}
