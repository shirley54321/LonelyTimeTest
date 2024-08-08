using System;
using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public class CoinsHoppingAnimationBehaviour : OneShotAnimationBehaviour {

        [SerializeField] ParticleSystem _coinsHoppingParticleSystem;


        protected override void OnEnable () {
            _coinsHoppingParticleSystem.Play();
        }

        protected override void OnDisable () {}


        public override void SetParameters (object paramters) {}

        public override void Advance () {
            _coinsHoppingParticleSystem.Stop();
            StartCoroutine(WaitingForEnding());
        }

        IEnumerator WaitingForEnding () {
            yield return new WaitUntil(() => _coinsHoppingParticleSystem.particleCount==0);
            gameObject.SetActive(false);
        }
    }
}
