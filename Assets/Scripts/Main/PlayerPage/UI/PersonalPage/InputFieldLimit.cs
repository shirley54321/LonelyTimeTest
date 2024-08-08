using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.PersonalPage
{
    /// <summary>
    /// Set input character count limit for MessageBoard
    /// Rule 1: max line = 8
    /// Rule 2: max character = 120
    /// </summary>
    public class InputFieldLimit : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        private readonly int _maxLines = 8;
        private readonly int _maxCharacters = 120;

        private string previousText = "";

        #region Set input character count limit when typing

        private void Start()
        {
            inputField.onValueChanged.AddListener(OnInputValueChanged);
        }

        private void OnInputValueChanged(string newText)
        {
            // Count the number of lines
            int lines = CountLines(newText);

            // Count the number of characters
            int characters = CountCharacters(newText);

            // Check if the number of lines or characters exceeds the limits
            if (lines > _maxLines || characters > _maxCharacters)
            {
                // If exceeded, revert to the previous text
                inputField.text = previousText;
            }
            else
            {
                // Update the previous text
                previousText = newText;
            }
        }

        private int CountLines(string text)
        {
            // Split the text by newline character '\n' and count the number of lines
            return text.Split('\n').Length;
        }

        private int CountCharacters(string text)
        {
            return text.Length;
        }

        #endregion
    }
}