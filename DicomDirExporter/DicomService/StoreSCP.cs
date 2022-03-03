using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DicomDirExporter.Model.AppConfig;
using DicomDirExporter.Runner;
using DicomDirExporter.Service;
using DicomDirExporter.Utility;
using DicomDirExporter.Utility.DicomHelper;
using Dicom;
using Dicom.Log;
using Dicom.Network;
using static System.Guid;

namespace DicomDirExporter.DicomService
{
    public class CStoreScp : Dicom.Network.DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes =
        {
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes =
        {
            // Lossless
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEGProcess14SV1,
            DicomTransferSyntax.JPEGProcess14,
            DicomTransferSyntax.RLELossless,
            // Lossy
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.JPEGProcess1,
            DicomTransferSyntax.JPEGProcess2_4,
            // Uncompressed
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian,
            // MP4
            DicomTransferSyntax.MPEG4AVCH264HighProfileLevel41,
            DicomTransferSyntax.MPEG4AVCH264BDCompatibleHighProfileLevel41,
            DicomTransferSyntax.MPEG4AVCH264HighProfileLevel42For2DVideo,
            DicomTransferSyntax.MPEG4AVCH264HighProfileLevel42For3DVideo,
            DicomTransferSyntax.MPEG4AVCH264StereoHighProfileLevel42,
            DicomTransferSyntax.HEVCH265MainProfileLevel51,
            DicomTransferSyntax.HEVCH265Main10ProfileLevel51,
        };

        private readonly List<string> _dicomFilePathList = new List<string>();
        private readonly List<PacsSetting> _pacsSettings = ConfigService.GetInstance().GetSetting().PacsSetting;
        private readonly string _serverAE = ConfigService.GetInstance().GetSetting().ServerSetting.ServerAe;
        private readonly string _guid;
        private DicomRequest _receiveDicomRequest;

        public CStoreScp(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
        {
            _guid = NewGuid().ToString();
        }

        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            _receiveDicomRequest = request;
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            string patId = null;
            try
            {
                _receiveDicomRequest = request;
                var sopInsUid = request.SOPInstanceUID.UID;

                var path = ConfigService.TempStoragePath;
                // path = Path.Combine(path, Association.CallingAE);

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var filePath = Path.Combine(path, sopInsUid) + ".dcm";
                request.File.Save(filePath);
            }
            catch (Exception e)
            {
                var dir = Path.Combine(ConfigService.TempStoragePath, Association.CallingAE, patId, _guid);
                var errorPath = Path.Combine(ConfigService.ErrorPath, patId);
                logger.Error($"C-Store process error, reason: {e.Message}");
                logger.Error($"Move dcm to {errorPath}");
                DirectoryHelper.MoveDir(dir, errorPath);
            }

            return new DicomCStoreResponse(request, DicomStatus.Success);
        }

        public void OnCStoreRequestException(string tempFileName, Exception e)
        {
            logger.Error($"C-Store request exception reason: {e.Message}");
            // let library handle logging and error response
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            logger.Trace("OnReceiveAssociationRequestAsync Start");
            var matched = false;
            var rejectReason = DicomRejectReason.NoReasonGiven;
            foreach (var pacs in _pacsSettings)
            {
                if (association.CalledAE == _serverAE /* && association.CallingAE == pacs.CallingAe */)
                {
                    matched = true;
                    break;
                }

                if (association.CalledAE != _serverAE)
                    rejectReason = DicomRejectReason.CalledAENotRecognized;

                // if(association.CallingAE != pacs.CallingAe)
                //     rejectReason = DicomRejectReason.CallingAENotRecognized;
            }

            if (!matched)
            {
                logger.Trace("C-Store assoc-reject, called ae error");
                logger.Trace($"client calledAE: {association.CalledAE}, callingAE: {association.CallingAE}");
                return SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    rejectReason);
            }

            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification) pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                    pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
            }

            return SendAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
        }

        public void OnConnectionClosed(Exception exception)
        {
        }
    }
}