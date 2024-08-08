using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Majaja.Utilities;

namespace SlotTemplate
{
    

    public class PB_AnimatedIconsController : AnimatedIconsController
    {

        [SerializeField] PB_GameStatesManager PB_gameStatesManager;

        [SerializeField] PB_BonusSpecialRules PB_bonusSpecialRules;

        //是否播放Bonus動畫
       public  bool isPlayingBonusAnimation;

        //此盤面無Bonus
        Dictionary<Vector2Int, GameObject> BonusGameAnimatedIcons = new Dictionary<Vector2Int, GameObject>();

        //重製
        public override void ClearAnimatedIcons()
        {
            IsPrepared = false;
            //公版沒有
            BonusGameAnimatedIcons.Clear();
            animatedIcons.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        //Run All Won Icon Animation
        public override void DisplayAnimatedIconsOfWonSituations(int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo)
        {
            isPlayingBonusAnimation = wonSituationsInfo[0].wonIconId == 1? true : false;
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationsInfo));
        }

        //Run Won Line Animation
        public override void DisplayAnimatedIconsOfWonSituation(int[,] showedIconsId, RoundResultData.WonSituationInfo wonSituationInfo)
        {
            isPlayingBonusAnimation = wonSituationInfo.wonIconId == 1 ? true : false;
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationInfo));
        }

        //啟用對應位置的動畫
        public override void DisplayAnimatedIcons(Vector2Int[] coords)
        {
            if (isPlayingBonusAnimation)
            {
                foreach (var coordAnimatedIconPair in animatedIcons)
                {
                    coordAnimatedIconPair.Value.SetActive(Array.Exists(coords, coord => coord == coordAnimatedIconPair.Key));
                }
                foreach (var coordAnimatedIconPair in BonusGameAnimatedIcons)
                {
                    coordAnimatedIconPair.Value.SetActive(false);
                }
            }
            else 
            {
                foreach (var coordAnimatedIconPair in BonusGameAnimatedIcons)
                {
                    coordAnimatedIconPair.Value.SetActive(Array.Exists(coords, coord => coord == coordAnimatedIconPair.Key));
                }
                foreach (var coordAnimatedIconPair in animatedIcons)
                {
                    coordAnimatedIconPair.Value.SetActive(false);
                }
            }
        }

        //生成動畫物件
        public override void PrepareAnimatedIcons(int[,] showedIconsId, Vector2Int[] coords)
        {

            for (int i = 0; i < coords.Length; i++)
            {
                int iconId = showedIconsId[coords[i].x, coords[i].y];
                Vector2 position = reelsContainerInfoManager.GetReelIconPosition(coords[i]);

                if (iconId == 0 && PB_bonusSpecialRules.isAllFakeReel[coords[i].x])
                {
                    position.y = 0;
                    animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, -1, -spawnedZInterval * i));
                    BonusGameAnimatedIcons.Add(coords[i], GenerateAnimatedIcon(position, -1, -spawnedZInterval * i));
                }
                else 
                {
                    animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));
                    if (iconId == 1 && PB_bonusSpecialRules.isAllFakeReel[coords[i].x])
                    {
                        position.y = 0;
                        BonusGameAnimatedIcons.Add(coords[i], GenerateAnimatedIcon(position, -1, -spawnedZInterval * i));
                    }
                    else 
                    {
                        BonusGameAnimatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));

                    }
                }

            }

            foreach (var TestanimatedIcons in BonusGameAnimatedIcons)
            {
                TestanimatedIcons.Value.SetActive(false);
            }
            foreach (var TestanimatedIcons in animatedIcons)
            {
                TestanimatedIcons.Value.SetActive(false);
            }

            IsPrepared = true;
        }

        //動畫物件設定(位置、動畫ID等)
        protected override GameObject GenerateAnimatedIcon(Vector2 position, int iconId, float deltaPosZ = 0f)
        {
            GameObject animatedIcon = Instantiate(animatedIconPrefab, transform);

            animatedIcon.transform.position = position;
            animatedIcon.transform.SetLocalPositionZ(0f);
            animatedIcon.transform.position += Vector3.forward * deltaPosZ;
            if (iconId == 1) 
            {
                animatedIcon.GetComponent<SpriteRenderer>().sortingOrder = 8;
            }

            animatedIcon.GetComponent<AnimatorOverrider>().OverrideAnimationClipsAndRestart(new AnimationClip[] { animClipsDB.GetAnimationClipById(iconId) });

            return animatedIcon;
        }


        //取得當前IconID
        int GetAnimationIconID(int i, int[,] showedIconsId, Vector2Int[] coords) 
        {
            int iconId;
            if (PB_bonusSpecialRules.isAllFakeReel[coords[i].x] == true && showedIconsId[coords[i].x, coords[i].y] == 1)
            {
                int BonusConut = 0;
                for (int a = 0; a < coords.Length; a++)
                {
                    if (showedIconsId[coords[a].x, coords[a].y] == 1)
                    {
                        BonusConut++;
                    }
                }
                iconId = BonusConut >= 3 ? 1 : 0;
            }
            else
            {
                iconId = showedIconsId[coords[i].x, coords[i].y];
            }

            return iconId;
        }
    }


}
