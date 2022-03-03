using Dicom;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Service;
using DicomDirExporter.Utility;
using DicomDirExporter.Utility.AutoMapper;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace DicomDirExporter.Runner
{
    public class ArchivingRunner : Runner
    {
        private static ArchivingRunner _instance;
        private readonly DicomArchivingService _dicomArchiving;
        private readonly Tag2ObjectMapper _mapper;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentQueue<List<string>> _queue;

        public ArchivingRunner()
        {
            _dicomArchiving = new DicomArchivingService();
            _mapper = new Tag2ObjectMapper();
            _queue = new ConcurrentQueue<List<string>>();

            if (!Directory.Exists(ConfigService.TempStoragePath)) Directory.CreateDirectory(ConfigService.TempStoragePath);
            if (!Directory.Exists(ConfigService.StoragePath)) Directory.CreateDirectory(ConfigService.StoragePath);
            if (!Directory.Exists(ConfigService.ErrorPath)) Directory.CreateDirectory(ConfigService.ErrorPath);

        }

        public static ArchivingRunner GetInstance()
        {
            return _instance ?? (_instance = new ArchivingRunner());
        }

        protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Stop();
            var collection = new List<string>();

            DirectoryHelper.GetFilesInFolder(ConfigService.TempStoragePath, collection, ".dcm");
            _queue.Enqueue(collection);

            if (_queue.Count == 0)
            {
                Start();
                return;
            }

            try
            {
                DicomFile dcm = null;
                _queue.TryDequeue(out var imageList);
                {
                    foreach (var dcmFile in imageList)
                    {
                        try
                        {
                            dcm = DicomFile.Open(dcmFile);
                            var storeFilePath = GenerateFileSavePath(dcm.Dataset, ConfigService.StoragePath);
                            var patObj = _mapper.Map<DicomPatient>(dcm.Dataset);
                            var studyObj = _mapper.Map<DicomStudy>(dcm.Dataset);
                            var seriesObj = _mapper.Map<DicomSeries>(dcm.Dataset);
                            var imageObj = _mapper.Map<DicomImage>(dcm.Dataset,
                                new Dictionary<string, dynamic>
                                {
                                    { "FilePath", storeFilePath }
                                });

                            logger.Trace("Archive start");
                            logger.Trace($"PatientID: {patObj.PatientID}");
                            logger.Trace($"StudyInstanceUID: {studyObj.StudyInstanceUID}");
                            logger.Trace($"SeriesInstanceUID: {seriesObj.SeriesInstanceUID}");
                            logger.Trace($"SOPInstanceUID: {imageObj.SOPInstanceUID}");

                            _dicomArchiving.ArchiveDicom(patObj);
                            _dicomArchiving.ArchiveDicom(studyObj);
                            _dicomArchiving.ArchiveDicom(seriesObj);
                            _dicomArchiving.ArchiveDicom(imageObj);
                            dcm.Save(storeFilePath);
                            File.Delete(dcmFile);
                        }
                        catch (Exception exception)
                        {
                            logger.Error($"Archive error: {exception.Message}");
                            var errorFilePath = GenerateFileSavePath(dcm.Dataset, ConfigService.ErrorPath);
                            dcm.Save(errorFilePath);
                            File.Delete(dcmFile);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error($"Exception: {exception}");
            }
            Start();
        }

        private string GenerateFileSavePath(DicomDataset dataset, string directory)
        {
            try
            {
                var patId = dataset.GetSingleValue<string>(DicomTag.PatientID);
                var studyDate = dataset.GetSingleValue<string>(DicomTag.StudyDate);
                var fileName = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                var targetPath = directory;
                targetPath = Path.Combine(targetPath, patId, studyDate);
                DirectoryHelper.CreateDirectory(targetPath);

                targetPath = Path.Combine(targetPath, fileName) + ".dcm";
                return targetPath;
            }
            catch (Exception e)
            {
                logger.Trace($"GenerateFileSavePath error: {e.Message}");
                throw new Exception(e.Message);
            }
        }
    }
}