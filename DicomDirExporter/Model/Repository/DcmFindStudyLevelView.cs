namespace DicomDirExporter.Model.Repository
{
    public class DcmFindStudyLevelView
    {
        private string _modality;
        private string _patientID;
        private string _patientName;
        private string _studyDate;
        private string _studyDescription;
        private string _studyTime;
        private string _accessionNumber;
        private string _performingPhysicianName;
        private string _studyInstanceUID;

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

        public string StudyInstanceUID
        {
            get => _studyInstanceUID;
            set => _studyInstanceUID = value?.Trim();
        }

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

        public string AccessionNumber
        {
            get => _accessionNumber;
            set => _accessionNumber = value?.Trim();
        }

        public string PerformingPhysicianName
        {
            get => _performingPhysicianName;
            set => _performingPhysicianName = value?.Trim();
        }

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
    }

}