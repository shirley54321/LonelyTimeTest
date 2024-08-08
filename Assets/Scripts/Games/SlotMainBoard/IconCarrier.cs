using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlotTemplate {

    [RequireComponent(typeof(SpriteRenderer))]
    public class IconCarrier : MonoBehaviour, IDBCollectionReceiver {


        [SerializeField] SpritesDB _iconsSpriteDB;


        int _currentIconId = -1;
        public int CurrentIconId {
            get {
                return _currentIconId;
            }
            set {
                _currentIconId = value;

                OnCurrentIconIdChanged(_currentIconId);
            }
        }


        SpriteRenderer _sr = null;
        public SpriteRenderer SR {
            get {
                if (_sr == null) {
                    _sr = GetComponent<SpriteRenderer>();
                }
                return _sr;
            }
        }



        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB ?? _iconsSpriteDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _iconsSpriteDB = dbCollection.iconsSpriteDB;
        }


        void OnCurrentIconIdChanged (int newId) {
            if (newId==0 && _iconsSpriteDB.GetSpriteById(-1)!=null&& _iconsSpriteDB.name == "MoonWolf_Icons Sprite DB"&& MoonWolfGameStatesManager.IsbonusGame)
            {
                Debug.Log("newIdDDDD" + newId);
                newId = -1;
            }
            if (_iconsSpriteDB != null) {
                SR.sprite = _iconsSpriteDB.GetSpriteById(newId);
            }

            if(newId ==1)//Bouns sprite
            {
                SR.sortingOrder = 1;//如果有fake reel 會放在fake reel前面
            }
            else
            {
                SR.sortingOrder = 0;
            }

            if(newId ==-2 && _iconsSpriteDB.name == "ED_Icons Sprite DB")//ED的加場數icon要在fake reel 上面
            {
                SR.sortingOrder = 1;
            }
        }

    }

}
