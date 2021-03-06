﻿using enums;
using GalaSoft.MvvmLight;
using module.device;

namespace module.window.device
{
    public class FerryView : ViewModelBase
    {
        public uint ID { set; get; }
        public uint AreaId { set; get; }
        public ushort LineId { set; get; }
        public string Name { set; get; }
        private bool working;

        #region[逻辑字段]
        private SocketConnectStatusE connstatus;
        private bool isconnect;
        #endregion
        public bool IsConnect
        {
            get => isconnect;
            set => Set(ref isconnect, value);
        }
        public SocketConnectStatusE ConnStatus
        {
            get => connstatus;
            set => Set(ref connstatus, value);
        }

        public bool Working
        {
            get => working;
            set => Set(ref working, value);
        }

        #region[通信字段]
        private byte deviceid;      //设备号
        private DevFerryStatusE devicestatus;  //设备状态
        private string targetsite;  //目标值
        private DevFerryTaskE currenttask;   //当前任务
        private string upsite; //当前左值
        private string downsite; //当前右值
        private DevFerryTaskE finishtask;    //完成任务
        private DevFerryLoadE loadstatus;    //载货状态
        private DevOperateModeE workmode;      //作业模式
        private bool downlight;       //上砖侧光电
        private bool uplight;     //下砖侧光电
        private byte reserve;       //预留

        private uint uptraid;
        private uint downtraid;
        #endregion

        #region[属性]

        public byte DeviceID      //设备号   
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        public DevFerryStatusE DeviceStatus  //设备状态   
        {
            set => Set(ref devicestatus, value);
            get => devicestatus;
        }
        public string TargetSite  //目标值   
        {
            set => Set(ref targetsite, value);
            get => targetsite;
        }
        public DevFerryTaskE CurrentTask   //当前任务   
        {
            set => Set(ref currenttask, value);
            get => currenttask;
        }

        public string UpSite //当前值   
        {
            set => Set(ref upsite, value);
            get => upsite;
        }

        public string DownSite //当前值   
        {
            set => Set(ref downsite, value);
            get => downsite;
        }

        public DevFerryTaskE FinishTask    //完成任务   
        {
            set => Set(ref finishtask, value);
            get => finishtask;
        }
        public DevFerryLoadE LoadStatus    //载货状态   
        {
            set => Set(ref loadstatus,value);
            get => loadstatus;
        }

        public DevOperateModeE WorkMode      //作业模式   
        {
            set => Set(ref workmode, value);
            get => workmode;
        }

        public bool DownLight       //下砖侧光电   
        {
            set => Set(ref downlight, value);
            get => downlight;
        }

        public bool UpLight     //上砖侧光电   
        {
            set => Set(ref uplight, value);
            get => uplight;
        }

        public byte Reserve       //预留   
        {
            set => Set(ref reserve, value);
            get => reserve;
        }

        public uint UpTrackId
        {
            get => uptraid;
            set => Set(ref uptraid, value);
        }
        public uint DownTrackId
        {
            get => downtraid;
            set => Set(ref downtraid, value);
        }
        #endregion

        #region[更新数据]

        internal void Update(DevFerry st, SocketConnectStatusE conn, bool working, uint uptraid, uint downtraid)
        {
            DeviceID = st.DeviceID;
            DeviceStatus = st.DeviceStatus;
            TargetSite =ID +":" + st.TargetSite;
            CurrentTask = st.CurrentTask;
            DownSite = ID + ":" + st.DownSite;
            UpSite = ID + ":" + st.UpSite;
            FinishTask = st.FinishTask;
            LoadStatus = st.LoadStatus;
            WorkMode = st.WorkMode;
            DownLight = st.DownLight;
            UpLight = st.UpLight;
            Reserve = st.Reserve;
            ConnStatus = conn;
            UpTrackId = uptraid;
            DownTrackId = downtraid;
            IsConnect = ConnStatus == SocketConnectStatusE.通信正常;
            Working = working;
        }
        #endregion
    }
}
