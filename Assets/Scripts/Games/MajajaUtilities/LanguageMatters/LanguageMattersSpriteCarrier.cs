using System;
using UnityEngine;

namespace Majaja.LanguageMatters {

    public abstract class LanguageMattersSpriteCarrier : MonoBehaviour {

        [SerializeField] LanguageMattersSpritesDB _languageMattersSpritesDB;


        void ChangeLanguageFromLanguageHandler (string languageToken) {
            if (_languageMattersSpritesDB != null) {
                SetSprite(_languageMattersSpritesDB.GetSprite(languageToken));
            }
        }


        protected abstract void SetSprite (Sprite sprite);

    }
}
