namespace DicomDirExporter.Model.Repository
{
    public class DicomSeries
    {
        public string SeriesInstanceUID { get; set; }

        public string StudyInstanceUID { get; set; }

        public string Modality { get; set; }

        public string SeriesDate { get; set; }

        public string SeriesTime { get; set; }

        public string SeriesNumber { get; set; }

        public string SeriesDescription { get; set; }

        public string PatientPosition { get; set; }

        public string BodyPartExamined { get; set; }
    }
}