using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SlotTemplate {

    public class IconPaysDisplayManager : MonoBehaviour, IDBCollectionReceiver {

        [SerializeField] int _targetIconId;
        [SerializeField] SequenceType _sequenceType = SequenceType.Desccending;

        [Header("DBs")]
        [SerializeField] PayTableDB _payTableDB;


        GameManager _gameManager;
        public GameManager gameManager {
            get {
                if (_gameManager == null) {
                    _gameManager = GetComponentInParent<GameManager>();
                }
                return _gameManager;
            }
        }


        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _payTableDB = dbCollection.payTableDB ?? _payTableDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _payTableDB = dbCollection.payTableDB;
        }


        void OnEnable () {
            RefreshDisplay();
        }

        void RefreshDisplay () {

            int[] pays = _payTableDB.GetPaysInfoById(_targetIconId);

            TextMeshProUGUI[] textContainers = GetTextContainters();

            for (int i = 0 ; i < textContainers.Length ; i++) {
                int payIndex = -1;
                if (_sequenceType == SequenceType.Ascending) {
                    payIndex = i;
                }
                else if (_sequenceType == SequenceType.Desccending) {
                    payIndex = pays.Length - 1 - i;
                }

                if (payIndex < pays.Length) {
                    //print("Bet Rate: "+ gameManager.playerStatus.BetRate);
                    textContainers[i].text = (pays[payIndex] * gameManager.playerStatus.BetRate).ToString();
                }
            }
        }


        TextMeshProUGUI[] GetTextContainters () {
            List<TextMeshProUGUI> resultList = new List<TextMeshProUGUI>();

            foreach (Transform child in transform) {
                resultList.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
            }

            return resultList.ToArray();
        }



        public enum SequenceType {
            Ascending,
            Desccending
        }

    }
}
