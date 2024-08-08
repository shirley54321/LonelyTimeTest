using UnityEngine;

namespace SlotTemplate
{
    public class ED_ReelController : ReelController
    {

        [SerializeField] GameStatesManager _gameStatesManager;

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
                    iconCarriers[i].CurrentIconId = showedIconsId[i] == 1 & _gameStatesManager.statesParameters.isInBonusGame ? -2: showedIconsId[i];
                    //iconCarriers[i].SR.sortingOrder = (iconCarriers[i].CurrentIconId == 0|| iconCarriers[i].CurrentIconId == -1) ? 1 : (iconCarriers[i].CurrentIconId == 1)?2:0;
                }
            }
        }

        protected override void OnIconCarrierSpinnedOverTheLimit(IconCarrier iconCarrier)
        {
            float deltaDistanceFromBottom = Vector3.Dot(iconCarrier.transform.localPosition, spinDirection) - topToBottomEdgeLocalDistance / 2;
            iconCarrier.transform.localPosition = spinDirection * (deltaDistanceFromBottom % topToBottomEdgeLocalDistance - topToBottomEdgeLocalDistance / 2);
            iconCarrier.transform.SetAsLastSibling();

            int newId = -1;


            if (isPreparingToStopManually && pendingTargetIconsId != null)
            {

                if (pendingTargetIconsId.Count > 0)
                {

                    newId = pendingTargetIconsId[0];
                    pendingTargetIconsId.RemoveAt(0);
                }
                else if (!isStartingToGoStopping)
                {

                    isStartingToGoStopping = true;
                    return;
                }
            }
            else
            {
                // newId = GetRandomIconId();
                newId = GetNextIconIdOfGameModeType(currentGameModeType);
            }
            newId = (newId == 1 & _gameStatesManager.statesParameters.isInBonusGame) ? -2 : newId;
            iconCarrier.CurrentIconId = newId;
        }

    }
}
