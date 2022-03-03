using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;

namespace DicomDirExporter.Repository.Table
{
    public class DicomSeriesRepository : Repository<DicomSeries>
    {
        public DicomSeriesRepository() : base("DicomSeries")
        {
        }
    }
}