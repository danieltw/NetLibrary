using System.Text.RegularExpressions;

namespace NetLibrary.Verify
{
    /// <summary>
    /// 文字驗證應用
    /// </summary>
    public class TextVerify
    {
        /// <summary>
        /// 判斷是否為合法的郵件地址
        /// </summary>
        /// <param name="MailAddress">郵件地址</param>
        public static bool IsValidMailAddress(string MailAddress)
        {
            string RegPattern = @"^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(MailAddress);
        }

        /// <summary>
        /// 判斷是否為合法的台灣市話號碼
        /// </summary>
        /// <param name="PhoneNumber">手機號碼</param>
        public static bool IsValidPhoneNummberTW(string PhoneNumber)
        {
            string RegPattern = @"^(0)([0-9]{1})([-]?)([0-9]{6,8})$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(PhoneNumber);
        }

        /// <summary>
        /// 判斷是否為合法的台灣手機號碼
        /// </summary>
        /// <param name="CellPhoneNumber">手機號碼</param>
        public static bool IsValidCellPhoneNummberTW(string CellPhoneNumber)
        {
            string RegPattern = @"^(09)([0-9]{2})([-]?)([0-9]{6})$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(CellPhoneNumber);
        }

        /// <summary>
        /// 判斷是否為合法的中國手機號碼
        /// </summary>
        /// <param name="CellPhoneNumber">手機號碼</param>
        public static bool IsValidCellPhoneNummberCN(string CellPhoneNumber)
        {
            string RegPattern = @"^([0-9]{4})([-]?)([0-9]{3})([-]?)([0-9]{4})$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(CellPhoneNumber);
        }

        /// <summary>
        /// 判斷是否為合法的密碼規則。數字與字母的組合，最少6個字元，最多32個字元。
        /// </summary>
        /// <param name="CheckPassword">密碼文字</param>
        /// <param name="MinLength">密碼最小長度</param>
        /// <param name="MaxLength">密碼最大長度</param>
        public static bool IsValidPassword(string CheckPassword, int MinLength, int MaxLength)
        {
            if (MinLength > MaxLength) return false;
            if ((MinLength <= 0) || (MaxLength <= 0)) return false;

            string RegPattern = @"^(?=.*\d)(?=.*[a-zA-Z]).{" + MinLength.ToString() + "." + MaxLength.ToString() + "}$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.None);
            return _tmpRegex.IsMatch(CheckPassword);
        }

        /// <summary>
        /// 判斷是否為合法的IP位址
        /// </summary>
        /// <param name="IPAddress">IP位址</param>
        public static bool IsValidIPAddress(string IPAddress)
        {
            string RegPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(IPAddress);
        }
    }
}
