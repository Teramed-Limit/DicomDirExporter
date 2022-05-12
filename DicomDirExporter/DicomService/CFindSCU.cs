using Dicom.Network.Client;
using DicomDirExporter.Model.AppConfig;
using System;
using System.Threading.Tasks;
using DicomDirExporter.Interface;
using System.Collections.Generic;
using Dicom;

namespace DicomDirExporter.DicomService
{
    /// <summary>
    /// C-Find SCU ADD 20220505 Oscar
    /// </summary>
    public class CFindSCU<IDicomSCUSetting, C, R> : ServiceBaseSCU<IDicomSCUSetting, C, R>
    {
        #region Methods
        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="setting"></param>
        public CFindSCU(IDicomSCUSetting setting, C condition, R result) : base(setting, condition, result)
        {
            
        }
        /// <summary>
        /// 指定SCU的服務型態
        /// </summary>
        protected override void AssignTypeOfSCU()
        {
            TypeOfSCU = TypeOfSCU.CFindStudy;
        }
        /// <summary>
        /// 執行
        /// </summary>
        public override async Task Execute()
        {
            List<DicomDataset> dcmResult = null;
            try
            {
                await base.Execute();
                if (!string.IsNullOrWhiteSpace(ResultMessage))
                    return;

                ServiceBaseSetting setting = ServiceSetting as ServiceBaseSetting;

                Dicom.Network.DicomCFindRequest findRequests = Condition as Dicom.Network.DicomCFindRequest;

                if (findRequests == null)
                    return;

                dcmResult = Result as List<DicomDataset>;
                if(dcmResult == null)
                {
                    ResultMessage = "C-Find : convert result object before query retreive failed.";
                    LogWriter?.Info(ResultMessage);
                    return;
                }

                findRequests.OnResponseReceived = (requests, response) =>
                {
                    Dicom.Network.DicomStatus dcmStatus = response.Status;
                    if (dcmStatus.State == Dicom.Network.DicomState.Pending)
                    {
                        ResultMessage = "C-Find pending.";
                        dcmResult.Add(response.Dataset.Clone());
                    }
                    else if (dcmStatus.State == Dicom.Network.DicomState.Success)
                        ResultMessage = "C-Find success.";
                    else
                        ResultMessage = "C-Find : " + dcmStatus.State.ToString() + " (" + dcmStatus.Code.ToString() + ") " + dcmStatus.Description;
                };

                DicomSCU = new DicomClient(setting.CalledIP, setting.CalledPort, false, setting.CallingAE, setting.CalledAE);
                await DicomSCU.AddRequestAsync(findRequests);
                await DicomSCU.SendAsync();
            }
            catch (Exception ex)
            {
                dcmResult = null;
                ResultMessage = "C-Find with error : " + ex.Message;
            }
            finally
            {
                LogWriter?.Info(ResultMessage);
            }
        }
        #endregion
    }
}
