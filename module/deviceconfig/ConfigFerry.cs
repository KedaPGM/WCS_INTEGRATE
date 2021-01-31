﻿namespace module.deviceconfig
{
    public class ConfigFerry
    {
        /// <summary>
        /// 摆渡车 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        public uint track_id { set; get; }

        /// <summary>
        /// 摆渡轨道地标
        /// </summary>
        public ushort track_point { set; get; }


        #region[模拟使用]

        /// <summary>
        /// 摆渡车重启后默认对上的轨道
        /// </summary>
        public ushort sim_last_point { set; get; }

        #endregion
    }
}
