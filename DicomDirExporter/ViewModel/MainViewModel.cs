using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dicom.Network;
using DicomDirExporter.DicomService;
using DicomDirExporter.Logic;
using DicomDirExporter.Model;
using DicomDirExporter.Runner;
using DicomDirExporter.Service;
using DicomDirExporter.View;
using DicomDirExporter.View.Dialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

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
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// private static readonly DispatcherTimer SystemStatusUpdateTimer = new DispatcherTimer();
        private readonly bool _isConfigValid = ConfigService.GetInstance().ConfigValid;

        private UserControl _currentPage;

        private object _dialogContent;

        private bool _isDialogOpen;

        public MainViewModel(SnackbarMessenger snackbarMessager)
        {
            SnackbarMessenger = snackbarMessager;

            // Set starting page
            _pages = new ObservableCollection<PageControl>();
            if (_isConfigValid)
                _pages.Add(new PageControl
                { Name = "History", UserControl = new HistoryPage(), Icon = "History", Active = "Red" });

            _pages.Add(new PageControl
            { Name = "Setting", UserControl = new SettingPage(), Icon = "Cogs", Active = "Black" });

            Pages = _pages;
            CurrentPage = _pages[0].UserControl;

            // Command
            CloseCommand = new RelayCommand<Window>(OnAppClose);
            ToggleWinCommand = new RelayCommand<Window>(ToggleWinSize);
            MinimizedCommand = new RelayCommand<Window>(OnMinimized);
            NavigatePageCommand = new RelayCommand<string>(Navigate);
            OpenAboutDialogCommand = new RelayCommand(OpenAboutDialog);

            if (!_isConfigValid) return;

            var port = ConfigService.GetInstance().GetSetting().ServerSetting.Port;
            DicomServer.Create<CStoreScp>(port);
            new SpaceReleaseRunner();
            new ArchivingRunner();
        }

        private ObservableCollection<PageControl> _pages { get; set; }

        public SnackbarMessenger SnackbarMessenger { get; set; }

        public ICommand CloseCommand { get; }
        public ICommand ToggleWinCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand NavigatePageCommand { get; }
        public ICommand OpenAboutDialogCommand { get; }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                _isDialogOpen = value;
                RaisePropertyChanged(() => IsDialogOpen);
            }
        }

        public object DialogContent
        {
            get => _dialogContent;
            set
            {
                _dialogContent = value;
                RaisePropertyChanged(() => DialogContent);
            }
        }

        public ObservableCollection<PageControl> Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                RaisePropertyChanged(() => Pages);
            }
        }

        public UserControl CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value) return;

                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
            }
        }

        public void OnAppClose(Window win)
        {
            win.Close();
        }

        public void ToggleWinSize(Window win)
        {
            win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public void OnMinimized(Window win)
        {
            win.WindowState = WindowState.Minimized;
        }

        public void OpenAboutDialog()
        {
            DialogContent = new AboutDialog();
            IsDialogOpen = true;
        }

        private void Navigate(string name)
        {
            DrawerHost.CloseDrawerCommand.Execute(null, null);
            foreach (var page in Pages) page.Active = page.Name == name ? "Red" : "Black";
            CurrentPage = _pages.First(x => x.Name == name).UserControl;
        }
    }
}