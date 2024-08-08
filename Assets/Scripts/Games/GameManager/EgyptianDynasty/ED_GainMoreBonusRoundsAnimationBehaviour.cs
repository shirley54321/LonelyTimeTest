using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlotTemplate
{

    public class ED_GainMoreBonusRoundsAnimationBehaviour : SimpleWaitingForAdvanceBehaviour
    {

        int _gainedRounds = 0;
        public int GainedRounds
        {
            get
            {
                return _gainedRounds;
            }
            private set
            {
                _gainedRounds = value;
                _roundsTextContainer.text = $"<sprite={_gainedRounds.ToString()}>";
            }
        }


        // [SerializeField] float _autoAdvanceWaitingDuration = 1f;

        [SerializeField] GameObject _triggeringInfoPanel;
        [SerializeField] TextMeshProUGUI _roundsTextContainer;
        
        [SerializeField] Sprite SecondRound;
        [SerializeField] Sprite ThirdRound;

        [SerializeField] ED_BonusSpecialRules ED_bonusSpecialRules;


        // bool _autoAdvance = false;


        protected override void OnEnable()
        {
            base.OnEnable();
            RoundPictureReplacement();
            _triggeringInfoPanel.SetActive(true);

            // StartCoroutine(Running());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GainedRounds = 0;
        }



        public override void SetParameters(object objectParameters)
        {
            if (objectParameters is Parameters)
            {
                Parameters parameters = (Parameters)objectParameters;

                _autoAdvance = parameters.autoAdvance;
                GainedRounds = parameters.gainedRounds;
            }
        }

        public void RoundPictureReplacement() 
        {
            if (ED_bonusSpecialRules.isAllFakeReel[1] == true && ED_bonusSpecialRules.isAllFakeReel[2] == false && ED_bonusSpecialRules.isAllFakeReel[3] == false)
            {
                _triggeringInfoPanel.GetComponent<Image>().sprite = SecondRound;
            }
            else if (ED_bonusSpecialRules.isAllFakeReel[1] == true && ED_bonusSpecialRules.isAllFakeReel[2] == false && ED_bonusSpecialRules.isAllFakeReel[3] == true)
            {
                _triggeringInfoPanel.GetComponent<Image>().sprite = ThirdRound;
            }
            else
            {
                _triggeringInfoPanel.GetComponent<Image>().sprite = SecondRound;
            }
        }

        public struct Parameters
        {
            public bool autoAdvance;
            public int gainedRounds;
        }

    }

}
