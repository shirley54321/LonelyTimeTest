using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine.Star
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite fillStar, hollowStar;

        public void SetFill()
        {
            image.sprite = fillStar;
        }

        public void SetHollow()
        {
            image.sprite = hollowStar;
        }
    }
}