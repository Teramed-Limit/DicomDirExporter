using System;
using System.Collections.Generic;
using DicomDirExporter.Interface;
using GalaSoft.MvvmLight;

namespace DicomDirExporter.Model.AppConfig
{
    public class DbSetting
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
    }

    public class ServerSetting
    {
        public string ServerAe { get; set; }
        public int Port { get; set; }
    }

    public class PacsSetting
    {
        public string Name { get; set; }
        public string CallingAe { get; set; }
    }

    public class ServiceBaseSetting : IDicomSCUSetting
    {
        public string CalledAE { get; set; }
        public string CalledIP { get; set; }
        public int CalledPort { get; set; }
        public string CallingAE { get; set; }
    }

    public class CMoveSetting : ServiceBaseSetting
    {
        public string DestinationAE { get; set; }
    }


    public class OtherSetting
    {
        public string DefaultCDViewerPath { get; set; }
        public int ExamKeepDays { get; set; }
        public int DiskMinimumAllowableVolume { get; set; }
    }

    public class Config
    {
        public DbSetting DbSetting { get; set; }
        public List<PacsSetting> PacsSetting { get; set; }
        public ServerSetting ServerSetting { get; set; }
        public OtherSetting OtherSetting { get; set; }
        public CMoveSetting RetrieveSetting { get; set; }       //ADD 20220504 Oscar
    }
}