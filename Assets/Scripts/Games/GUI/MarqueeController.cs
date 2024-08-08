using UnityEngine;

namespace SlotTemplate {

    public abstract class MarqueeController : MonoBehaviour, IDBCollectionReceiver {


        [SerializeField] SpritesDB _iconsSpriteDB;
        public SpritesDB iconsSpriteDB => _iconsSpriteDB;


        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB ?? _iconsSpriteDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB;
        }


        public virtual void ShowTotalScore (decimal score) {
            gameObject.SetActive(true);
        }

        public virtual void ShowWonIconAmountAndWonScore (int iconId, int amount, decimal score) {
            gameObject.SetActive(true);
        }

        public void Hide () {
            gameObject.SetActive(false);
        }


    }
}
