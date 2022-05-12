using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomDirExporter.Model.Repository
{
    /// <summary>
    /// QR 查詢結果 ADD 20220509 Oscar
    /// </summary>
    public class QueryRetrieveQueryResultModel
    {
        #region Fields
        /// <summary>
        /// 病歷號碼
        /// </summary>
        public string PatientID { get; set; }
        /// <summary>
        /// 病人姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 檢查單號
        /// </summary>
        public string AccessionNumber { get; set; }
        /// <summary>
        /// 檢查種類
        /// </summary>
        public string Modality { get; set; }
        /// <summary>
        /// 檢查日期
        /// </summary>
        public string StudyDate { get; set; }
        /// <summary>
        /// 檢查描述
        /// </summary>
        public string StudyDescription { get; set; }
        /// <summary>
        /// 檢查唯一碼
        /// </summary>
        public string StudyInstanceUID { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 建構
        /// </summary>
        public QueryRetrieveQueryResultModel()
        {
            Clear();
        }
        /// <summary>
        /// 清除資料
        /// </summary>
        protected void Clear()
        {
            PatientID = "";
            PatientName = "";
            AccessionNumber = "";
            Modality = "";
            StudyDate = "";
            StudyDescription = "";
            StudyInstanceUID = "";
        }
        #endregion
    }
}
