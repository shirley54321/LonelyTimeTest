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
        /// �s�u�ʧ@�����ɮɷ|Ĳ�o�A�p�GOnConnectedArg����Error�ѼƫD�šA�N��s�u���ѡA�Ϥ��h���s�u���\�C
        /// </summary>
        public event ConnectionEvent OnConnected;
        /// <summary>
        /// �s�u�����\�����Ʈɷ|Ĳ�o�A���쪺��ƴN�bOnDataReceivedArg����Data�ѼƤ��A�O���쪺��l��ơA�u���L�ѱK�ʧ@�A�|���ѽX�C
        /// ���L�ثe�[�K�t��k�٦b��s���A�ثe�S���[�K�C
        /// </summary>
        public event DataReceivedEvent OnDataReceived;
        /// <summary>
        /// ��s�u���_�ɷ|Ĳ�o�A�p�GOnClosedArg����Error�ѼƫD�šA�N��s�u�����`���_�AError�Y���o�ͪ����`���p�C
        /// </summary>
        public event ConnectionEvent OnClosed;
    }
}
