using System.Collections.Generic;
using UnityEngine;

namespace Games.SelectedMachine.Star
{
    public class StarLevelUI : MonoBehaviour
    {
        public List<Star> stars;

        public void SetLevel(int level)
        {
            for (var i = 0; i < stars.Count; i++)
            {
                if (i  < level)
                {
                    stars[i].SetFill();
                }
                else
                {
                    stars[i].SetHollow();
                }
            }
        }
    }
}