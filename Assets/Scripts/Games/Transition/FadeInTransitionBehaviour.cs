using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public class FadeInTransitionBehaviour : OneShotAnimationBehaviour {

        [SerializeField] float _duration = 1f;

        [Header("REFS")]
        [SerializeField] CanvasGroup _canvasGroup;


        protected override void OnEnable () {
            StartCoroutine(Fading(_duration));
        }

        protected override void OnDisable () {

        }

        IEnumerator Fading (float duration) {
            float startTime = Time.time;
            float elapsedTimeRate = 0f;

            while (elapsedTimeRate < 1f) {
                elapsedTimeRate = (Time.time - startTime) / duration;
                _canvasGroup.alpha = 1f - elapsedTimeRate;
                yield return null;
            }

            gameObject.SetActive(false);
        }


        public override void SetParameters (object paramters) {

        }

        public override void Advance () {

        }



    }
}
