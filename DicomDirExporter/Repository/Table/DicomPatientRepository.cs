using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;

namespace DicomDirExporter.Repository.Table
{
    public class DicomPatientRepository : Repository<DicomPatient>
    {
        public DicomPatientRepository() : base("DicomPatient")
        {
        }
    }
}