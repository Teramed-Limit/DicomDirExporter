/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DicomDirExporter"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using DicomDirExporter.Logic;
using DicomDirExporter.Service;
using DicomDirExporter.ViewModel;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace DicomDirExporter.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Service
            SimpleIoc.Default.Register<HistoryQueryService>();
            SimpleIoc.Default.Register<MainSnackbarMessageQueue>();
            SimpleIoc.Default.Register<SnackbarMessenger>();

            // Window
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SplashWindowViewModel>();

            // Page
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<HistoryPageViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public SettingViewModel Setting => ServiceLocator.Current.GetInstance<SettingViewModel>();
        public HistoryPageViewModel History => ServiceLocator.Current.GetInstance<HistoryPageViewModel>();

        public SplashWindowViewModel SplashWindowViewModel =>
            ServiceLocator.Current.GetInstance<SplashWindowViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}