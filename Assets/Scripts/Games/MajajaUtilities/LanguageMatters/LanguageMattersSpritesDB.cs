using System;
using UnityEngine;

namespace Majaja.LanguageMatters {

    [CreateAssetMenu(fileName = "LanguageMattersSpritesDB", menuName = "ScriptableObject/LanguageMattersSpritesDB")]
    public class LanguageMattersSpritesDB : ScriptableObject {

        public LanguageSpritePair[] pairs;

        public Sprite GetSprite (string languageToken) {
            foreach (var pair in pairs) {
                if (pair.languageToken == languageToken) {
                    return pair.sprite;
                }
            }
            return null;
        }


        [Serializable]
        public class LanguageSpritePair {
            public string languageToken;
            public Sprite sprite;
        }

    }
}
