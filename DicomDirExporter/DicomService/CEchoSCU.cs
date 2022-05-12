using Dicom.Network.Client;
using DicomDirExporter.Model.AppConfig;
using System;
using System.Threading.Tasks;
using DicomDirExporter.Interface;
using NLog;

namespace DicomDirExporter.DicomService
{
    /// <summary>
    /// C-Echo SCU ADD 20220505 Oscar
    /// </summary>
    public class CEchoSCU<IDicomSCUSetting, C, R> : ServiceBaseSCU<IDicomSCUSetting, C, R>
    {
        #region Methods
        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="setting"></param>
        public CEchoSCU(IDicomSCUSetting setting, C condition, R result) : base(setting, condition, result)
        {
            LogWriter = LogManager.GetLogger("DicomDirExporter.DicomService.CEchoSCU");
        }
        /// <summary>
        /// 指定SCU的服務型態
        /// </summary>
        protected override void AssignTypeOfSCU()
        {
            TypeOfSCU = TypeOfSCU.CEcho;
        }
        /// <summary>
        /// 執行
        /// </summary>
        public override async Task Execute()
        {
            try
            {
                await base.Execute();
                if (!string.IsNullOrWhiteSpace(ResultMessage))
                    return;

                ServiceBaseSetting setting = ServiceSetting as ServiceBaseSetting;
                
                DicomSCU = new DicomClient(setting.CalledIP, setting.CalledPort, false, setting.CallingAE, setting.CalledAE);
                await DicomSCU.AddRequestAsync(new Dicom.Network.DicomCEchoRequest
                {
                    OnResponseReceived = (requests, response) =>
                    {
                        Dicom.Network.DicomStatus dcmStatus = response.Status;
                        if(dcmStatus.State == Dicom.Network.DicomState.Success)
                            ResultMessage = "C-Echo success.";
                        else
                            ResultMessage = "C-Echo : " + dcmStatus.State.ToString() + " (" + dcmStatus.Code.ToString() + ") " + dcmStatus.Description;
                    }
                });
                await DicomSCU.SendAsync();
            }
            catch(Exception ex)
            {
                ResultMessage = "C-Echo with error : " + ex.Message;
            }
            finally
            {
                LogWriter?.Info(ResultMessage);
            }
        }
        #endregion
    }
}
