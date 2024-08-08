using System;
using UnityEngine;
using UnityEngine.UI;

namespace Majaja.LanguageMatters {

    [RequireComponent(typeof(Image))]
    public class LanguageMattersUGUIImage : LanguageMattersSpriteCarrier {

        Image _image;
        public Image AttachedImage {
            get {
                if (_image == null) {
                    _image = GetComponent<Image>();
                }
                return _image;
            }
        }

        protected override void SetSprite(Sprite sprite) {
            AttachedImage.sprite = sprite;
        }

    }
}
