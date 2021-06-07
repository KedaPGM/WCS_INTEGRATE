﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace tool.appconfig
{
    public class GlobalWcsDataConfig
    {
        public static void Init()
        {
            #region[数据库配置文件读取/初始化]
            if (File.Exists(MysqlConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(MysqlConfig.SavePath);
                    MysqlConfig = (string.IsNullOrEmpty(json) ? new MysqlConfig() : JsonConvert.DeserializeObject<MysqlConfig>(json)) ?? new MysqlConfig();
                }
                catch
                {
                    MysqlConfig = new MysqlConfig();
                }
            }
            else
            {
                MysqlConfig = new MysqlConfig();
            }
            SaveMysqlConfig();
            #endregion

            #region[测试配置文件读取/初始化]
            if (File.Exists(DebugConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(DebugConfig.SavePath);
                    DebugConfig = (string.IsNullOrEmpty(json) ? new DebugConfig() : JsonConvert.DeserializeObject<DebugConfig>(json)) ?? new DebugConfig();
                }
                catch
                {
                    DebugConfig = new DebugConfig();
                }
            }
            else
            {
                DebugConfig = new DebugConfig();
            }
            SaveDebugConfig();
            #endregion

            #region[模拟系统设备信息]
            if (DebugConfig.IsDebug)
            {
                if (File.Exists(string.Format(SimulateConfig.SavePath, MysqlConfig.Database)))
                {
                    try
                    {
                        var json = File.ReadAllText(string.Format(SimulateConfig.SavePath, MysqlConfig.Database));
                        SimulateConfig = (string.IsNullOrEmpty(json) ? new SimulateConfig() : JsonConvert.DeserializeObject<SimulateConfig>(json)) ?? new SimulateConfig();
                    }
                    catch
                    {
                        SimulateConfig = new SimulateConfig();
                    }
                }
                else
                {
                    SimulateConfig = new SimulateConfig();
                }
                SaveSimulateConfig();
            }
            else
            {
                SimulateConfig = new SimulateConfig();
            }
            #endregion

            #region[默认配置信息]
            if (File.Exists(DefaultConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(DefaultConfig.SavePath);
                    DefaultConfig = (string.IsNullOrEmpty(json) ? new DefaultConfig() : JsonConvert.DeserializeObject<DefaultConfig>(json)) ?? new DefaultConfig();
                }
                catch
                {
                    DefaultConfig = new DefaultConfig();
                }
            }
            else
            {
                DefaultConfig = new DefaultConfig();
            }
            SaveDefaultConfig();
            #endregion

            #region[大配置]

            if (File.Exists(BigConifg.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(BigConifg.SavePath);
                    BigConifg = (string.IsNullOrEmpty(json) ? new BigConifg() : JsonConvert.DeserializeObject<BigConifg>(json)) ?? new BigConifg();
                }
                catch
                {
                    BigConifg = new BigConifg();
                }
            }
            else
            {
                BigConifg = new BigConifg();
            }
            SaveBigConifg();

            #endregion
        }

        public static void SaveMysqlConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(MysqlConfig);
                if (!Directory.Exists(MysqlConfig.Path))
                {
                    Directory.CreateDirectory(MysqlConfig.Path);
                }
                using (FileStream fs = new FileStream(MysqlConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }catch(Exception )
            {

            }
            
        }

        public static void SaveDebugConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(DebugConfig);
                if (!Directory.Exists(DebugConfig.Path))
                {
                    Directory.CreateDirectory(DebugConfig.Path);
                }
                using (FileStream fs = new FileStream(DebugConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }catch(Exception )
            {

            }
        }

        public static void SaveSimulateConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(SimulateConfig);
                if (!Directory.Exists(SimulateConfig.Path))
                {
                    Directory.CreateDirectory(SimulateConfig.Path);
                }
                using (FileStream fs = new FileStream(string.Format(SimulateConfig.SavePath, MysqlConfig.Database), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }
            catch (Exception )
            {

            }
        }
        public static void SaveDefaultConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(DefaultConfig);
                if (!Directory.Exists(DefaultConfig.Path))
                {
                    Directory.CreateDirectory(DefaultConfig.Path);
                }
                using (FileStream fs = new FileStream(DefaultConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }
            catch (Exception )
            {

            }
        }
        public static void SaveBigConifg()
        {
            try
            {
                var json = JsonConvert.SerializeObject(BigConifg);
                if (!Directory.Exists(BigConifg.Path))
                {
                    Directory.CreateDirectory(BigConifg.Path);
                }
                using (FileStream fs = new FileStream(BigConifg.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public static MysqlConfig MysqlConfig { get; set; }
        public static DebugConfig DebugConfig { get; set; }
        public static SimulateConfig SimulateConfig { get; set; }
        public static DefaultConfig DefaultConfig { get; set; }
        public static BigConifg BigConifg { get; set; }
    }
}
