using System;
using UnityEngine;

namespace SlotTemplate {

    public class RoundResultDataLogShower : MonoBehaviour {

        [SerializeField] NetworkDataReceiver _networkDataReceiver;

        void OnEnable () {
            _networkDataReceiver.RoundResultDataLoaded += OnRoundResultDataLoaded;
        }

        void OnDisable () {
            _networkDataReceiver.RoundResultDataLoaded -= OnRoundResultDataLoaded;
        }



        void OnRoundResultDataLoaded (object sender, NetworkDataReceiver.RoundResultDataLoadedEventArgs args) {
            Debug.Log(args.roundResultData);
        }
    }
}
