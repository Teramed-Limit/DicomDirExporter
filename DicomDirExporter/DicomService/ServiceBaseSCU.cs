using NLog;
using Dicom.Network.Client;
using DicomDirExporter.Interface;
using DicomDirExporter.Model.AppConfig;
using System.Threading.Tasks;

namespace DicomDirExporter.DicomService
{
    /// <summary>
    /// Service Base Scu ADD 20220505 Oscar
    /// </summary>
    public abstract class ServiceBaseSCU<IDicomSCUSetting, C, R> : IDicomServiceSCU<IDicomSCUSetting, C, R>
    {
        #region Fields
        /// <summary>
        /// Log
        /// </summary>
        protected Logger LogWriter = null;
        /// <summary>
        /// 設定
        /// </summary>
        protected IDicomSCUSetting ServiceSetting;
        /// <summary>
        /// 條件
        /// </summary>
        protected C Condition;
        /// <summary>
        /// 結果物件
        /// </summary>
        protected R Result;
        public R ResultData { get { return Result; } }
        /// <summary>
        /// 執行結果訊息
        /// </summary>
        public string ResultMessage { get; set; }
        /// <summary>
        /// SCU物件
        /// </summary>
        protected DicomClient DicomSCU = null;
        /// <summary>
        /// 服務型態
        /// </summary>
        public TypeOfSCU TypeOfSCU { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 指定SCU的服務型態
        /// </summary>
        protected abstract void AssignTypeOfSCU();
        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="setting"></param>
        public ServiceBaseSCU(IDicomSCUSetting setting, C condition,  R result)
        {
            LogWriter = LogManager.GetLogger("DicomDirExporter.DicomService.ServiceBaseSCU");

            ServiceSetting = setting;
            Condition = condition;
            Result = result;

            ResultMessage = "";
            TypeOfSCU = TypeOfSCU.Unknown;
            AssignTypeOfSCU();
        }
        /// <summary>
        /// 執行
        /// </summary>
        public virtual async Task Execute()
        {
            ResultMessage = "";

            if (!(await Task.Run(() => CheckSettings())))
            {
                LogWriter?.Error("Service base SCU check settings failure.");
                ResultMessage += ", check settings is failure, please check settings content.";
                LogWriter?.Error(ResultMessage);
            }
        }
        /// <summary>
        /// 檢查設定檔
        /// </summary>
        /// <returns>是否有效</returns>
        protected virtual bool CheckSettings()
        {
            //區域函式
            bool hasContent(string value)
            {
                return !(string.IsNullOrWhiteSpace(value));
            }
            bool isCurrentNumber(int value)
            {
                return (value > 0) ? true : false;
            }

            //預設類別是基本的四個項目
            ServiceBaseSetting setting = ServiceSetting as ServiceBaseSetting;
            if (setting == null)
            {
                ResultMessage = "ServiceSetting is not a valid object";
                return false;
            }
            bool valid = true;
            //valid &= await Task<bool>.Run(() => hasContent(setting.CalledAE));
            valid &= hasContent(setting.CalledAE);
            valid &= hasContent(setting.CallingAE);
            valid &= hasContent(setting.CalledIP);
            valid &= isCurrentNumber(setting.CalledPort);
            if (!valid)
                ResultMessage = "AE or IP or Port";
            return valid;
        }
        #endregion
    }
}
