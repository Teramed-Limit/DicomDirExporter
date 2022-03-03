using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DicomDirExporter.Model.AppConfig;
using Newtonsoft.Json;

namespace DicomDirExporter.Service
{
    public class ConfigService
    {
        public static string ConfigPath = Path.Combine(Application.StartupPath, "settings.json");
        public static string GridDefinePath = Path.Combine(Application.StartupPath, "gridDefine.json");
        public static string TempStoragePath = Path.GetFullPath(@".\tempDCM");
        public static string JobStoragePath = Path.GetFullPath(@".\job");
        public static string StoragePath = Path.GetFullPath(@".\storage");
        public static string ErrorPath = Path.GetFullPath(@".\errorDcm");

        private static ConfigService _instance;
        private readonly Config _config;
        private readonly GridQueryDefine _gridQueryDefine;
        public bool ConfigValid;


        private ConfigService()
        {
            using (var r = new StreamReader(ConfigPath))
            {
                var json = r.ReadToEnd();
                _config = JsonConvert.DeserializeObject<Config>(json);
            }

            using (var r = new StreamReader(GridDefinePath))
            {
                var json = r.ReadToEnd();
                _gridQueryDefine = JsonConvert.DeserializeObject<GridQueryDefine>(json);
            }
        }

        public static ConfigService GetInstance()
        {
            return _instance ?? (_instance = new ConfigService());
        }

        public bool ValidateSetting()
        {
            try
            {
                // sql valid
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    connection.Close();
                }

                // Default CDViewer Path Setting
                if (!string.IsNullOrEmpty(_config.OtherSetting.DefaultCDViewerPath) &&
                    !Directory.Exists(_config.OtherSetting.DefaultCDViewerPath))
                    return false;
            }
            catch
            {
                return false;
            }

            ConfigValid = true;
            return true;
        }

        public Config GetSetting()
        {
            return _config;
        }

        public GridQueryDefine GetGridQueryDefine()
        {
            return _gridQueryDefine;
        }

        public Config GetNewInstanceSetting()
        {
            Config config;
            using (var r = new StreamReader(ConfigPath))
            {
                var json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }

            return config;
        }

        public string GetConnectionString()
        {
            var result = "";
            try
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = _config.DbSetting.ServerName,
                    InitialCatalog = _config.DbSetting.DatabaseName,
                    UserID = _config.DbSetting.UserId,
                    Password = _config.DbSetting.Password
                };
                result = builder.ConnectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public void SaveSetting(Config config)
        {
            using (var fs = File.Open(ConfigPath, FileMode.Truncate))
            using (var sw = new StreamWriter(fs))
            {
                var serializer = new JsonSerializer();
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    serializer.Serialize(jw, config);
                }
            }
        }
    }
}