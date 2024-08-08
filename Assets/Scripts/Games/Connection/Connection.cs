using System;
using System.Threading;
using UnityEngine;

namespace SlotTemplate
{
    //public delegate void DataReceivedEvent(object sender, byte[] data, Exception ex);
    public delegate void DataReceivedEvent(object sender, RecieveDataResult data, Exception ex);
    public delegate void ConnectionEvent(object sender, Exception ex);
    public class Connection
    {
        private readonly Thread recieveThred = new Thread(RecieveNetwork) { Name = "ConnectionReceive" };
        //private static readonly Thread sendThread = new Thread(PFSendWork) { Name = "ConnectionReceive" };
        private readonly CancellationTokenSource token = new CancellationTokenSource();

        public static AutoResetEvent connectedEvent = new AutoResetEvent(false);
        AutoResetEvent dataRecieviedEvent = new AutoResetEvent(false);
        public void Start()
        {
            recieveThred.Start(this);
        }

        public static byte[] sendData = null;
        public static RecieveDataResult sendDataResult = null;
        private static void RecieveNetwork(object sender)
        {
            Connection self = (Connection)sender;
            self.OnConnected?.Invoke(self, null);
            connectedEvent.WaitOne();

            self.dataRecieviedEvent.Set();
            self.OnDataReceived?.Invoke(sender, sendDataResult, null);

            self.dataRecieviedEvent.Close();
        }

        public void Close()
        {
            token.Cancel();
        }

        /// <summary>
        /// 連線動作完成時時會觸發，如果OnConnectedArg中的Error參數非空，代表連線失敗，反之則為連線成功。
        /// </summary>
        public event ConnectionEvent OnConnected;
        /// <summary>
        /// 連線中成功接到資料時會觸發，接到的資料就在OnDataReceivedArg中的Data參數中，是接到的原始資料，只做過解密動作，尚未解碼。
        /// 不過目前加密演算法還在研究中，目前沒有加密。
        /// </summary>
        public event DataReceivedEvent OnDataReceived;
        /// <summary>
        /// 當連線中斷時會觸發，如果OnClosedArg中的Error參數非空，代表連線為異常中斷，Error即為發生的異常狀況。
        /// </summary>
        public event ConnectionEvent OnClosed;
    }
}
