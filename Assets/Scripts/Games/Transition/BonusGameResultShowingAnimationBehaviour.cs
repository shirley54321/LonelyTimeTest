using System.Collections;
using UnityEngine;
using TMPro;

namespace SlotTemplate {

    public class BonusGameResultShowingAnimationBehaviour : SimpleWaitingForAdvanceBehaviour
    {
        decimal _bonusWinPerBonusLine = 0;
        public decimal BonusWinPerBonusLine
        {
            get
            {
                return _bonusWinPerBonusLine;
            }
            private set
            {
                _bonusWinPerBonusLine = value;
                string totalnum = _bonusWinPerBonusLine.ToString();
                string values = string.Empty;
                foreach (var num in totalnum)
                {
                    values += $"<sprite={num}>";
                }
                _bonusWinPerBonusLineTextContainer.text = values;
            }
        }

        int _bonusLineCount = 0;
        public int BonusLineCount
        {
            get
            {
                return _bonusLineCount;
            }
            private set
            {
                _bonusLineCount = value;
                string totalnum = _bonusLineCount.ToString();
                string values = string.Empty;
                foreach(var num in totalnum)
                {
                    values += $"<sprite={num}>";
                }
                _bonusLineCountTextContainer.text = values;
            }
        }

        decimal _totalBonusWin = 0;
        public decimal TotalBonusWin
        {
            get
            {
                return _totalBonusWin;
            }
            set
            {
                _totalBonusWin = value;
                string totalnum = _totalBonusWin.ToString();
                string values = string.Empty;
                foreach (var num in totalnum)
                {
                    values += $"<sprite={num}>";
                }
                _totalBonusWinTextContainer.text = values;
            }
        }


        // [SerializeField] float _autoAdvanceWaitingDuration = 2.5f;

        [SerializeField] GameObject _triggeringInfoPanel;
        [SerializeField] TextMeshProUGUI _bonusWinPerBonusLineTextContainer;
        [SerializeField] TextMeshProUGUI _bonusLineCountTextContainer;
        [SerializeField] TextMeshProUGUI _totalBonusWinTextContainer;



        // bool _autoAdvance = false;


        protected override void OnEnable()
        {
            base.OnEnable();
            _triggeringInfoPanel.SetActive(true);

            // StartCoroutine(Running());
        }

        // protected override void OnDisable () {
        //
        // }



        public override void SetParameters(object objectParameters)
        {
            if (objectParameters is Parameters)
            {
                Parameters parameters = (Parameters)objectParameters;

                _autoAdvance = parameters.autoAdvance;
                BonusWinPerBonusLine = parameters.bonusWinPerBonusLine;
                BonusLineCount = parameters.bonusLineCount;
                TotalBonusWin = parameters.TotalBonusWin;
            }
        }


        public struct Parameters
        {
            public bool autoAdvance;
            public decimal bonusWinPerBonusLine;
            public int bonusLineCount;

            public decimal TotalBonusWin => bonusWinPerBonusLine * bonusLineCount;
        }

    }
}
