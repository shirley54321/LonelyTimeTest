using UnityEngine;

namespace SlotTemplate
{
    public class WushihBonusSpecialRules : MonoBehaviour
    {
        //所有的假滾筒
        public GameObject[] FakeReelObject = new GameObject[5];
        // 全部需要Lock的滾筒
        bool[] isAllLockReel = new bool[5];
        //記錄所有假滾筒開啟訊息
        static public bool[] isAllFakeReel = new bool[5];

        /// <summary>
        /// 所有需要被Lock的Reel
        /// </summary>
        /// <param name="IconID"></param>
        public void RecordLockedReel(int[,] IconID)
        {
            int WildConut = 0;
            for (int Reel = 0; Reel < IconID.GetLength(0); Reel++)
            {
                for (int row = 0; row < IconID.GetLength(1); row++)
                {
                    if (IconID[Reel, row] == 0)
                        WildConut++;

                }
                isAllLockReel[Reel] = (WildConut == 4 || isAllLockReel[Reel] == true) ? true : false;
                WildConut = 0;
            }
            GetAllLockReel();
        }

        /// <summary>
        /// 開關滾筒
        /// </summary>
        public void OpenAndCloseFakeReel()
        {
            for (int Reel = 0; Reel < isAllLockReel.Length; Reel++)
            {
                FakeReelObject[Reel].SetActive(isAllLockReel[Reel]);
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

    }

}
