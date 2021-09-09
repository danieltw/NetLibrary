using System;
using System.Runtime.InteropServices;

namespace NetLibrary.Tools
{
    /// <summary>
    /// 檔案版本應用
    /// </summary>
    public class FileVersion
    {
        [DllImport("version.dll")]
        private static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);

        [DllImport("version.dll")]
        private static extern int GetFileVersionInfoSize(string sFileName, out int handle);

        /// <summary>
        /// 取回檔案的版本資訊
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <returns>版本編號</returns>
        public static string GetFileVersion(string FileName)
        {
            return _ReadVersion(FileName);
        }

        private static string _ReadVersion(string _FileName)
        {
            int handle = 0;
            int size = GetFileVersionInfoSize(_FileName, out handle);
            if (size == 0) return "";
            byte[] buffer = new byte[size];
            if (!GetFileVersionInfo(_FileName, handle, size, buffer)) return "";
            return string.Format("{0}.{1}.{2}.{3}", ConvertToInt16(buffer[45], buffer[46]), ConvertToInt16(buffer[47], buffer[48]), ConvertToInt16(buffer[49], buffer[50]), ConvertToInt16(buffer[51], buffer[52]));
        }

        private static int ConvertToInt16(byte FirstByte, byte LastByte)
        {
            byte[] tmpByte = new byte[] { LastByte, FirstByte };
            return BitConverter.ToInt16(tmpByte, 0);
        }
    }
}
