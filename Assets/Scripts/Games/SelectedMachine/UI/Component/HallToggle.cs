using Games.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class HallToggle : MonoBehaviour
    {
        public Hall hall;
        public Toggle toggle;
        public ToggleColorChangeTMP colorChange;

        public void SetIsOnWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        public void ChangeHall(bool isOn)
        {
            if (isOn)
            {
                MachineLobbyMediator.Instance.TryChangeHall(hall);
            }
        }
    }
}