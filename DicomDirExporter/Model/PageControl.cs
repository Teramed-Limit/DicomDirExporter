using System.Windows.Controls;
using DicomDirExporter.Interface;
using GalaSoft.MvvmLight;

namespace DicomDirExporter.Model
{
    public class PageControl : ObservableObject, IPage
    {
        private string _active;

        public string Active
        {
            get => _active;
            set
            {
                _active = value;
                RaisePropertyChanged(() => Active);
            }
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public UserControl UserControl { get; set; }
    }
}