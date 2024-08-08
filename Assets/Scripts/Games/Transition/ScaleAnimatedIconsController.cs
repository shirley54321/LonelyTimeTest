using System;
using System.Collections.Generic;
using UnityEngine;
using Majaja.Utilities;

namespace SlotTemplate {

    public class ScaleAnimatedIconsController : MonoBehaviour, IDBCollectionReceiver {

        public bool IsPrepared {get; private set;} = false;

        [SerializeField] float _spawnedZInterval = 0.0005f;

        [Header("DBs")]
        [SerializeField] SpritesDB _iconsSpriteDB;

        [Header("Prefabs")]
        [SerializeField] GameObject _scaleAnimatedIconPrefab;

        [Header("REFS")]
        [SerializeField] ReelsContainerInfoManager _reelsContainerInfoManager;


        Dictionary<Vector2Int, GameObject> _scaleAnimatedIcons = new Dictionary<Vector2Int, GameObject>();



        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB ?? _iconsSpriteDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB;
        }



        public void ClearScaleAnimatedIcons () {
            IsPrepared = false;

            _scaleAnimatedIcons.Clear();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }

        public void Play (int[,] showedIconsId, Vector2Int[] coords) {
            for (int i = 0 ; i < coords.Length ; i++) {
                Vector3 pos = _reelsContainerInfoManager.GetReelIconPosition(coords[i]);
                GenerateScaleAnimatedIcon(pos, showedIconsId[coords[i].x, coords[i].y], i * -_spawnedZInterval);
            }
        }

        GameObject GenerateScaleAnimatedIcon (Vector3 position, int iconId, float localPosZ = 0f) {
            GameObject scaleAnimatedIcon = Instantiate(_scaleAnimatedIconPrefab, transform);

            scaleAnimatedIcon.transform.position = position;
            scaleAnimatedIcon.transform.SetLocalPositionZ(localPosZ);

            SpriteRenderer sr = scaleAnimatedIcon.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sprite = _iconsSpriteDB.GetSpriteById(iconId);
            }

            return scaleAnimatedIcon;
        }

    }

}
