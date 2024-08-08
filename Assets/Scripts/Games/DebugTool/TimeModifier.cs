using UnityEngine;

namespace SlotTemplate {

    public class TimeModifier : MonoBehaviour {

        #if UNITY_EDITOR

            [Range(1f, 100f)]
            [SerializeField] float timeScale1To100 = 1f;
            float _prevTimeScale1To100 = 1f;

            [Range(0f, 2f)] [SerializeField] float timeScale0To2 = 1f;
            float _prevTimeScale0To2 = 1f;

            void Awake () {
                Time.timeScale = 1f;
            }

            void Update () {

                if (_prevTimeScale1To100 != timeScale1To100) {
                    TimeScaleController.BaseTimeScale = timeScale1To100;
                }
                else if (_prevTimeScale0To2 != timeScale0To2) {
                    TimeScaleController.BaseTimeScale = timeScale0To2;
                }

                timeScale1To100 = TimeScaleController.BaseTimeScale;
                timeScale0To2 = TimeScaleController.BaseTimeScale;

                _prevTimeScale1To100 = timeScale1To100;
                _prevTimeScale0To2 = timeScale0To2;
            }

        #endif
    }

}
