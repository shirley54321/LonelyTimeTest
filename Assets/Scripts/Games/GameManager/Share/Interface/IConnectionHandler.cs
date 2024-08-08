using System;
using System.Data;

namespace SlotTemplate
{

    public interface IConnectionHandler
    {
        // == Events ==
        public event Action<byte[]> DataReceived;

        // The "ushort" parameter is the machine ID
        public event Action MachineEntered; // 公版本身用不到，但負責載入公版的傢伙會用到 (例如公版測試場景中 "Debug Tools" 裡的 "MachineSelectingTools")
        public event Action MachineEnteringFailed; // 公版本身用不到，但負責載入公版的傢伙會用到 (例如公版測試場景中 "Debug Tools" 裡的 "MachineSelectingTools")

        // == Methods ==
        public void MachinePlay(decimal bet, bool status);
        //public void GetMachineDetail(ushort machineId);
        public void EnterMachine();

        // == Properties ==
        public IMachineDetail CurrentMachineDetail { get; }

    }

}

