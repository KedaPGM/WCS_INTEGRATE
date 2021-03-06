﻿using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.role;
using resource;
using System;
using System.Windows;
using tool.mlog;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class ToolBarViewModel : ViewModelBase
    {

        private Log mLog;
        public ToolBarViewModel()
        {
            mLog = (Log)new LogFactory().GetLog("认证", false);
            btnname = "登陆";
            Primary = Application.Current.Resources["ButtonPrimary"] as Style;
            Danger = Application.Current.Resources["ButtonDanger"] as Style;

            btnstyle = Primary;
            LogOutOrInit();//初始化授权取消
        }

        #region[字段]
        private string btnname;
        private string username;
        private Style btnstyle;
        private bool islogin;
        private Style Danger, Primary;
        private bool IsLoginDialogShow = false;
        #endregion

        #region[属性]
        public string BtnName
        {
            get => btnname;
            set => Set(ref btnname, value);
        }

        public string UserName
        {
            get => username;
            set => Set(ref username, value);
        }

        public Style BtnStyle
        {
            get => btnstyle;
            set => Set(ref btnstyle, value);
        }

        #endregion

        #region[命令]
        public RelayCommand LoginOutCmd => new Lazy<RelayCommand>(() => new RelayCommand(DoLoginOut)).Value;
        #endregion

        #region[方法]
        private async void DoLoginOut()
        {
            //登陆
            if (!islogin)
            {
                if (OperateGrandDialogConst.IsOprerateDialogOpen) return;
                if (IsLoginDialogShow) return;
                OperateGrandDialogConst.IsOprerateDialogOpen = true;
                IsLoginDialogShow = true;
                MsgAction result = await HandyControl.Controls.Dialog.Show<OperateGrandDialog>()
                       .Initialize<OperateGrandDialogViewModel>((vm) => { vm.Clear(); vm.SetDialog(false); })
                       .GetResultAsync<MsgAction>();
                IsLoginDialogShow = false;
                OperateGrandDialogConst.IsOprerateDialogOpen = false;
                if (result.o1 is null)
                {
                    Growl.Error("用户密码错误，认证失败！");
                    if (result.o3 is string username)
                    {
                        mLog.Status(true, username + "：用户密码错误，认证失败！");
                    }
                    return;
                }

                //取消认证
                if (result.o1 is int cint)
                {
                    if (result.o3 is string username)
                    {
                        mLog.Status(true, username + "：取消认证！");
                    }
                    return;
                }

                if (result.o1 is WcsUser user)
                {
                    UserName = user.name;
                    MsgAction msg = new MsgAction()
                    {
                        o1 = user
                    };

                    Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);

                    mLog.Status(true, UserName + "：认证成功！");

                    PubMaster.Role.SetLoginUser(user.id);

                    MsgAction allowmsg = new MsgAction()
                    {
                        o1 = PubMaster.Role.MatchRolePrior(WcsRolePrior.管理员, user),
                        o2 = PubMaster.Role.MatchRolePrior(WcsRolePrior.超级管理员, user)
                    };
                    Messenger.Default.Send(allowmsg, MsgToken.AllowShow);
                }

                BtnName = "登出";
                BtnStyle = Danger;
                islogin = true;
            }
            else//退出
            {
                LogOutOrInit();
            }
        }

        /// <summary>
        /// 退出或回复默认用户
        /// </summary>
        private void LogOutOrInit()
        {
            WcsUser guest = PubMaster.Role.GetGuestUser();
            if (guest != null)
            {
                MsgAction msg = new MsgAction()
                {
                    o1 = guest
                };
                Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);

                UserName = guest.name;
                PubMaster.Role.SetLoginUser(guest.id);

                //MsgAction allowmsg = new MsgAction()
                //{
                //    o1 = PubMaster.Role.MatchRolePrior(WcsRolePrior.管理员, guest),
                //    o2 = PubMaster.Role.MatchRolePrior(WcsRolePrior.超级管理员, guest)
                //};
                //Messenger.Default.Send(allowmsg, MsgToken.AllowShow);
            }
            else
            {
                MsgAction msg = new MsgAction()
                {
                    o1 = true
                };
                Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);

                UserName = "普通用户";

                //MsgAction allowmsg = new MsgAction()
                //{
                //    o1 = false,
                //    o2 = false
                //};
                //Messenger.Default.Send(allowmsg, MsgToken.AllowShow);
            }
            BtnName = "登陆";
            BtnStyle = Primary;
            islogin = false;

            //由于是初始化或者登出，所以是普通用户的权限
            MsgAction allowmsg = new MsgAction()
            {
                o1 = false,
                o2 = false
            };
            Messenger.Default.Send(allowmsg, MsgToken.AllowShow);
        }
        #endregion
    }
}
