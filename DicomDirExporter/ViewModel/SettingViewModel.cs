using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DicomDirExporter.DicomService;
using DicomDirExporter.Interface;
using DicomDirExporter.Logic;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Data.SqlClient;

namespace DicomDirExporter.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class SettingViewModel : ViewModelBase
    {
        private readonly SnackbarMessenger _snackbarMessenger;
        private Config _config;
        private ObservableCollection<PacsSetting> _pacsSettings = new ObservableCollection<PacsSetting>();
        private string _testResult;

        public SettingViewModel(SnackbarMessenger snackbarMessager)
        {
            _snackbarMessenger = snackbarMessager;
            ConfigSet = ConfigService.GetInstance().GetNewInstanceSetting();

            // Command
            SaveCommand = new RelayCommand(OnSaveSetting);
            OpenCDViewerPickerCommand = new RelayCommand(OnOpenDefaultCDViewerPicker);
            RemovePacsSetCommand = new RelayCommand(OnRemovePacsSet);
            AddPacsSetCommand = new RelayCommand(OnAddPacsSet);
            TestSqlConnectCommand = new RelayCommand(OnTestSqlConnection);
            TestCEchoCommand = new RelayCommand(OnTestCEcho);   //ADD 20220505 Oscar
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCDViewerPickerCommand { get; }
        public ICommand AddPacsSetCommand { get; }
        public ICommand RemovePacsSetCommand { get; }
        public ICommand TestSqlConnectCommand { get; }
        public ICommand TestCEchoCommand { get; }   //ADD 20220505 Oscar

        public Config ConfigSet
        {
            get => _config;
            set
            {
                _config = value;
                PacsSettings = new ObservableCollection<PacsSetting>(_config.PacsSetting);
                RaisePropertyChanged(() => ConfigSet);
            }
        }

        public string TestResult
        {
            get => _testResult;
            set
            {
                _testResult = value;
                RaisePropertyChanged(() => TestResult);
            }
        }

        public int SelectedPacsSettingIdx { get; set; }

        public ObservableCollection<PacsSetting> PacsSettings
        {
            get => _pacsSettings;
            set
            {
                _pacsSettings = value;
                RaisePropertyChanged(() => PacsSettings);
            }
        }

        public void OnSaveSetting()
        {
            ConfigService.GetInstance().SaveSetting(_config);
            _snackbarMessenger.ShowSuccessMessage("Save setting success, please restart server.");
        }

        public void OnOpenDefaultCDViewerPicker()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

                ConfigSet.OtherSetting.DefaultCDViewerPath = fbd.SelectedPath + "\\";
                RaisePropertyChanged(() => ConfigSet);
            }
        }


        public void OnAddPacsSet()
        {
            ConfigSet.PacsSetting.Add(new PacsSetting
            { Name = "Name", CallingAe = "SCU" });
            PacsSettings = new ObservableCollection<PacsSetting>(ConfigSet.PacsSetting);
        }

        public void OnRemovePacsSet()
        {
            ConfigSet.PacsSetting.RemoveAt(SelectedPacsSettingIdx);
            PacsSettings = new ObservableCollection<PacsSetting>(ConfigSet.PacsSetting);
        }

        public async void OnTestSqlConnection()
        {
            TestResult = "Testing";
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _config.DbSetting.ServerName,
                InitialCatalog = _config.DbSetting.DatabaseName,
                UserID = _config.DbSetting.UserId,
                Password = _config.DbSetting.Password
            };
            var connectionString = builder.ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    connection.Close();
                    TestResult = "Test connection success.";
                }
                catch (SqlException)
                {
                    TestResult = "Test connection fail.";
                }
            }
        }

        /// <summary>
        /// C-Echo´ú¸Õ ADD 20220505 Oscar
        /// </summary>
        public async void OnTestCEcho()
        {
            TestResult = "Testing";

            IDicomSCUSetting setting = new ServiceBaseSetting();
            setting.CalledAE = _config.RetrieveSetting.CalledAE;
            setting.CallingAE = _config.RetrieveSetting.CallingAE;
            setting.CalledIP = _config.RetrieveSetting.CalledIP;
            setting.CalledPort = _config.RetrieveSetting.CalledPort;
            CEchoSCU<IDicomSCUSetting, string, string> scu = new CEchoSCU<IDicomSCUSetting, string, string>(setting, "", "");
            await scu.Execute();
            TestResult = scu.ResultMessage;
        }
    }
}