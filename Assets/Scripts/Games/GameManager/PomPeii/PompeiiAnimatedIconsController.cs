using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Majaja.Utilities;
using static SlotTemplate.GameStatesManager;

namespace SlotTemplate
{
    

    public class PompeiiAnimatedIconsController : AnimatedIconsController,IDBCollectionReceiver
    {
        /// <summary>
        /// 是播放BonusAnimationIocn動畫
        /// 否的話會撥放wild動畫
        /// </summary>
        bool isPlayingBonusAnimation;

        /// <summary>
        /// Bonus特殊模式下，Lock的滾筒上出現Bonus時，需要播放wild動畫與Bonus動畫
        /// 此紀錄為指撥放Wild動畫
        /// </summary>
        Dictionary<Vector2Int, GameObject> BonusGameAnimatedIcons = new Dictionary<Vector2Int, GameObject>();

        [SerializeField] GameStatesManager _gameStatesManager;
        public override void ClearAnimatedIcons()
        {
            IsPrepared = false;
            BonusGameAnimatedIcons.Clear();
            animatedIcons.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        /// <summary>
        /// 播放所有Icons動畫用
        /// </summary>
        /// <param name="showedIconsId"></param>
        /// <param name="wonSituationsInfo"></param>
        public override void DisplayAnimatedIconsOfWonSituations(int[,] showedIconsId, RoundResultData.WonSituationInfo[] wonSituationsInfo)
        {
            isPlayingBonusAnimation = true;
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationsInfo));

        }

        /// <summary>
        /// 播放得分線上Icon動畫
        /// </summary>
        /// <param name="showedIconsId"></param>
        /// <param name="wonSituationInfo"></param>
        public override void DisplayAnimatedIconsOfWonSituation(int[,] showedIconsId, RoundResultData.WonSituationInfo wonSituationInfo)
        {
            isPlayingBonusAnimation = wonSituationInfo.wonIconId == 1 ? true:false ;
            DisplayAnimatedIcons(GetIconsCoord(showedIconsId, wonSituationInfo));
        }

        /// <summary>
        /// 播放Icon動畫用與關閉不必要的動畫Icon
        /// </summary>
        /// <param name="coords"></param>
        public override void DisplayAnimatedIcons(Vector2Int[] coords)
        {
            Dictionary<Vector2Int, GameObject> test = new Dictionary<Vector2Int, GameObject>();
            if (isPlayingBonusAnimation==false)
            {
                test = animatedIcons;
                animatedIcons = BonusGameAnimatedIcons;
                BonusGameAnimatedIcons = test;
            }

            foreach (var coordAnimatedIconPair in animatedIcons)
            {
                coordAnimatedIconPair.Value.SetActive(Array.Exists(coords, coord => coord == coordAnimatedIconPair.Key));//內為Bonus
                
            }

            foreach (var coordbonusGameAnimatedIcons in BonusGameAnimatedIcons)
            {
                coordbonusGameAnimatedIcons.Value.SetActive(false);//內為wild
            }

        }

        public override void PrepareAnimatedIcons(int[,] showedIconsId, Vector2Int[] coords)
        {
            for (int i = 0; i < coords.Length; i++)
            {

                int iconId = GetAnimationIconID(i, showedIconsId, coords);

                Vector2 position = reelsContainerInfoManager.GetReelIconPosition(coords[i]);

                
                if (PompeiiBonusSpecialRules.isAllFakeReel[coords[i].x] & iconId == 1)
                {
                    //此變數因為animatedIcons存放
                    BonusGameAnimatedIcons.Add(coords[i], GenerateAnimatedIcon(position, 0, -spawnedZInterval * i));
                }
                else 
                {
                    BonusGameAnimatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));
                }
                //此變數存放所有要顯示Icon動畫
                animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));
            }
            foreach (var TestanimatedIcons in BonusGameAnimatedIcons) 
            {
                TestanimatedIcons.Value.SetActive(false);
            }

            IsPrepared = true;
        }
        /// <summary>
        /// 生成得分線上的Icon動畫物件
        /// </summary>
        protected override GameObject GenerateAnimatedIcon(Vector2 position, int iconId, float deltaPosZ = 0f) 
        {
            
            GameObject animatedIcon = Instantiate(animatedIconPrefab, transform);
            
            animatedIcon.transform.position = position;
            animatedIcon.transform.SetLocalPositionZ(0f);
            animatedIcon.transform.position += Vector3.forward * deltaPosZ;
            //使用IconID來判斷,改變動畫播放層級，使bonus不備起他動畫遮擋
            animatedIcon.transform.GetComponent<SpriteRenderer>().sortingOrder = iconId == 1  ? 3 : 2;
            iconId = (iconId == 0 & _gameStatesManager.statesParameters.isInBonusGame) ? -1:iconId;
            animatedIcon.GetComponent<AnimatorOverrider>().OverrideAnimationClipsAndRestart(new AnimationClip[] { animClipsDB.GetAnimationClipById(iconId) });

            return animatedIcon;
        }


        /// <summary>
        /// 取得場景上bonus數量，來決定重疊部分生成的動畫是甚麼
        /// </summary>
        int GetAnimationIconID(int i, int[,] showedIconsId, Vector2Int[] coords) 
        {
            int iconId;
            if (PompeiiBonusSpecialRules.isAllFakeReel[coords[i].x] == true & showedIconsId[coords[i].x, coords[i].y] == 1)
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
