using UnityEngine;

namespace SlotTemplate
{
    public class ED_BonusSpecialRules : MonoBehaviour
    {
        //所有的假滾筒
        public GameObject[] FakeReelObject = new GameObject[5];

        // 全部需要Lock的滾筒
        bool[] isAllLockReel = new bool[5];

        //記錄所有假滾筒開啟訊息
        public bool[] isAllFakeReel = new bool[5];



        /// <summary>
        /// 所有需要被Lock的Reel
        /// </summary>
        /// <param name="IconID"></param>
        public void RecordLockedReel(int GainedBonusRounds)
        {
            if (GainedBonusRounds != 0 && isAllLockReel[1] == false && isAllLockReel[2] == false && isAllLockReel[3] == false)
            {
                isAllLockReel[1] = true;
            }
            else if (GainedBonusRounds != 0 && isAllLockReel[1] == true && isAllLockReel[2] == false && isAllLockReel[3] == false)
            {
                isAllLockReel[3] = true;
            } 
            else if (GainedBonusRounds != 0 && isAllLockReel[1] == true && isAllLockReel[2] == false && isAllLockReel[3] == true)
            {
                isAllLockReel[2] = true;
            }
            GetAllLockReel();
        }

        /// <summary>
        /// 開關滾筒
        /// </summary>
        public void OpenAndCloseFakeReel()
        {
            //開啟假滾筒
            for (int Reel = 0; Reel < isAllLockReel.Length; Reel++)
            {
                FakeReelObject[Reel].SetActive(isAllLockReel[Reel]);
            }
        }

        /// <summary>
        /// 取得所有已被鎖定的滾筒
        /// </summary>
        public void GetAllLockReel()
        {
            for (int Reel = 0; Reel < FakeReelObject.Length; Reel++)
            {
                isAllFakeReel[Reel] = (FakeReelObject[Reel].activeInHierarchy) ? true : false;
            }
        }

        /// <summary>
        /// 退出Bonus時並關閉所有lock的滾筒
        /// </summary>
        public void LeaveBonusGameunLockReel()
        {
            for (int Reel = 0; Reel < isAllLockReel.Length; Reel++)
            {
                isAllLockReel[Reel] = false;
            }
            OpenAndCloseFakeReel();
            GetAllLockReel();
        }

    }

}
