﻿using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.deviceconfig;
using module.msg;
using module.window;
using resource;
using System;
using System.Threading;
using System.Windows;
using task;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    /// <summary>
    /// 砖机转产操作面板
    /// </summary>
    public class GoodShiftDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public GoodShiftDialogViewModel()
        {
            _result = new MsgAction();
            InitTimer();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private uint _devid, _area, _goodsid, _pregoodsid;
        private string _devname, _pregoodsname;
        TileShiftStatusE _shiftstatus;
        private bool shiftbtnenable;

        public uint AREA
        {
            get => _area;
            set => Set(ref _area, value);
        }

        public uint GOODSID
        {
            get => _goodsid;
            set => Set(ref _goodsid, value);
        }

        public string PREGOODSNAME
        {
            get => _pregoodsname;
            set => Set(ref _pregoodsname, value);
        }

        public string DEVNAME
        {
            get => _devname;
            set => Set(ref _devname, value);
        }

        public TileShiftStatusE SHIFTSTATUS
        {
            get => _shiftstatus;
            set => Set(ref _shiftstatus, value);
        }

        public bool SHIFTBTNENABLE
        {
            get => shiftbtnenable;
            set => Set(ref shiftbtnenable, value);
        }

        #region[字段]
        private MsgAction _result;
        #endregion

        #region[属性]

        #endregion

        #region[命令]

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        public RelayCommand<string> BtnCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(BtnAction)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        private void BtnAction(string tag)
        {
            switch (tag)
            {
                case "chosepregood":
                    DoSelectPreGood();
                    break;
                case "refreshstatus":
                    DoRefresh();
                    break;
                case "doshift":
                    DoShift();
                    break;
            }
        }

        /// <summary>
        /// 选择预设品种
        /// </summary>
        private async void DoSelectPreGood()
        {
            uint area = PubMaster.Device.GetDeviceArea(_devid);
            bool isuptilelifter = PubMaster.DevConfig.IsTileWorkMod(_devid, TileWorkModeE.上砖);
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
             .Initialize<GoodsSelectViewModel>((vm) =>
             {
                 vm.SetAreaFilter(area, true);
                 if (isuptilelifter)
                 {
                     vm.QueryStockGood();
                 }
                 else
                 {
                     vm.QueryGood();
                 }
             }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is GoodsView good)
            {
                if (PubMaster.DevConfig.UpdateTilePreGood(_devid, GOODSID, good.ID, out string msg))
                {
                    SetPreGood(string.IsNullOrEmpty(good.info) ? good.Name : good.info, good.ID);
                }
                else
                {
                    Growl.Warning(msg);
                }
            }
        }


        private void SetPreGood(string name, uint id)
        {
            PREGOODSNAME = name;
            _pregoodsid = id;
        }

        private void ClearPreGood()
        {
            PREGOODSNAME = "";
            _pregoodsid = 0;
        }


        /// <summary>
        /// 刷新转产状态
        /// </summary>
        private void DoRefresh()
        {
            SHIFTSTATUS = PubTask.TileLifter.GetTileShiftStatus(_devid);
            if (SHIFTSTATUS != TileShiftStatusE.转产中)
            {
                SHIFTBTNENABLE = true;
            }
        }

        /// <summary>
        /// 执行转产操作
        /// </summary>
        private void DoShift()
        {
            if (_pregoodsid == 0)
            {
                Growl.Info("请选择预设品种！");
                return;
            }

            if (!PubTask.TileLifter.IsOnline(_devid))
            {
                Growl.Warning("砖机离线！不能执行转产操作！");
                return;
            }

            if (!PubMaster.DevConfig.UpdateShiftTileGood(_devid, GOODSID, out string msg))
            {
                Growl.Warning(msg);
                return;
            }
            Growl.Success("开始转产！");
            CloseAction?.Invoke();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void CancelChange()
        {
            StopTimer();
            CloseAction?.Invoke();
        }

        /// <summary>
        /// 设置设备名称和需要过滤的区域
        /// </summary>
        /// <param name="area"></param>
        /// <param name="devname"></param>
        public void SetArea(uint area, uint devid, string devname, uint goodid)
        {
            AREA = area;
            _devid = devid;
            ConfigTileLifter confit = PubMaster.DevConfig.GetTileLifter(devid);
            if (confit == null)
            {
                Growl.Warning("获取不到砖机的配置信息！");
                CancelChange();
                return;
            }
            DEVNAME = devname;
            GOODSID = goodid;
            SHIFTBTNENABLE = false;

            uint pregid = PubMaster.DevConfig.GetDevicePreId(devid);
            if (pregid > 0)
            {
                SetPreGood(PubMaster.Goods.GetGoodsName(pregid), pregid);
            }
            else
            {
                ClearPreGood();
            }

            StartTimer();
        }

        #endregion

        #region[刷新转产状态]

        //定义Timer类
        Timer threadTimer;
        private void InitTimer()
        {
            threadTimer = new Timer(new TimerCallback(TimesUp), null, Timeout.Infinite, 2000);
        }

        private void StartTimer()
        {
            //立即开始计时，时间间隔2000毫秒
            threadTimer.Change(0, 2000);
        }

        private void TimesUp(object value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DoRefresh();
            });
        }

        private void StopTimer()
        {
            //停止计时
            threadTimer.Change(Timeout.Infinite, 2000);
        }
        #endregion
    }
}
