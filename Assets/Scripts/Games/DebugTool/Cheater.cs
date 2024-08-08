using Games.SelectedMachine;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SlotTemplate {

    public class Cheater : MonoBehaviour {

        [SerializeField] KeyCode _switchingKeyCode;

        [SerializeField] PlayerStatus _playerStatus;

        [Header("Children")]
        [SerializeField] GameObject _cheatingUI;
        [SerializeField] InputField _timeScaleInputField;

        bool isPause = false;

        void Start () {
            _timeScaleInputField.text = TimeScaleController.BaseTimeScale.ToString();
            PauseGame();
        }

        void Update () {
            if (Input.GetKeyDown(_switchingKeyCode)) {
                _cheatingUI.SetActive(!_cheatingUI.activeSelf);
            }
        }

        public void SetTimeScale (string text) {
            float value;
            if (float.TryParse(text, out value)) {
                TimeScaleController.BaseTimeScale = value;
            }
        }

        public void PauseGame()
        {
            if (isPause)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }

            FindObjectOfType<ConnectionScript>().InitGame();
            isPause = !isPause;
            Time.timeScale = 1;
        }
    }

}
