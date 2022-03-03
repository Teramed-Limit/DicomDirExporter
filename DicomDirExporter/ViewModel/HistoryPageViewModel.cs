using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DicomDirExporter.Logic;
using DicomDirExporter.Model;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DicomDirExporter.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class HistoryPageViewModel : ViewModelBase
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetLogger("UserAction");
        private readonly SnackbarMessenger _snackbarMessenger;
        private readonly HistoryQueryService _historyQueryService;
        private readonly JobBuilderService _jobBuilder = new JobBuilderService();
        private readonly List<ColumnDefine> _qyeryColumnDefines =
            ConfigService.GetInstance().GetGridQueryDefine().HistoryGridQueryCondition;

        public HistoryPageViewModel(HistoryQueryService historyQueryService, SnackbarMessenger snackbarMessager)
        {
            _historyQueryService = historyQueryService;
            _snackbarMessenger = snackbarMessager;
            // Register query field
            QueryFields = new ObservableCollection<UIQueryField>(
                from column in _qyeryColumnDefines
                from dbField in _historyQueryService.GetQueryFields()
                where column.Key == dbField.Name
                select new UIQueryField
                (
                    column.Key,
                    column.Operation,
                    null,
                    column.Visible,
                    column.Type,
                    column.Label
                ));
            // Command
            RapidQueryCommand = new RelayCommand<string>(RapidQuery);
            QueryCommand = new RelayCommand(Query);
            ExportCommand = new RelayCommand<int>(OnExport);
        }

        public ICommand RapidQueryCommand { get; }
        public ICommand QueryCommand { get; }
        public ICommand ExportCommand { get; }

        private ObservableCollection<DcmFindStudyLevelView> _queryResult { get; set; }

        public ObservableCollection<DcmFindStudyLevelView> QueryResult
        {
            get => _queryResult;
            set
            {
                _queryResult = value;
                RaisePropertyChanged(() => QueryResult);
            }
        }

        private ObservableCollection<UIQueryField> _queryFields { get; set; }

        public ObservableCollection<UIQueryField> QueryFields
        {
            get => _queryFields;
            set
            {
                _queryFields = value;
                RaisePropertyChanged(() => QueryFields);
            }
        }

        public async void RapidQuery(string day)
        {
            var query = await _historyQueryService.RapidQuery(Convert.ToInt32(day));
            QueryResult = new ObservableCollection<DcmFindStudyLevelView>(query);
        }

        public async void Query()
        {
            try
            {
                var where = _historyQueryService.ConvertQueryFields(QueryFields).ToArray();
                foreach (var queryField in where)
                {
                    logger.Trace($"Field name: {queryField.Field.Name}");
                    if (queryField.GetValue() is List<string>)
                        foreach (var value in (List<string>)queryField.GetValue())
                            logger.Trace($"Field value: {value}");
                    else
                        logger.Trace($"Field value: {queryField.GetValue()}");
                }

                var query = await _historyQueryService.ConditionQuery(where);
                QueryResult = new ObservableCollection<DcmFindStudyLevelView>(query);
            }
            catch (Exception e)
            {
                _snackbarMessenger.ShowErrorMessage(e.Message);
            }
        }

        public async void OnExport(int studyIdx)
        {
            try
            {
                var study = QueryResult[studyIdx];
                var exportDirName = $"{study.PatientID}({study.PatientID})-{study.StudyDate}";
                _snackbarMessenger.ShowInfoMessage($"Export study: {study.AccessionNumber}");
                await _jobBuilder.BuildJob(study.StudyInstanceUID, exportDirName);
            }
            catch (Exception e)
            {
                _snackbarMessenger.ShowErrorMessage(e.Message);
            }
        }
    }
}