using System;
using System.Threading.Tasks;
using Dicom;
using Dicom.IO.Buffer;

namespace DicomDirExporter.Utility.DicomHelper
{
    public delegate void CollectItem(string tag, string value);

    public class DatasetWalker : IDicomDatasetWalker
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly CollectItem _collectItem;
        private int _level;

        public DatasetWalker()
        {
            Level = 0;
        }

        public DatasetWalker(CollectItem collectItem)
        {
            _collectItem = collectItem;
            Level = 0;
        }


        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                Indent = string.Empty;
                for (var i = 0; i < _level; i++)
                    Indent += "    ";
            }
        }

        private string Indent { get; set; }


        public bool OnElement(DicomElement element)
        {
            var tag = $"{Indent}{element.Tag.ToString().ToUpper()}  {element.Tag.DictionaryEntry.Name}";
            var value = "<large value not displayed>";
            if (element.Length <= 2048)
                value = string.Join("\\", element.Get<string[]>());

            if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
            {
                var uid = element.Get<DicomUID>(0);
                var name = uid.Name;
                if (name != "Unknown")
                    value = $"{value} ({name})";
            }

            logger.Debug($"{tag}: {value}");

            if (_collectItem != null)
                _collectItem.Invoke(element.Tag.DictionaryEntry.Keyword, value);

            return true;
        }

        public bool OnBeginSequence(DicomSequence sequence)
        {
            var tag = $"{Indent}{sequence.Tag.ToString().ToUpper()}  {sequence.Tag.DictionaryEntry.Name}";
            logger.Debug($"{tag}: {sequence}");
            Level++;
            return true;
        }

        public bool OnBeginSequenceItem(DicomDataset dataset)
        {
            var tag = $"{Indent}Sequence Item:";
            logger.Debug($"{tag}");
            Level++;
            return true;
        }

        public bool OnEndSequenceItem()
        {
            Level--;
            return true;
        }

        public bool OnEndSequence()
        {
            Level--;
            return true;
        }

        public bool OnBeginFragment(DicomFragmentSequence fragment)
        {
            var tag = $"{Indent}{fragment.Tag.ToString().ToUpper()}  {fragment.Tag.DictionaryEntry.Name}";
            logger.Debug(tag);
            Level++;
            return true;
        }

        public bool OnFragmentItem(IByteBuffer item)
        {
            var tag = $"{Indent}Fragment";
            logger.Debug(tag);
            return true;
        }

        public bool OnEndFragment()
        {
            Level--;
            return true;
        }

        public void OnEndWalk()
        {
            logger.Debug($"===================End of dataset===================");
        }

        public void OnBeginWalk()
        {
            logger.Debug($"===================Start of dataset===================");
        }

        public Task<bool> OnElementAsync(DicomElement element)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OnFragmentItemAsync(IByteBuffer item)
        {
            throw new NotImplementedException();
        }
    }
}