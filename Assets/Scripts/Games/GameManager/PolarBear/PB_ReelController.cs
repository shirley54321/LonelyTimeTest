using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    public class PB_ReelController : ReelController
    {
        [SerializeField] PB_BonusSpecialRules PB_bonusSpecialRules;

        float IconCarriersZAxis = -0.009f;
        /// <summary>
        /// 複寫-多加Icon圖層順序
        /// </summary>
        /// <param name="showedIconsId"></param>
        public override void SetShowedIconsId(int[] showedIconsId)
        {
            IconCarrier[] iconCarriers = GetAllIconCarriers();
            for (int i = 0; i < iconCarriers.Length; i++)
            {
                if (i < showedIconsId.Length && iconCarriers[i] != null)
                {
                    iconCarriers[i].CurrentIconId = showedIconsId[i];
                    //Wushih-公版沒有
                    if (PB_bonusSpecialRules.isAllFakeReel[this.ReelIndex] == false)
                    {
                        if (iconCarriers[i].CurrentIconId == 1 || iconCarriers[i].CurrentIconId == 0)
                        {
                            iconCarriers[i].SR.sortingOrder = (iconCarriers[i].CurrentIconId == 1) ? 3 : 1;
                        }
                        else 
                        {
                            iconCarriers[i].SR.sortingOrder = 0;
                        }
                    }
                }
            }
        }



        protected override void MoveIcons(Vector3 movement)
        {
            List<IconCarrier> iconCarriersWhoOverTheLimit = new List<IconCarrier>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                //Wushih-公版沒有 Lock住的滾筒除Bonus以外都不顯示調整OrderLayer
                child.gameObject.GetComponent<IconCarrier>().SR.sortingOrder 
                    = (currentGameModeType == GameModeType.BonusGame) ? GetBonusGameIDLayerOrder(child) : 0;

                //移動Icon
                child.localPosition += movement;

                if (Vector3.Dot(child.localPosition, spinDirection) > topToBottomEdgeLocalDistance / 2)
                {
                    iconCarriersWhoOverTheLimit.Add(child.gameObject.GetComponent<IconCarrier>());
                }
            }

            foreach (IconCarrier iconCarrier in iconCarriersWhoOverTheLimit)
            {
                OnIconCarrierSpinnedOverTheLimit(iconCarrier);
            }
        }

        /// <summary>
        /// 滾筒停止時，調整Icon的位置，
        /// 此區將Icon Z軸位置進行調整使圖層有區分
        /// </summary>
        protected override void ResetIconCarriersPosition()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = spinDirection * (topToBottomEdgeLocalDistance / 2 - iconsIntervalLocalDistance * (i + 1));
                IconCarriersZAxis += 0.0001f;
                transform.GetChild(i).localPosition
                    = new Vector3(transform.GetChild(i).localPosition.x, transform.GetChild(i).localPosition.y, IconCarriersZAxis);
            }
            IconCarriersZAxis = -0.009f;

        }

        /// <summary>
        /// IconID決定圖層(除Bouns以外全都隱藏) Wushih-公版沒有
        /// </summary>
        /// <param name="child">Icon物件</param>
        /// <returns></returns>
        int GetBonusGameIDLayerOrder(Transform child)
        {
            int childOrder = 0;
            if (PB_bonusSpecialRules.isAllFakeReel[this.ReelIndex])
            {
                childOrder = (child.gameObject.GetComponent<IconCarrier>().CurrentIconId != 1) ? -1 : 1;
            }
            else
            {
                childOrder = 0;
            }
            return childOrder;
        }

    }
}
