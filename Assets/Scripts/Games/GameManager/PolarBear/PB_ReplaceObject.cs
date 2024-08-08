using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SlotTemplate
{

    public class PB_ReplaceObject : MonoBehaviour
    {
        [SerializeField] GameDBManager _gameDBManager;
        [SerializeField] GameObject ReelBackGround;
        [SerializeField] GameObject Mask;
        [SerializeField] GameObject Frame;
        

        [Header("BonusDB")]
        [SerializeField] SpritesDB BonusIconsSpriteDB;
        [SerializeField] AnimationClipsDB BonusIconsAnimClipDB;
        [SerializeField] Sprite BonusFrame;
        [SerializeField] Sprite BonusReelBackGround;
        

        [Header("BaseDB")]
        [SerializeField] SpritesDB BaseIconsSpriteDB;
        [SerializeField] AnimationClipsDB BaseIconsAnimClipDB;
        [SerializeField] Sprite BaseFrame;
        [SerializeField] Sprite BaseReelBackGround;


        //Base轉Bonus
        public void BaseToBonus() 
        {
            _gameDBManager.dbCollection.iconsSpriteDB = BonusIconsSpriteDB;
            _gameDBManager.dbCollection.iconsAnimClipDB = BonusIconsAnimClipDB;
            ReelBackGround.GetComponent<SpriteRenderer>().sprite = BonusReelBackGround;
            Mask.GetComponent<SpriteMask>().sprite = BonusReelBackGround;
            Frame.GetComponent<SpriteRenderer>().sprite = BonusFrame;
            AssignDBs();
        }

        public void BonusToBase() 
        {
            _gameDBManager.dbCollection.iconsSpriteDB = BaseIconsSpriteDB;
            _gameDBManager.dbCollection.iconsAnimClipDB = BaseIconsAnimClipDB;
            ReelBackGround.GetComponent<SpriteRenderer>().sprite = BaseReelBackGround;
            Mask.GetComponent<SpriteMask>().sprite = BaseReelBackGround;
            Frame.GetComponent<SpriteRenderer>().sprite = BaseFrame;
            AssignDBs();
        }

        void AssignDBs(bool includeEmpty = false)
        {
            var monoBehaviours = _gameDBManager.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IDBCollectionReceiver)
                {
                    if (includeEmpty)
                    {
                        ((IDBCollectionReceiver)monoBehaviour).OnDBsAssignedByDBCollectionIncludeEmpty(_gameDBManager.dbCollection);
                    }
                    else
                    {
                        ((IDBCollectionReceiver)monoBehaviour).OnDBsAssignedByDBCollection(_gameDBManager.dbCollection);
                    }
                }
            }
            Debug.Log("DBs Assigned" + (includeEmpty ? " (include empty)" : ""));
        }

    }
}
