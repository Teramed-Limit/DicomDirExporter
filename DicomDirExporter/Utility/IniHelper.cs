using System.Runtime.InteropServices;
using System.Text;

namespace DicomDirExporter.Utility
{
    public class IniFile
    {
        public static string GetKeyValue(string strSection, string strKeyName, string strFileName)
        {
            var result = 0;
            const int bufferSize = 2048;

            var lpReturn = new StringBuilder(bufferSize);

            result = GetPrivateProfileString(strSection, strKeyName, "", lpReturn, bufferSize, strFileName);
            return result == 0 ? "" : lpReturn.ToString();
        }


        /// <summary>
        ///     Writes a key/value pair to a section of an INI file
        /// </summary>
        /// <param name="strSection">String containing section name</param>
        /// <param name="strKeyName">String containing key name</param>
        /// <param name="strKeyValue">String containing key value</param>
        /// <param name="strFileName">Full path and file name of INI file</param>
        public static bool WriteKeyValue(string strSection, string strKeyName, string strKeyValue, string strFileName)
        {
            var intReturn = WritePrivateProfileString(strSection, strKeyName,
                strKeyValue, strFileName);
            return intReturn != 0;
        }

        public static bool EmptySectionValue(string strSection, string strFileName)
        {
            var intReturn = WritePrivateProfileString(strSection, null, null, strFileName);
            return intReturn != 0;
        }

        public static bool EmptyKeyValue(string strSection, string key, string strFileName)
        {
            var intReturn = WritePrivateProfileString(strSection, key, null, strFileName);
            return intReturn != 0;
        }

        // GetPrivateProfileString 
        [DllImport("kernel32.DLL", EntryPoint = "GetPrivateProfileString", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileString(
            string lpSectionName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize,
            string lpFileName);


        // WritePrivateProfileString 
        [DllImport("kernel32.DLL", EntryPoint = "WritePrivateProfileStringA", SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WritePrivateProfileString(
            string lpSectionName, string lpKeyName, string lpKeyValue, string lpFileName);
    }
}