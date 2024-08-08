
namespace SlotTemplate{
    /// <summary>
    /// 機台詳細資訊
    /// </summary>
    public interface IMachineDetail
    {
        /// <summary>
        /// 機台編號
        /// </summary>
        public ushort MachineID {get;}
        /// <summary>
        /// 機台類型
        /// </summary>
        public byte MachineType {get;}
        /// <summary>
        /// 遊戲設定編號
        /// </summary>
        public byte Setting {get;}
        /// <summary>
        /// 已經幾輪沒出Bonus
        /// </summary>
        public uint NGC {get;}
        /// <summary>
        /// 最近第1次開出Bonus前的遊戲次數
        /// </summary>
        public uint FirstLBNGC {get;}
        /// <summary>
        /// 最近第2次開出Bonus前的遊戲次數
        /// </summary>
        public uint SecondLBNGC {get;}
        /// <summary>
        /// 最近第3次開出Bonus前的遊戲次數
        /// </summary>
        public uint ThirdLBNGC {get;}
        /// <summary>
        /// 最近第4次開出Bonus前的遊戲次數
        /// </summary>
        public uint FourthLBNGC {get;}
        /// <summary>
        /// 本月開出1000倍以上回饋的次數
        /// </summary>
        public uint CurrentMO1000 {get;}
        /// <summary>
        /// 本月開出500倍以上回饋的次數
        /// </summary>
        public uint CurrentMO500 {get;}
        /// <summary>
        /// 本月開出300倍以上回饋的次數
        /// </summary>
        public uint CurrentMO300 {get;}
        /// <summary>
        /// 本月開出100倍以上回饋的次數
        /// </summary>
        public uint CurrentMO100 {get;}
        /// <summary>
        /// 上月開出1000倍以上回饋的次數
        /// </summary>
        public uint LastMO1000 {get;}
        /// <summary>
        /// 上月開出500倍以上回饋的次數
        /// </summary>
        public uint LastMO500 {get;}
        /// <summary>
        /// 上月開出300倍以上回饋的次數
        /// </summary>
        public uint LastMO300 {get;}
        /// <summary>
        /// 上月開出100倍以上回饋的次數
        /// </summary>
        public uint LastMO100 {get;}
    }
}
