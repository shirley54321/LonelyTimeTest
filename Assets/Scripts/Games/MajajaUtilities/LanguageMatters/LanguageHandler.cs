using System;
using UnityEngine;

namespace Majaja.LanguageMatters {

    public class LanguageHandler : MonoBehaviour {

        string _currentLanguageToken = "";
        public string CurrentLanguageToken {
            get {
                return _currentLanguageToken;
            }
            set {
                _currentLanguageToken = value;

                BroadcastMessage("ChangeLanguageFromLanguageHandler", _currentLanguageToken);
            }
        }


        public class LanguageChangeEventArgs : EventArgs {
            public string newLanguageToken;
        }

    }

}
