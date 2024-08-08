using System;
using System.Collections.Generic;
using Games.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class HallToggleController : MonoBehaviour
    {
        [SerializeField] private HallToggle previousToggle;
        [SerializeField] private HallToggle changingToggle;
        
        [SerializeField] private List<HallToggle> _toggles;
        
        // Subscribes to events when the panel is enabled.
        private void OnEnable()
        {
            ChangeHallHandler.OnTryChangeHall.AddListener(SetChangingToggle);
            ChangeHallHandler.OnVipNotEnough.AddListener(ResetToggle);
            ChangeHallHandler.OnChangeHallSuccess.AddListener(OnChangeHallSuccess);

            SceneManager.sceneLoaded += SetToggleWhenLoadedScene;
        }

        // Unsubscribes from events when the panel is disabled.
        private void OnDisable()
        {
            ChangeHallHandler.OnTryChangeHall.RemoveListener(SetChangingToggle);
            ChangeHallHandler.OnVipNotEnough.RemoveListener(ResetToggle);
            ChangeHallHandler.OnChangeHallSuccess.RemoveListener(OnChangeHallSuccess);
            
            SceneManager.sceneLoaded -= SetToggleWhenLoadedScene;
        }
        
        private void ResetToggle(Hall hall)
        {
            SetToggleIsOnWithoutNotify(previousToggle.hall);
            changingToggle = previousToggle;
        }

        private void SetChangingToggle(Hall hall)
        {
            changingToggle = GetHallToggle(hall);
        }

        private void OnChangeHallSuccess()
        {
            previousToggle = changingToggle;
        }

        private void SetToggleWhenLoadedScene(Scene scene, LoadSceneMode mode)
        {
            Hall currentHall = MachineLobbyMediator.Instance.SelectedHall;
            SetToggleIsOnWithoutNotify(currentHall);
        }
        
        public void SetToggleIsOnWithoutNotify(Hall hall)
        {
            Debug.Log($"SetToggleIsOnWithoutNotify {hall}");
            foreach (var toggle in _toggles)
            {
                toggle.SetIsOnWithoutNotify(toggle.hall == hall);
            }
        }

        private HallToggle GetHallToggle(Hall hall)
        {
            foreach (var toggle in _toggles)
            {
                if (toggle.hall == hall)
                {
                    return toggle;
                }
            }

            Debug.LogError($"Can't Find Hall Toggle {hall}");
            return null;
        }
    }
}