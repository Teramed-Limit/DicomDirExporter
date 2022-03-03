using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DicomDirExporter.Service;
using DicomDirExporter.View;
using NLog;
using NLog.Config;
using RepoDb;

namespace DicomDirExporter
{
    /// <summary>
    ///     App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var assemblyFolder = System.Windows.Forms.Application.StartupPath;
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(assemblyFolder, "NLog.config"));

            SqlServerBootstrap.Initialize();
            //initialize the splash screen and set it as the application main window
            var splashScreen = new SplashWindow();
            MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                ConfigService.GetInstance().ValidateSetting();
                //we need to do the work in batches so that we can report progress
                for (var i = 1; i <= 100; i++)
                    //simulate a part of work being done
                    Thread.Sleep(30);

                //once we're done we need to use the Dispatcher
                //to create and show the main window
                Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainWindow();
                    MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }
    }
}