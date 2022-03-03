using System;
using DicomDirExporter.Enumerate;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;
using RepoDb;

namespace DicomDirExporter.Repository.Table
{
    public class DicomStudyRepository : Repository<DicomStudy>
    {
        public DicomStudyRepository() : base("DicomStudy")
        {
        }
    }
}