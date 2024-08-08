using UnityEngine;

namespace SlotTemplate {

    [DisallowMultipleComponent]
    public class BonusGameMultiplierDisplayController : MonoBehaviour, IDBCollectionReceiver {


        [Header("DBs")]
        [SerializeField] AnimationClipsDB _multiplierAnimClipsDB;

        [Header("REFS")]
        [SerializeField] GameObject _isTheMaxNumberDisplay;
        [SerializeField] AnimatorOverrider _multiplierAnimatorOverrider;
        [SerializeField] SpriteRenderer MultiplierDisplaying;
        private Sprite InitImage;

        void Start()
        {
            InitImage = MultiplierDisplaying.sprite;
        }
         void OnDisable()
        {
            MultiplierDisplaying.sprite = InitImage;
            AnimationClip animClip = _multiplierAnimClipsDB.GetAnimationClipByName("X" + 1);

            _multiplierAnimatorOverrider.OverrideAnimationClipsAndRestart(new AnimationClip[] { animClip });
        }

        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _multiplierAnimClipsDB = dbCollection.bonusGameMultipliersAnimationClipDB ?? _multiplierAnimClipsDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _multiplierAnimClipsDB = dbCollection.bonusGameMultipliersAnimationClipDB;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Show(int multiplierNumber, bool isTheMaxNumber = false) {
            
            if(_multiplierAnimClipsDB != null)
            {
                gameObject.SetActive(true);

                _isTheMaxNumberDisplay.SetActive(!isTheMaxNumber);

                AnimationClip animClip = _multiplierAnimClipsDB.GetAnimationClipByName("X" + multiplierNumber);

                _multiplierAnimatorOverrider.OverrideAnimationClipsAndRestart(new AnimationClip[] { animClip });
            }
        }

        public void Hide () {
            gameObject.SetActive(false);
        }

    }
}
