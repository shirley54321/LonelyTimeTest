using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Majaja.Utilities;

namespace SlotTemplate {

    public class AnimatedIconsController : MonoBehaviour, IDBCollectionReceiver {

        [SerializeField] GameManager _gameManager;

        public bool IsPrepared {get; protected set;} = false;

        [SerializeField] float _spawnedZInterval = 0.0005f;  // Set this value non-zero to let the generated objects won't sit on the same z position.
        protected float spawnedZInterval {
            get => _spawnedZInterval;
            set => _spawnedZInterval = value;
        }

        [Header("DBs")]
        [SerializeField] SlotLinesDB _slotLinesDB;
        protected SlotLinesDB slotLinesDB {
            get => _slotLinesDB;
            set => _slotLinesDB = value;
        }

        [SerializeField] AnimationClipsDB _animClipsDB;
        protected AnimationClipsDB animClipsDB {
            get => _animClipsDB;
            set => _animClipsDB = value;
        }


        [Header("Prefabs")]
        [SerializeField] GameObject _animatedIconPrefab;
        protected GameObject animatedIconPrefab {
            get => _animatedIconPrefab;
            set => _animatedIconPrefab = value;
        }


        [Header("REFS")]
        [SerializeField] ReelsContainerInfoManager _reelsContainerInfoManager;
        protected ReelsContainerInfoManager reelsContainerInfoManager {
            get => _reelsContainerInfoManager;
            set => _reelsContainerInfoManager = value;
        }

        Dictionary<Vector2Int, GameObject> _animatedIcons = new Dictionary<Vector2Int, GameObject>();
        protected Dictionary<Vector2Int, GameObject> animatedIcons {
            get => _animatedIcons;
            set => _animatedIcons = value;
        }


        // Implement the methods from "IDBCollectionReceiver"
        public void OnDBsAssignedByDBCollection (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB ?? _slotLinesDB;
            _animClipsDB = dbCollection.iconsAnimClipDB ?? _animClipsDB;
        }
        public void OnDBsAssignedByDBCollectionIncludeEmpty (GameDBManager.DBCollection dbCollection) {
            _slotLinesDB = dbCollection.slotLinesDB;
            _animClipsDB = dbCollection.iconsAnimClipDB;
        }



        public virtual void ClearAnimatedIcons () {
            IsPrepared = false;

            _animatedIcons.Clear();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
        public virtual void PrepareAnimatedIcons(int[,] showedIconsId, RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo)
        {
            PrepareAnimatedIcons(showedIconsId, GetIconsCoord(showedIconsId, swildIconWinSituationInfo));
        }

        public virtual void PrepareAnimatedIcons (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo) {
            PrepareAnimatedIcons(showedIconsId, GetIconsCoord(showedIconsId, wonSituationsInfo));
        }

        public virtual void PrepareAnimatedIcons (int[,] showedIconsId, Vector2Int[] coords) {

            for (int i = 0 ; i < coords.Length ; i++) {
                int iconId = showedIconsId[coords[i].x, coords[i].y];
                Vector2 position = _reelsContainerInfoManager.GetReelIconPosition(coords[i]);

                _animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -_spawnedZInterval * i));
            }

            IsPrepared = true;
        }

        public virtual void DisplayAnimatedIconsOfWonSituations (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo) {
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationsInfo));
        }

        public virtual void DisplayAnimatedIconsOfWonSituation (int[,] showedIconsId, RoundResultData.WonSituationInfo wonSituationInfo) {
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationInfo));
        }

        public virtual void DisplayAnimatedIcons (Vector2Int[] coords) {
            foreach (var coordAnimatedIconPair in _animatedIcons) {
                coordAnimatedIconPair.Value.SetActive( Array.Exists(coords, coord => coord == coordAnimatedIconPair.Key) );
            }
        }


        protected virtual GameObject GenerateAnimatedIcon (Vector2 position, int iconId, float deltaPosZ = 0f) {
            GameObject animatedIcon = Instantiate(_animatedIconPrefab, transform);


            animatedIcon.transform.position = position;
            animatedIcon.transform.SetLocalPositionZ(0f);
            animatedIcon.transform.position += Vector3.forward * deltaPosZ;
            if (iconId == 0 && _animClipsDB.GetAnimationClipById(-1) != null && _animClipsDB.name == "MoonWolf_Icons Anim Clip DB" && MoonWolfGameStatesManager.IsbonusGame)
            {
                Debug.Log("newIdDDDD" + iconId);
                iconId = -1;
            }


            animatedIcon.GetComponent<AnimatorOverrider>().OverrideAnimationClipsAndRestart(new AnimationClip[] { _animClipsDB.GetAnimationClipById(iconId) });

            return animatedIcon;
        }


        protected virtual Vector2Int[] GetIconsCoord (int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo) {
            List<Vector2Int> totalCoords = new List<Vector2Int>();

            foreach (var info in wonSituationsInfo) {
                totalCoords.AddRange(GetIconsCoord(showedIconsId, info));
            }
            return totalCoords.Distinct().ToArray();
        }

        protected virtual Vector2Int[] GetIconsCoord (int[,] showedIconsId, RoundResultData.WonSituationInfo wonSituationInfo) {
            if (wonSituationInfo is RoundResultData.WonLineInfo) {
                return wonSituationInfo.GetWonIconsCoord(new RoundResultData.WonLineInfo.GettingWonIconsCoordRequiredArgs{
                    slotLinesDB = _slotLinesDB
                });
            }
            else if (wonSituationInfo is RoundResultData.ScatteredIconWinSituationInfo) {
                return wonSituationInfo.GetWonIconsCoord(new RoundResultData.ScatteredIconWinSituationInfo.GettingWonIconsCoordRequiredArgs{
                    showedIconsId = showedIconsId
                });
            }
            return new Vector2Int[0];
        }
        protected virtual Vector2Int[] GetIconsCoord(int[,] showedIconsId, RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo)
        {
            return swildIconWinSituationInfo.GetTotalWonIconsCoord(showedIconsId);
        }
    }

}
