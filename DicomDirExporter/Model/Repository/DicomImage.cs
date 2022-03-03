namespace DicomDirExporter.Model.Repository
{
    public class DicomImage
    {
        public string SOPInstanceUID { get; set; }

        public string SeriesInstanceUID { get; set; }

        public string SOPClassUID { get; set; }

        public string InstanceCreationDate { get; set; }

        public string InstanceCreationTime { get; set; }

        public string FilePath { get; set; }

        public string InstanceNumber { get; set; }
    }

}
