using System;
using System.IO;
using System.Linq;
using System.Timers;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Table;
using DicomDirExporter.Repository.View;
using DicomDirExporter.Service;
using DicomDirExporter.Utility;
using RepoDb;
using RepoDb.Enumerations;

namespace DicomDirExporter.Runner
{
    public class SpaceReleaseRunner
    {
        private static Timer _timer;
        private readonly StudyWithImageViewRepository _imageView = new StudyWithImageViewRepository();
        private readonly DicomImageRepository _imageRepo = new DicomImageRepository();
        private readonly DicomSeriesRepository _seriesRepo = new DicomSeriesRepository();
        private readonly DicomStudyRepository _studyRepo = new DicomStudyRepository();
        private readonly Config _config = ConfigService.GetInstance().GetSetting();
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public SpaceReleaseRunner()
        {
            ReleaseSpace();
            ScheduleTimer();
        }

        public void ScheduleTimer()
        {
            var nowTime = DateTime.Now;
            var scheduledTime =
                new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 12, 0, 0,
                    0);
            if (nowTime > scheduledTime) scheduledTime = scheduledTime.AddDays(1);

            var tickTime = (scheduledTime - DateTime.Now).TotalMilliseconds;
            _timer = new Timer(tickTime);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            ReleaseSpace();
            ScheduleTimer();
        }

        private void ReleaseSpace()
        {
            try
            {
                var where = new[]
                {
                    new QueryField("LastReceivedTime", Operation.LessThan, DateTime.Now.AddDays(-(_config.OtherSetting.ExamKeepDays)))
                };

                var collection = _imageView.Get(where);
                var studyWithImageViews = collection as StudyWithImageView[] ?? collection.ToArray();
                var studyList = studyWithImageViews.GroupBy(x => x.StudyInstanceUID).Select(x => x.Key);
                var seriesList = studyWithImageViews.GroupBy(x => x.SeriesInstanceUID).Select(x => x.Key);

                foreach (var image in studyWithImageViews)
                {
                    if (File.Exists(image.FilePath)) File.Delete(image.FilePath);
                    _imageRepo.Remove(image.SOPInstanceUID);
                    logger.Trace($"Remove file: {image.FilePath}");
                }

                foreach (var series in seriesList)
                {
                    _seriesRepo.Remove(series);
                }

                foreach (var study in studyList)
                {
                    var studyPath = Path.Combine(ConfigService.StoragePath, study);
                    if (Directory.Exists(studyPath)) DirectoryHelper.RemoveEmptyDirectory(studyPath, true);
                    _studyRepo.Remove(study);
                }
            }
            catch (Exception e)
            {
                logger.Error($"Release space error: {e.Message}");
            }
        }
    }
}