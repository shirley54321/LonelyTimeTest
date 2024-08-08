using Spine.Unity;
using UnityEngine;

namespace SlotTemplate
{
    public class WushihLockReelEffects : MonoBehaviour
    {
        [SerializeField] WushihBonusSpecialRules _wushihBonusSpecialRules;
        [SerializeField] GameStatesManager _gameStatesManager;

        //記錄所有鞭炮
        public SkeletonAnimation[] LockReelEffects = new SkeletonAnimation[6];

        public bool[] isAllOpenFakeReel = new bool[WushihBonusSpecialRules.isAllFakeReel.Length];


        public void GetOpenLockReel()
        {
            //當前為BonusGame模式
            if (_gameStatesManager.statesParameters.isInBonusGame)
            {
                //紀錄當前鞭炮開啟的狀況
                for (int Reel = 0; Reel < 5; Reel++)
                {
                    //判斷現在開啟滾筒有那些
                    if (WushihBonusSpecialRules.isAllFakeReel[Reel] == true)
                    {
                        PlayLoopOrPlayLockAnimation(Reel);
                    }
                }
                for (int i = 0; i < WushihBonusSpecialRules.isAllFakeReel.Length; i++) 
                {
                    isAllOpenFakeReel[i] = WushihBonusSpecialRules.isAllFakeReel[i];
                }
            }
        }
        
        public void PlayLoopOrPlayLockAnimation(int Reel)
        {
            // 判斷滾筒右邊鞭炮開啟狀況
            if (LockReelEffects[Reel].gameObject.activeInHierarchy & LockReelEffects[Reel + 1].gameObject.activeInHierarchy & isAllOpenFakeReel[Reel]== WushihBonusSpecialRules.isAllFakeReel[Reel])//已開啟鞭炮
            {
                //鞭炮已啟用時，撥放Loop動畫
                PlayLoopAnimation(LockReelEffects[Reel]);
                PlayLoopAnimation(LockReelEffects[Reel + 1]);
            }
            else //鞭炮為關閉狀態
            {
                //播放鎖定動畫
                PlayLockAnimation(LockReelEffects[Reel]);
                PlayLockAnimation(LockReelEffects[Reel + 1]);
            }
        }

        //播放Loop動畫
        public void PlayLoopAnimation(SkeletonAnimation _skeletonAnimation)
        {
            //_skeletonAnimation.state.SetAnimation(0, "Loop", true);

        }
        //撥放Lock動畫
        public void PlayLockAnimation(SkeletonAnimation _skeletonAnimation)
        {
            _skeletonAnimation.gameObject.SetActive(false);
            _skeletonAnimation.ClearState();
            _skeletonAnimation.gameObject.SetActive(true);
            _skeletonAnimation.state.SetAnimation(0, "Lock", false);
            _skeletonAnimation.state.AddAnimation(0, "Loop", true, 0);
        }

        //關閉所有動畫
        public void CloseReelEffects()
        {
            foreach (var LockReelEffect in LockReelEffects)
            {
                LockReelEffect.gameObject.SetActive(false);
                LockReelEffect.loop = false;
                LockReelEffect.ClearState();
            }
        }


    }


}
