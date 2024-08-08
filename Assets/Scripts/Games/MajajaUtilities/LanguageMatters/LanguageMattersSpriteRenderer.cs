using System;
using UnityEngine;

namespace Majaja.LanguageMatters {

    [RequireComponent(typeof(SpriteRenderer))]
    public class LanguageMattersSpriteRenderer : LanguageMattersSpriteCarrier {

        SpriteRenderer _sr;
        public SpriteRenderer AttachedSR {
            get {
                if (_sr == null) {
                    _sr = GetComponent<SpriteRenderer>();
                }
                return _sr;
            }
        }

        protected override void SetSprite(Sprite sprite) {
            AttachedSR.sprite = sprite;
        }

    }
}
