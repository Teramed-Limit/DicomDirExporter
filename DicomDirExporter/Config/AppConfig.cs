using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace storescp.Config
{
    internal class AppConfig
    {
        private AppConfig()
        {
            // / To register all default providers:
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            // var player = config.GetSection("Player").GetConnectionString();
            // var appSetting = config.
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<RootSetting>(myJsonResponse); 
    public class DbSetting
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
    }

    public class PacsSetting
    {
        public string CallingAe { get; set; }
        public string CalledAe { get; set; }
        public string Port { get; set; }
        public int IP { get; set; }
        public bool SupportMp4 { get; set; }
        public string CdViewerPath { get; set; }
        public int DelayTime { get; set; }
    }

    public class SoundSetting
    {
        public string JobSuccess { get; set; }
        public string JobFailed { get; set; }
        public string SystemError { get; set; }
    }

    public class OtherSetting
    {
        public int ExamKeepDays { get; set; }
        public int DiskMinimumAllowableVolume { get; set; }
        public string CdCoverFilePath { get; set; }
    }

    public class RootSetting
    {
        public DbSetting DbSetting { get; set; }
        public List<PacsSetting> PacsSetting { get; set; }
        public SoundSetting SoundSetting { get; set; }
        public OtherSetting OtherSetting { get; set; }
    }


}