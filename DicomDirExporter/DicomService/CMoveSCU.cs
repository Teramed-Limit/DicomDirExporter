using DicomDirExporter.Model.AppConfig;
using System;
using System.Threading.Tasks;
using DicomDirExporter.Interface;
using Dicom.Network.Client;

namespace DicomDirExporter.DicomService
{
    /// <summary>
    /// C-Move SCU ADD 20220510 Oscar
    /// </summary>
    public class CMoveSCU<IDicomSCUSetting, C, R> : ServiceBaseSCU<IDicomSCUSetting, C, R>
    {
        #region Methods
        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="setting"></param>
        public CMoveSCU(IDicomSCUSetting setting, C condition, R result) : base(setting, condition, result)
        {
            
        }
        /// <summary>
        /// 指定SCU的服務型態
        /// </summary>
        protected override void AssignTypeOfSCU()
        {
            TypeOfSCU = TypeOfSCU.CMove;
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

                string assignStudyInsUID = Condition as string;

                if(string.IsNullOrWhiteSpace(assignStudyInsUID))
                {
                    ResultMessage = "C-Move : do nothing, UID is empty.";
                    return;
                }

                Dicom.DicomDataset dataset = Result as Dicom.DicomDataset;

                if (dataset==null)
                {
                    ResultMessage = "C-Move : convert result object before query retreive failed.";
                    return;
                }
                dataset.Remove(Dicom.DicomTag.StudyInstanceUID);
                dataset.AddOrUpdate(Dicom.DicomTag.StudyInstanceUID, "");

                CMoveSetting setting = ServiceSetting as CMoveSetting;

                if (string.IsNullOrWhiteSpace(setting.DestinationAE))
                {
                    ResultMessage = "C-Move : do nothing, destination AE is empty.";
                    return;
                }

                LogWriter?.Info("C-Move : start UID " + assignStudyInsUID + " to destination AE : " + setting.DestinationAE);

                DicomSCU = new DicomClient(setting.CalledIP, setting.CalledPort, false, setting.CallingAE, setting.CalledAE);
                await DicomSCU.AddRequestAsync(new Dicom.Network.DicomCMoveRequest(setting.DestinationAE, assignStudyInsUID)
                {
                    OnResponseReceived = (requests, response) =>
                    {
                        Dicom.Network.DicomStatus dcmStatus = response.Status;
                        if (dcmStatus.State == Dicom.Network.DicomState.Success)
                            dataset.AddOrUpdate(Dicom.DicomTag.StudyInstanceUID, assignStudyInsUID);
                        ResultMessage = "C-Move : " + dcmStatus.State.ToString() + " (" + dcmStatus.Code.ToString() + ") " + dcmStatus.Description;
                    }
                });
                await DicomSCU.SendAsync();
            }
            catch (Exception ex)
            {
                ResultMessage = "C-Move with error : " + ex.Message;
            }
            finally
            {
                LogWriter?.Info(ResultMessage);
            }
        }
        #endregion
    }
}
