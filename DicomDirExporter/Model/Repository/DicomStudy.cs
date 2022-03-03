using System;

namespace DicomDirExporter.Model.Repository
{
    public class DicomStudy
    {
        public string StudyInstanceUID { get; set; }

        public string PatientID { get; set; }

        public string StudyDate { get; set; }

        public string StudyTime { get; set; }

        public string AccessionNumber { get; set; }

        public string PerformingPhysicianName { get; set; }

        public string StudyDescription { get; set; }

        public string Modality { get; set; }

        public string StationAETitle { get; set; }

        public string StudyID { get; set; }
    }
}