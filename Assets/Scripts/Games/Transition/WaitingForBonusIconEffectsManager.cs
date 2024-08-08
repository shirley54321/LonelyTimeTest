using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class WaitingForBonusIconEffectsManager : MonoBehaviour {

        [Header("Prefabs")]
        [SerializeField] GameObject _waitingForBonusIconEffectPrefab;


        Dictionary<int, GameObject> _currentPlayings = new Dictionary<int, GameObject>();


        public void PlayAt (int reelIndex, Transform reelTransform) {
            if (_waitingForBonusIconEffectPrefab != null) {
                _currentPlayings.Add(reelIndex, Instantiate(_waitingForBonusIconEffectPrefab, reelTransform.position, reelTransform.rotation, transform));
            }
        }

        public void StopAt (int reelIndex) {
            if (_currentPlayings.ContainsKey(reelIndex)) {

                GameObject effectObject = _currentPlayings[reelIndex];
                WaitingForBonusIconEffectController effectController = effectObject.GetComponent<WaitingForBonusIconEffectController>();

                _currentPlayings.Remove(reelIndex);

                if (effectController != null) {
                    effectController.StopThenKill();
                }
                else {
                    Destroy(effectObject);
                }

            }
        }

    }
}
