using UnityEngine;

namespace SlotTemplate {

    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundManager : MonoBehaviour, IDBCollectionReceiver {

        [Header("DBs")]
        [SerializeField] SpritesDB _backgroundSpritesDB;


        SpriteRenderer _sr;
        public SpriteRenderer BackGroundSpriteRenderer {
            get {
                if (_sr == null) {
                    _sr = GetComponent<SpriteRenderer>();
                }
                return _sr;
            }
        }


        void Start () {
            _sr = GetComponent<SpriteRenderer>();
        }


        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _backgroundSpritesDB = dbCollection.backgroundSpritesDB ?? _backgroundSpritesDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _backgroundSpritesDB = dbCollection.backgroundSpritesDB;
        }


        public void ChangeBackground (string typeName) {
            BackGroundSpriteRenderer.sprite = _backgroundSpritesDB.GetSpriteByName(typeName);
        }

    }
}
