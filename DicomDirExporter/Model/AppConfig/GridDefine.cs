using System.Collections.Generic;
using RepoDb.Enumerations;

namespace DicomDirExporter.Model.AppConfig
{
    public class ColumnDefine
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public bool Visible { get; set; }
        public string Type { get; set; }
        public Operation Operation { get; set; }
        public string DataType { get; set; }
    }

    public class GridQueryDefine
    {
        public List<ColumnDefine> HistoryGridQueryCondition { get; set; }
    }
}