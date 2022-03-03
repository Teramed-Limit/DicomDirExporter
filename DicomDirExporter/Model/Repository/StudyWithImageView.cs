using System;
using System.Globalization;

namespace DicomDirExporter.Model.Repository
{
    public class StudyWithImageView
    {
        private string _modality;
        private string _patientID;
        private string _patientName;
        private string _studyDate;
        private string _studyDescription;
        private string _studyTime;

        public string PatientID
        {
            get => _patientID;
            set => _patientID = value?.Trim();
        }

        public string PatientName
        {
            get => _patientName;
            set => _patientName = value?.Trim();
        }

        public string PatientBirthDate { get; set; }

        public string PatientSex { get; set; }

        public string PatientAge { get; set; }

        public string StudyInstanceUID { get; set; }

        public string StudyDate
        {
            get => _studyDate;
            set => _studyDate = value?.Trim();
        }

        public string StudyTime
        {
            get => _studyTime;
            set => _studyTime = value?.Trim();
        }

        public string AccessionNumber { get; set; }

        public string PerformingPhysicianName { get; set; }

        public string StudyDescription
        {
            get => _studyDescription;
            set => _studyDescription = value?.Trim();
        }

        public string Modality
        {
            get => _modality;
            set => _modality = value?.Trim();
        }

        public string SeriesInstanceUID { get; set; }

        public string SOPInstanceUID { get; set; }

        public string FilePath { get; set; }

        public string StationAETitle { get; set; }

        public DateTime LastReceivedTime { get; set; }

        public string InstanceNumber { get; set; }
    }

}