using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace DicomDirExporter.View
{
    /// <summary>
    ///     SettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SettingPage : UserControl
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void AllowNumber(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}