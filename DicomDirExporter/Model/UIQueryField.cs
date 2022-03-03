using System;
using RepoDb;
using RepoDb.Enumerations;

namespace DicomDirExporter.Model
{
    public class UIQueryField : QueryField
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; }
        public string UIType { get; set; }

        public UIQueryField(string fieldName, object value, bool visible, string type, string label) : base(fieldName, value)
        {
            Label = label;
            Value = "";
            Visible = visible;
            UIType = type;
        }

        public UIQueryField(string fieldName, Operation operation, object value, bool visible, string type, string label) : base(fieldName, operation, value)
        {
            Label = label;
            Value = "";
            Visible = visible;
            UIType = type;
        }

        public UIQueryField(Field field, object value, bool visible, string type, string label) : base(field, value)
        {
            Label = label;
            Value = "";
            Visible = visible;
            UIType = type;
        }

        public UIQueryField(Field field, Operation operation, object value, bool visible, string type, string label) : base(field, operation, value)
        {
            Label = label;
            Value = "";
            Visible = visible;
            UIType = type;
        }

        public QueryField ConvertToQueryField()
        {
            return new QueryField(base.Field.Name, Operation.Equal, Value);
        }
    }
}