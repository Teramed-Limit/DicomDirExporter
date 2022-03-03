using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;

namespace DicomDirExporter.Repository.Table
{
    public class DicomImageRepository : Repository<DicomImage>
    {
        public DicomImageRepository() : base("DicomImage")
        {
        }
    }
}