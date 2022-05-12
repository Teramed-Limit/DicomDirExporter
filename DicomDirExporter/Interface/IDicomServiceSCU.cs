using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomDirExporter.Interface
{
    /// <summary>
    /// 服務型態
    /// </summary>
    public enum TypeOfSCU { Unknown=0, CEcho=1, CFindStudy=2, CMove=3, CFindWorklist=4 }
    /// <summary>
    /// Dicom Service SCU ADD 20220505 Oscar
    /// </summary>
    public interface IDicomServiceSCU<IDicomSCUSetting, C, R>
    {
        /// <summary>
        /// 執行
        /// </summary>
        Task Execute();
        /// <summary>
        /// 結果訊息
        /// </summary>
        string ResultMessage { get; set; }
        /// <summary>
        /// 服務型態
        /// </summary>
        TypeOfSCU TypeOfSCU { get; set; }
    }
}
