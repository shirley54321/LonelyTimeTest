using System.Collections;
using UnityEngine;
using TMPro;

namespace SlotTemplate {

    public class BonusGameTriggeringAnimationBehaviour : SimpleWaitingForAdvanceBehaviour {

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


        [SerializeField] GameObject _triggeringInfoPanel;
        [SerializeField] TextMeshProUGUI _roundsTextContainer;




        protected override void OnEnable () {
            base.OnEnable();
            _triggeringInfoPanel.SetActive(true);

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
