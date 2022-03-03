using System.Windows.Controls;

namespace DicomDirExporter.Interface
{
    public interface IPage
    {
        string Name { get; set; }

        string Icon { get; set; }

        UserControl UserControl { get; set; }
    }
}