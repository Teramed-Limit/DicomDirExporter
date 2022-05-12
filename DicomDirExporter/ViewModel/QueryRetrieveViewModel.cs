using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DicomDirExporter.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Service;
using DicomDirExporter.Interface;
using DicomDirExporter.DicomService;
using System;
using DicomDirExporter.Logic;

namespace DicomDirExporter.ViewModel
{
    public class QueryRetrieveViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// messge show
        /// </summary>
        private readonly SnackbarMessenger _snackbarMessenger = null;
        #endregion

        #region Methods
        /// <summary>
        /// 建構
        /// </summary>
        public QueryRetrieveViewModel(SnackbarMessenger snackbarMessager)
        {
            _snackbarMessenger = snackbarMessager;

            //Query Condition (for C-Find requests)
            List<ColumnDefine> _qyeryColumnDefines = ConfigService.GetInstance().GetQRQueryDefine().HistoryGridQueryCondition;

            QueryFields = new ObservableCollection<UIQueryField>(
                from column in _qyeryColumnDefines
                where (!string.IsNullOrWhiteSpace(column.Key))
                select new UIQueryField
                (
                    column.Key,
                    column.Operation,
                    null,
                    column.Visible,
                    column.Type,
                    column.Label
                ));
        }
        /// <summary>
        /// 查詢
        /// </summary>
        protected async void Search()
        {
            //至少要有一個條件
            IEnumerable<UIQueryField> tagKeys = QueryFields.Where(x => !string.IsNullOrWhiteSpace(x.Value));
            if (!tagKeys.Any())
            {
                _snackbarMessenger?.ShowErrorMessage("Please type-in condition content for search.");
                return;
            }

            QueryResult = null;
            using (QueryRetrieveService qrService = new QueryRetrieveService(TypeOfSCU.CFindStudy))
            {
                await qrService.Query(QueryFields);
                QueryResult = new ObservableCollection<QueryRetrieveQueryResultModel>(qrService.Result);
            }
        }
        /// <summary>
        /// 取回影像
        /// </summary>
        protected async void Retrieve(int studyIdx)
        {
            //依據StudyInstanceUID去取回
            if ((QueryResult == null)||(!QueryResult.Any())||(studyIdx<=-1))
            {
                _snackbarMessenger?.ShowErrorMessage("No any result for retrieve or index is invalid");
                return;
            }
                
            QueryRetrieveQueryResultModel qrResultModel = QueryResult[studyIdx];

            CMoveSetting setting = new CMoveSetting();
            Config config = ConfigService.GetInstance().GetNewInstanceSetting();
            setting.CalledAE = config.RetrieveSetting.CalledAE;
            setting.CallingAE = config.RetrieveSetting.CallingAE;
            setting.CalledIP = config.RetrieveSetting.CalledIP;
            setting.CalledPort = config.RetrieveSetting.CalledPort;
            setting.DestinationAE = config.RetrieveSetting.DestinationAE;
            CMoveSCU<IDicomSCUSetting, string, Dicom.DicomDataset> scu
                = new CMoveSCU<IDicomSCUSetting, string, Dicom.DicomDataset>(setting, qrResultModel.StudyInstanceUID,
                new Dicom.DicomDataset());
            await scu.Execute();

            Dicom.DicomDataset dataset = scu.ResultData as Dicom.DicomDataset;

            string uid = dataset.GetString(Dicom.DicomTag.StudyInstanceUID);

            if (!string.IsNullOrWhiteSpace(uid))
                _snackbarMessenger?.ShowSuccessMessage("Retrieve study image(s) success.");
            else
                _snackbarMessenger?.ShowErrorMessage("Retrieve study images(s) failed.");
        }
        #endregion

        #region Bindings
        /// <summary>
        /// 查詢條件
        /// </summary>
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
        /// <summary>
        /// 查詢結果
        /// </summary>
        private ObservableCollection<QueryRetrieveQueryResultModel> _queryResult { get; set; }
        public ObservableCollection<QueryRetrieveQueryResultModel> QueryResult
        {
            get => _queryResult;
            set
            {
                _queryResult = value;
                RaisePropertyChanged(() => QueryResult);
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// 查詢
        /// </summary>
        private ICommand _QueryCommand;
        public ICommand QueryCommand
        {
            get { return _QueryCommand ?? (_QueryCommand = new RelayCommand(() => { Search(); })); }
            set { _QueryCommand = value; }
        }
        /// <summary>
        /// 取得影像
        /// </summary>
        protected ICommand _RetrieveCommand;
        public ICommand RetrieveCommand
        {
            get { return _RetrieveCommand ?? (_RetrieveCommand = new RelayCommand<int>((index) => { Retrieve(index); })); }
            set { _RetrieveCommand = value; }
        }
        #endregion
    }
}
