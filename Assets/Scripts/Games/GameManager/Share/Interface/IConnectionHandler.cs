using System;
using System.Data;

namespace SlotTemplate
{

    public interface IConnectionHandler
    {
        // == Events ==
        public event Action<byte[]> DataReceived;

        // The "ushort" parameter is the machine ID
        public event Action MachineEntered; // ���������Τ���A���t�d���J�������å�|�Ψ� (�Ҧp�������ճ����� "Debug Tools" �̪� "MachineSelectingTools")
        public event Action MachineEnteringFailed; // ���������Τ���A���t�d���J�������å�|�Ψ� (�Ҧp�������ճ����� "Debug Tools" �̪� "MachineSelectingTools")

        // == Methods ==
        public void MachinePlay(decimal bet, bool status);
        //public void GetMachineDetail(ushort machineId);
        public void EnterMachine();

        // == Properties ==
        public IMachineDetail CurrentMachineDetail { get; }

    }

}

