using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class ToggleColorChange : MonoBehaviour
    {
        [Header("References")]
        public Toggle toggle;
        public Text text;

        [Header("Colors")]
        [SerializeField] private Color trueColor = Color.black;
        [SerializeField] private Color falseColor = Color.gray;

        private void Start()
        {
            // 設定 Toggle 的監聽事件
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            // 根據 Toggle 的狀態設定文字顏色
            text.color = isOn ? trueColor : falseColor;
        }
    }
}