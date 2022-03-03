using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;

namespace DicomDirExporter.Repository.View
{
    public class DcmFindStudyLevelViewRepository : Repository<DcmFindStudyLevelView>
    {
        public DcmFindStudyLevelViewRepository() : base("[dbo].[DcmFindStudyLevelView]")
        {
        }
    }
}