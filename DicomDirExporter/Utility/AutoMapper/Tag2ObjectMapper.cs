using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DicomDirExporter.Utility.DicomHelper;
using Dicom;

namespace DicomDirExporter.Utility.AutoMapper
{
    //  <summary>
    //      DICOM Tag keyword 自動轉換成 object，確定 Tag name 等於 object name，value一律為string
    //  </summary>
    public class PropertyMap
    {
        public KeyValuePair<string, string> SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
    }

    public class Tag2ObjectMapper
    {
        private Dictionary<string, string> dicomItemMap;

        public void CollectDicomItem(string tag, string value)
        {
            if (dicomItemMap.ContainsKey(tag))
                return;

            dicomItemMap.Add(tag, value);
        }

        public IEnumerable<PropertyMap> GetMatchingTag
            (DicomDataset sourceType, Type targetType)
        {
            dicomItemMap = new Dictionary<string, string>();
            new DicomDatasetWalker(sourceType).Walk(new DatasetWalker(CollectDicomItem));

            var targetProperties = targetType.GetProperties();

            var properties = (from s in dicomItemMap
                              from t in targetProperties
                              where s.Key == t.Name &&
                                    t.CanWrite
                              select new PropertyMap
                              {
                                  SourceProperty = s,
                                  TargetProperty = t
                              }).ToList();

            return properties;
        }

        public T Map<T>(DicomDataset source, Dictionary<string, dynamic> mapper = null)
            where T : new()
        {
            if (mapper == null)
                mapper = new Dictionary<string, dynamic>();

            var obj = new T();
            var propMap = GetMatchingTag(source, obj.GetType());

            foreach (var prop in propMap)
            {
                var sourceValue = prop.SourceProperty.Value;
                prop.TargetProperty.SetValue(obj, sourceValue, null);
            }

            foreach (var prop in mapper)
            {
                var key = prop.Key;
                var value = prop.Value;
                obj.GetType().GetProperty(key)?.SetValue(obj, value, null);
            }

            return obj;
        }
    }
}