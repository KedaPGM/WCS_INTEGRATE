﻿using System;

namespace tool.appconfig
{
    public class DebugConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\DebugConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";
        /// <summary>
        /// 是否启用调试模式
        /// </summary>
        public bool IsDebug { set; get; } = false;
        public bool DefaultSupervisor { set; get; } = false;
    }
}
