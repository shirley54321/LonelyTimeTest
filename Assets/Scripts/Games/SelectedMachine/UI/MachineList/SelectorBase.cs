using Games.Data;
using Player;
using UnityEngine;

namespace Games.SelectedMachine
{
    public abstract class SelectorBase : MonoBehaviour
    {
        [SerializeField] protected Data.MachineInfo _machineInfo;
 
        
        
        #region For Button

        /// <summary>
        /// Set Selected machine.
        /// </summary>
        public void SelectMachine()
        {
            Debug.Log($"Select Machine");
            MachineLobbyMediator.Instance.SetSelectedMachine(_machineInfo, true);
        }

        /// <summary>
        /// Try Enter Selected Machine
        /// </summary>
        public virtual void TryEnterMachine()
        {
            MachineLobbyMediator.Instance.TryEnterSelectedMachine();
        }
        

        #endregion
        
        
        public int GetMachineId()
        {
            return _machineInfo.MachineId;
        }

        /// <summary>
        /// Is this Machine played or Reserved by Current Player
        /// </summary>
        /// <returns></returns>
        protected bool IsUsedByCurrentPlayer()
        {
            return _machineInfo.ReserveMachineInfo.PlayFabId == PlayerInfoManager.Instance.PlayFabId;
        }
    }
}