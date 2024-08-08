using UnityEngine;
using UnityEngine.UI;

namespace SlotTemplate {

    [RequireComponent(typeof(Text))]
    public class VersionNumberShowing : MonoBehaviour {

        Text _textContainter;
        public Text textContainer {
            get {
                _textContainter ??= GetComponent<Text>();
                return _textContainter;
            }
        }


        void OnEnable () {
            textContainer.text = Application.version;
        }

    }
}
