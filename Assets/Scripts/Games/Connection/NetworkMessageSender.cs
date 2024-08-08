using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    public class NetworkMessageSender : MonoBehaviour
    {

        public IConnectionHandler connectionHandler { get; protected set; }

        public virtual void Init(IConnectionHandler connectionHandler)
        {
            this.connectionHandler = connectionHandler;
        }
        public virtual void SendSpinSlotMessage(decimal bet)
        {
            connectionHandler.MachinePlay(bet, true);
        }
        public virtual void GetMachineDetail(ushort machineId)
        {
            //connectionHandler.GetMachineDetail(2412);
        }

        public virtual void EnterMachine()
        {
            //connectionHandler.EnterMachine();
            ConnectionScript connectionScript = gameObject.AddComponent<ConnectionScript>();
            connectionScript.EnterMachine();
        }
    }
}

