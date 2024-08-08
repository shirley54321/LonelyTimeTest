using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SlotTemplate {

    public class HoldableAdjustmentButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public UnityEvent onTrigger;


        [SerializeField] float _waitingTimeToEnableHoldingEffect = 1f;
        [SerializeField] float _continuousTriggeringIntervalTime = 0.2f;

        bool _isPressed = false;
        float _lastestPressedRealTime = 0f;
        Coroutine _currentHoldingEffect = null;


        void Update () {
            if (_isPressed) {
                if (Time.realtimeSinceStartup - _lastestPressedRealTime > _waitingTimeToEnableHoldingEffect) {
                    if (_currentHoldingEffect == null) {
                        _currentHoldingEffect = StartCoroutine(HoldingEffect());
                    }
                }
            }
        }


        public void OnPointerDown (PointerEventData data) {
            _isPressed = true;
            _lastestPressedRealTime = Time.realtimeSinceStartup;
        }

        public void OnPointerUp (PointerEventData data) {
            _isPressed = false;
            if (_currentHoldingEffect == null) {
                onTrigger.Invoke();
            }
        }


        IEnumerator HoldingEffect () {
            while (_isPressed) {
                onTrigger.Invoke();
                yield return new WaitForSecondsRealtime(_continuousTriggeringIntervalTime);
            }
            _currentHoldingEffect = null;
        }

    }

}
