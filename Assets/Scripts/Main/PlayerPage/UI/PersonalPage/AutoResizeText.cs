using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.PersonalPage
{
    /// <summary>
    /// Ｗhen the text increases to a newline, this scripts can let Transform auto resize.
    /// </summary>
    public class AutoResizeText : MonoBehaviour
    {
        private TextMeshProUGUI textComponent;
        private RectTransform rectTransform;

        private void Start()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();

            ResizeText();
        }

        private void Update()
        {
            ResizeText();
        }

        private void ResizeText()
        {
            float preferredWidth = textComponent.preferredWidth;
            float preferredHeight = textComponent.preferredHeight;

            rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);
        }
    }
}
