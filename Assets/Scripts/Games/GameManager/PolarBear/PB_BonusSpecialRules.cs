using UnityEngine;

namespace SlotTemplate
{
    public class PB_BonusSpecialRules : MonoBehaviour
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
            StopAnimation();
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

            CloseWildTransAnimation();
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

        void CloseWildTransAnimation() 
        {
            foreach (var a in FakeReelObject) 
            {
                if (a.transform.GetChild(0).gameObject.activeInHierarchy == true) 
                {
                    a.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

        }


        //不播放動畫
        void StopAnimation()
        {
            foreach (var a in FakeReelObject)
            {
                a.transform.GetChild(0).gameObject.GetComponent<Animator>().speed = 0;
            }
        }

        //開啟動畫
        public void OpenAnimation() 
        {
            foreach (var a in FakeReelObject)
            {

                if (a.activeInHierarchy == true)
                {
                    if (a.transform.GetChild(0).gameObject.activeInHierarchy == false)
                    {
                        a.transform.GetChild(0).gameObject.SetActive(true);
                        a.transform.GetChild(0).gameObject.GetComponent<Animator>().speed = 1;
                        a.transform.GetChild(0).gameObject.GetComponent<Animator>().Play(0);
                    }
                }
            }
        }



    }

}
