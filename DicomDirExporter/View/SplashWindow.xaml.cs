using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DicomDirExporter
{
    /// <summary>
    /// LoginWindow.xaml原本的Login改為應用在Splash畫面
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();

            //建立欄位故事板
            Duration duration = new Duration(TimeSpan.FromMilliseconds(500));
            //讀取及載入檔案文字閃爍故事板
            FilesLoadingStoryboard = new Storyboard();
            DoubleAnimation recAnimation = new DoubleAnimation
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                Duration = duration,
                From = 1,
                To = 0,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            FilesLoadingStoryboard.Children.Add(recAnimation);
            Storyboard.SetTarget(recAnimation, lbLoginTipMessage);
            Storyboard.SetTargetProperty(recAnimation, new PropertyPath("Opacity"));
            FilesLoadingStoryboard.Begin();
        }

        #region Fields
        /// <summary>
        /// 錄影文字閃爍動畫故事板
        /// </summary>
        private readonly Storyboard FilesLoadingStoryboard;
        #endregion

        #region Methods

        #endregion
    }
}
