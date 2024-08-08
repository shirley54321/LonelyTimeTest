using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class ToggleColorChangeTMP : MonoBehaviour
    {
        [Header("References")]
        public Toggle toggle;
        public TextMeshProUGUI text;

        [Header("Colors")]
        [SerializeField] private Color trueColor = Color.black;
        [SerializeField] private Color falseColor = Color.gray;
        
        private void Update()
        {
            text.color = toggle.isOn ? trueColor : falseColor;
        }

    }
}