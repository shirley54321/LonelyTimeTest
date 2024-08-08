using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlotTemplate
{
    public class AL_AnimatedIconController : AnimatedIconsController
    {
        public GameInfoDB _gameInfo;
        public GameObject _elves;

        [SerializeField] AL_SwildController _swildController;
        public override void PrepareAnimatedIcons(int[,] showedIconsId, RoundResultData.SwildIconWinSituationInfo swildIconWinSituationInfo)
        {
            Vector2Int[] coords = GetIconsCoord(showedIconsId, swildIconWinSituationInfo);
            PrepareAnimatedIcons(showedIconsId, coords);
            _swildController.StartSmokeAnimationPlaying(coords);

            var ani = _elves.GetComponent<SkeletonAnimation>();
            ani.AnimationState.SetAnimation(0, "ElvesInhale", false);
            ani.timeScale = 2;
            ani.AnimationState.Complete += (operation) =>
            {
                ani.AnimationState.SetAnimation(0, "ElvesStandby", true);
                ani.timeScale = 1;
            };
        }

        public IEnumerator ElvesAnimation()
        {
            
            yield return null;
        }
    }
}
