using Dicom;
using Dicom.Network;
using DicomDirExporter.DicomService;
using DicomDirExporter.Interface;
using DicomDirExporter.Model;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Model.Repository;
using DicomQCTool.Model.DicomOperator;
using NLog;
using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomDirExporter.Service
{
    /// <summary>
    /// Query Retrieve Service ADD 20220506 Oscar
    /// 主要針對查詢與結果類型的SCU服務
    /// </summary>
    public class QueryRetrieveService : IDisposable
    {
        #region Fields
        /// <summary>
        /// Log
        /// </summary>
        protected readonly Logger LogWriter = LogManager.GetLogger("DicomDirExporter.DicomService.ServiceBaseSCU");
        /// <summary>
        /// 服務的類型
        /// </summary>
        protected TypeOfSCU ServiceType;
        /// <summary>
        /// 查詢結果
        /// </summary>
        protected List<QueryRetrieveQueryResultModel> _Result { get; set; }
        public List<QueryRetrieveQueryResultModel> Result { get { return _Result; } }
        #endregion

        /// <summary>
        /// 建構
        /// </summary>
        public QueryRetrieveService(TypeOfSCU type)
        {
            ServiceType = type;
            Clear();
        }

        #region Methods
        /// <summary>
        /// 清除
        /// </summary>
        protected void Clear()
        {
            _Result = null;
        }
        /// <summary>
        /// 釋放
        /// </summary>
        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 查詢數據
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task Query(IEnumerable<UIQueryField> condition)
        {
            _Result = null;

            try
            {
                LogWriter?.Info("QR Service starty query with SCU type : " + TypeOfSCU.CFindStudy.ToString());
                //目前僅做QR的C-Find, Worklist的C-Find裡面沒有做,其餘也是
                if (ServiceType==TypeOfSCU.CFindStudy)
                {
                    DicomCFindRequest requests = ConvertConditionForCFind(condition);
                    if(requests!=null)
                    {
                        IDicomSCUSetting setting = new ServiceBaseSetting();
                        Config config = ConfigService.GetInstance().GetNewInstanceSetting();
                        setting.CalledAE = config.RetrieveSetting.CalledAE;
                        setting.CallingAE = config.RetrieveSetting.CallingAE;
                        setting.CalledIP = config.RetrieveSetting.CalledIP;
                        setting.CalledPort = config.RetrieveSetting.CalledPort;
                        CFindSCU<IDicomSCUSetting, DicomCFindRequest, List<DicomDataset>> scu
                            = new CFindSCU<IDicomSCUSetting, DicomCFindRequest, List<DicomDataset>>(setting, requests, new List<DicomDataset>());
                        await scu.Execute();
                        _Result = ConvertResult(scu.ResultData as List<DicomDataset>);
                    }
                }
                else if(ServiceType == TypeOfSCU.CFindStudy)
                {
                    DicomCFindRequest requests = ConvertConditionForCFindWorklist(condition);
                    if (requests != null)
                    {
                        // In the future, do C-Find for worklist here. and parse the result for display on UI.
                    }
                }
            }
            catch { _Result = null; }
            finally
            {
                string finallyMsg = "QR Service finally : result ";
                finallyMsg += (_Result == null) ? "is null." : _Result.Count().ToString() + " item(s).";
                LogWriter?.Info(finallyMsg);
            }
        }
        /// <summary>
        /// 轉換字串內容為日期格式字串的內容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ParseValueForDate(string value)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    return DateTime
                            .ParseExact(value, sysFormat, CultureInfo.InvariantCulture,
                                DateTimeStyles.AllowWhiteSpaces).ToString("yyyyMMdd");
                }
                return value;
            }
            catch { return value; }
        }

        /// <summary>
        /// 取得指定作業類型的查詢條件
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="assignType"></param>
        /// <returns></returns>
        protected IEnumerable<UIQueryField> GetAssignTypeFields(IEnumerable<UIQueryField> condition, Operation assignType)
        {
            return condition
                .Where(x =>
                    x.Operation == assignType)
                .Select(field => field).ToList();
        }
        /// <summary>
        /// 轉換條件資料
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected DicomCFindRequest ConvertConditionForCFind(IEnumerable<UIQueryField> condition)
        {
            if ((condition == null)||(!condition.Any()))
                return null;

            //至少要有一個條件
            //IEnumerable<UIQueryField> tagKeys = condition.Where(x => !string.IsNullOrWhiteSpace(x.Value));
            IEnumerable<UIQueryField> tagKeys = condition.Where(x => !string.IsNullOrWhiteSpace(x.Field.Name));
            //if (!tagKeys.Any())
            //    return null;

            //目前只支援 Equal(等於), Between(區間)

            //等於
            IEnumerable<UIQueryField> equalFields = GetAssignTypeFields(tagKeys, Operation.Equal);
            //區間
            IEnumerable<UIQueryField> betweenFields = GetAssignTypeFields(tagKeys, Operation.Between);
            //C-Find dataset
            DicomCFindRequest findRequests = DicomCFindRequest.CreateStudyQuery();

            DicomOperatorHelper helper = new DicomOperatorHelper();

            //等於
            foreach (UIQueryField field in equalFields)
            {
                helper.ConvertTagStringToUIntGE(field.Field.Name, out ushort group, out ushort elem);
                DicomTag findTag = new DicomTag(group, elem);
                string value = field.Value.Trim();
                if (field.UIType == "DatePicker")
                    value = ParseValueForDate(value);
                helper.WriteDicomValueInDataset(findRequests.Dataset, findTag, value, false);
            }
            //區間(複數組合)-目前僅支援一組(單一tag設定區間條件值)
            string betweenValue = "";
            DicomTag betweenDicomTag = null;
            foreach (UIQueryField field in betweenFields)
            {
                if(betweenDicomTag==null)
                {
                    helper.ConvertTagStringToUIntGE(field.Field.Name, out ushort group, out ushort elem);
                    betweenDicomTag = new DicomTag(group, elem);
                }
                string value = field.Value.Trim();
                if (field.UIType == "DatePicker")
                    value = ParseValueForDate(value);

                if(!string.IsNullOrWhiteSpace(value))
                {
                    if (string.IsNullOrWhiteSpace(betweenValue))
                        betweenValue = value;
                    else
                        betweenValue += "-" + value;
                }
            }
            if((!string.IsNullOrWhiteSpace(betweenValue))&&(betweenDicomTag!=null))
                helper.WriteDicomValueInDataset(findRequests.Dataset, betweenDicomTag, betweenValue, false);

            return findRequests;
        }
        /// <summary>
        /// 轉換條件資料
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected DicomCFindRequest ConvertConditionForCFindWorklist(IEnumerable<UIQueryField> condition)
        {
            if ((condition == null) || (!condition.Any()))
                return null;

            //至少要有一個條件
            IEnumerable<UIQueryField> tagKeys = condition.Where(x => !string.IsNullOrWhiteSpace(x.Value));
            //if (!tagKeys.Any())
            //    return null;

            //目前只支援 Equal(等於), Between(區間)

            //等於
            IEnumerable<UIQueryField> equalFields = GetAssignTypeFields(tagKeys, Operation.Equal);
            //區間
            IEnumerable<UIQueryField> betweenFields = GetAssignTypeFields(tagKeys, Operation.Between);
            //C-Find dataset
            DicomCFindRequest findRequests = DicomCFindRequest.CreateWorklistQuery();

            //Parse query condition for dataset (match key, return key) here.
            /*
             * 
             * 
             * 
             */
            
            return findRequests;
        }
        /// <summary>
        /// 是否為 UTF8 編碼
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public bool IsUtf8Code(DicomDataset dataset)
        {
            if (dataset == null)
                return false;
            if (dataset.TryGetString(DicomTag.SpecificCharacterSet, out string elementVal) == true)
            {
                if (elementVal.Trim() == "ISO_IR 192" || elementVal.Trim() == "ISO_IR_192")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 轉換結果資料
        /// </summary>
        /// <param name="resultSource"></param>
        /// <returns></returns>
        public List<QueryRetrieveQueryResultModel> ConvertResult(List<DicomDataset> resultSource)
        {
            List<QueryRetrieveQueryResultModel> result = new List<QueryRetrieveQueryResultModel>();
            try
            {
                if (resultSource == null)
                {
                    LogWriter?.Info("QR Service : After query, the result is null.");
                    return result;
                }
                LogWriter?.Info("QR Service : After query, the result has "+resultSource.Count().ToString()+ " item(s).");

                DicomOperatorHelper helper = new DicomOperatorHelper();
                foreach (DicomDataset dataset in resultSource)
                {
                    bool utf8 = IsUtf8Code(dataset);
                    QueryRetrieveQueryResultModel qrResultModel = new QueryRetrieveQueryResultModel();
                    qrResultModel.PatientID = helper.GetDicomValueToString(dataset, DicomTag.PatientID, DicomVR.LO, utf8);
                    qrResultModel.PatientName = helper.GetDicomValueToString(dataset, DicomTag.PatientName, DicomVR.PN, utf8);
                    qrResultModel.AccessionNumber = helper.GetDicomValueToString(dataset, DicomTag.AccessionNumber, DicomVR.SH, utf8);
                    qrResultModel.Modality = helper.GetDicomValueToString(dataset, DicomTag.ModalitiesInStudy, DicomVR.CS, utf8);
                    qrResultModel.StudyDate = helper.GetDicomValueToString(dataset, DicomTag.StudyDate, DicomVR.DA, utf8);
                    qrResultModel.StudyDescription = helper.GetDicomValueToString(dataset, DicomTag.StudyDescription, DicomVR.LO, utf8);
                    qrResultModel.StudyInstanceUID = helper.GetDicomValueToString(dataset, DicomTag.StudyInstanceUID, DicomVR.UI, utf8).Replace('\0', ' ').Trim();
                    result.Add(qrResultModel);
                }
                LogWriter?.Info("QR Service : After ConvertResult, the result has " + result.Count().ToString() + " item(s).");
                return result;
            }
            catch(Exception ex)
            {
                LogWriter?.Info("QR Service : ConvertResult with error : " + ex.Message);
                return new List<QueryRetrieveQueryResultModel>();
            }
        }
        #endregion
    }
}
