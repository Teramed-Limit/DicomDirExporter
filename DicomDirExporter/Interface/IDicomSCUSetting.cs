using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomDirExporter.Interface
{
    /// <summary>
    /// Dicom服務基本設定 ADD 20220505 Oscar
    /// </summary>
    interface IDicomSCUSetting
    {
        /// <summary>
        /// 被呼叫端AE
        /// </summary>
        string CalledAE { get; set; }
        /// <summary>
        /// 被呼叫端IP位址
        /// </summary>
        string CalledIP { get; set; }
        /// <summary>
        /// 被呼叫端提供的連接埠號
        /// </summary>
        int CalledPort { get; set; }
        /// <summary>
        /// 呼叫端的AE
        /// </summary>
        string CallingAE { get; set; }
    }
}
