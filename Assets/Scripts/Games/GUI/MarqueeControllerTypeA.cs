using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotTemplate {

    public class MarqueeControllerTypeA : MarqueeController {

        [Header("Score Showing")]
        [SerializeField] GameObject _scoreShowing;
        [SerializeField] Text _scoreShowingText;

        [Header("Icon Showing")]
        [SerializeField] GameObject _iconShowing;
        [SerializeField] Image _iconImage;
        [SerializeField] Text _iconShowingText;


        public override void ShowTotalScore (decimal score) {
            base.ShowTotalScore(score);

            _scoreShowing.SetActive(true);
            _iconShowing.SetActive(false);

            _scoreShowingText.text = "中獎 = " + score;
        }

        public override void ShowWonIconAmountAndWonScore (int iconId, int amount, decimal score) {
            base.ShowWonIconAmountAndWonScore(iconId, amount, score);

            _scoreShowing.SetActive(false);
            _iconShowing.SetActive(true);

            if (iconsSpriteDB != null) {
                _iconImage.sprite = iconsSpriteDB.GetSpriteById(iconId);
                _iconImage.SetNativeSize();
            }
            _iconShowingText.text = "X" + amount + "連線 = " + score;
        }

    }
}
