using Majaja.Utilities;
using UnityEngine;

namespace SlotTemplate
{


    public class ED_AnimatedIconsController : AnimatedIconsController,IDBCollectionReceiver
    {
    
        [SerializeField] ED_GameStatesManager ED_gameStatesManager;

        [SerializeField] ED_BonusSpecialRules ED_bonusSpecialRules;

        int IconCoordsX ;
        /// <summary>
        /// 生成得分線上的Icon動畫物件
        /// </summary>
        /// 
        public override void PrepareAnimatedIcons(int[,] showedIconsId, Vector2Int[] coords)
        {

            for (int i = 0; i < coords.Length; i++)
            {
                int iconId = showedIconsId[coords[i].x, coords[i].y];
                Vector2 position = reelsContainerInfoManager.GetReelIconPosition(coords[i]);
                
                //擴張百搭圖動畫位置為滾筒中間顯示
                if (ED_bonusSpecialRules.isAllFakeReel[coords[i].x] == true) 
                {
                    IconCoordsX = coords[i].x;
                    position.y = 0;
                }

                animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));
            }

            IsPrepared = true;
        }
        protected override GameObject GenerateAnimatedIcon(Vector2 position, int iconId, float deltaPosZ = 0f) 
        {
            
            GameObject animatedIcon = Instantiate(animatedIconPrefab, transform);
            
            animatedIcon.transform.position = position;
            animatedIcon.transform.SetLocalPositionZ(0f);
            animatedIcon.transform.position += Vector3.forward * deltaPosZ;
            //animatedIcon.GetComponent<SpriteRenderer>().sortingOrder = 4;
            //只有Lock住的滾筒需要調整與更換動畫
            if (iconId == 0 && ED_gameStatesManager.statesParameters.isInBonusGame && ED_bonusSpecialRules.isAllFakeReel[IconCoordsX] == true)
            {
                iconId = -1;
                IconCoordsX = 0;
            }
            if (iconId == 1 && ED_gameStatesManager.statesParameters.isInBonusGame)
            {
                iconId = -2;
            }

            animatedIcon.GetComponent<AnimatorOverrider>().OverrideAnimationClipsAndRestart(new AnimationClip[] { animClipsDB.GetAnimationClipById(iconId) });

            return animatedIcon;
        }
    }
}
