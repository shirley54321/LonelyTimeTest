using UnityEngine;

namespace SlotTemplate {

    public static class TimeScaleController {

        static float _baseTimeScale = 1f;
        public static float BaseTimeScale {
            get {
                return _baseTimeScale;
            }
            set {
                _baseTimeScale = value;
                UpdateTimeScale();
            }
        }

        static float _additionalAppliedTimeScale = 1f;
        public static float AdditionalAppliedTimeScale {
            get {
                return _additionalAppliedTimeScale;
            }
            set {
                _additionalAppliedTimeScale = value;
                UpdateTimeScale();
            }
        }


        static void UpdateTimeScale () {
            Time.timeScale = Mathf.Clamp(BaseTimeScale * AdditionalAppliedTimeScale, 0f, 100f);
        }


    }
}
