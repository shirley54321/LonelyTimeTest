using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class LinesDisplayController : MonoBehaviour, IDBCollectionReceiver {

        public bool IsPrepared {get; private set;} = false;

        [Header("DBs")]
        [SerializeField] SlotLinesDB _slotLinesDB;

        [Header("Prefabs")]
        [SerializeField] GameObject _lineDisplayPrefab;

        [Header("REFS")]
        [SerializeField] ReelsContainerInfoManager _reelsContainerInfoManager;



        Dictionary<int, GameObject> _lineObjects = new Dictionary<int, GameObject>();



        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB ?? _slotLinesDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB;
        }


        public void ClearLines () {
            IsPrepared = false;
            _lineObjects.Clear();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }

        public void PrepareLines (int[] lineIndices) {
            foreach (int lineIndex in lineIndices) {
                _lineObjects.Add(lineIndex, GenerateLine(lineIndex));
            }
            IsPrepared = true;
        }

        public void DisplayLines (int[] lineIndices) {
            foreach (var lineIndexLineObjectPair in _lineObjects) {
                lineIndexLineObjectPair.Value.SetActive( Array.Exists(lineIndices, lineIndex => lineIndex == lineIndexLineObjectPair.Key) );
            }
        }



        GameObject GenerateLine (int lineIndex) {
            GameObject lineObject = Instantiate(_lineDisplayPrefab, transform).gameObject;
            SlotLinesDB.SlotLine lineData = _slotLinesDB.lines[lineIndex];

            int[] linePositionOnColumns = lineData.positionOnColumns;
            Vector3[] positionOnLine = new Vector3[linePositionOnColumns.Length + 2];


            positionOnLine[0] = _reelsContainerInfoManager.GetLeftEdge(linePositionOnColumns[0]);
            for (int i = 0 ; i < linePositionOnColumns.Length ; i++) {
                positionOnLine[i + 1] = _reelsContainerInfoManager.GetReelIconPosition( new Vector2Int(i, linePositionOnColumns[i]) );
            }
            positionOnLine[positionOnLine.Length - 1] = _reelsContainerInfoManager.GetRightEdge(linePositionOnColumns[linePositionOnColumns.Length - 1]);

            lineObject.GetComponent<LineDisplayController>().DrawLineShape(positionOnLine);

            return lineObject;
        }

    }
}
