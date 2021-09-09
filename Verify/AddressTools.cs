using System.Text.RegularExpressions;

namespace NetLibrary.Verify
{
    public class TaiwanAddress
    {
        private readonly static string AddressPattern = @"(?<zipcode>(^\\d{5}|^\\d{3})?)(?<city>\\D+?[縣市])(?<region>\\D+?(市區|鎮區|鎮市|[鄉鎮市區]))?(?<village>\\D+?[村里])?(?<neighbor>\\d+[鄰])?(?<road>\\D+?(村路|[路街道段]))?(?<section>\\D?段)?(?<lane>\\d+巷)?(?<alley>\\d+弄)?(?<no>\\d+號?)?(?<seq>-\\d+?(號))?(?<floor>\\d+樓)?(?<room>\\d+室)?(?<others>.+)?";

        /// <summary>
        /// 判斷是否為合法的台灣地址
        /// </summary>
        /// <param name="Address">台灣地址</param>
        public static bool IsValidAddress(string Address)
        {
            Regex _tmpRegex = new Regex(AddressPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(Address);
        }

        /// <summary>
        /// 分析處理地址資訊，並轉為json字串
        /// </summary>
        /// <param name="Address">地址</param>
        public static string TryParse(string Address)
        {
            AddressClass _addr = new AddressClass()
            {
                IsParseSuccessed = false,
                OrginalAddress = Address
            };

            Match match = new Regex(AddressPattern, RegexOptions.IgnoreCase).Match(Address);
            if (match.Success)
            {
                _addr.IsParseSuccessed = true;
                _addr.City = match.Groups["city"].ToString();
                _addr.Region = match.Groups["region"].ToString();
                _addr.Village = match.Groups["village"].ToString();
                _addr.Neighbor = match.Groups["neighbor"].ToString();
                _addr.Road = match.Groups["road"].ToString();
                _addr.Section = match.Groups["section"].ToString();
                _addr.Lane = match.Groups["lane"].ToString();
                _addr.Alley = match.Groups["alley"].ToString();
                _addr.No = match.Groups["no"].ToString();
                _addr.Seq = match.Groups["seq"].ToString();
                _addr.Floor = match.Groups["floor"].ToString();
                _addr.Room = match.Groups["room"].ToString();
                _addr.Others = match.Groups["others"].ToString();
                _addr.IsParseSuccessed = true;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(_addr, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// 地址分析類別
        /// </summary>
        public class AddressClass
        {
            /// <summary>
            /// 縣市
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 鄉鎮市區
            /// </summary>
            public string Region { get; set; }

            /// <summary>
            /// 村里
            /// </summary>
            public string Village { get; set; }

            /// <summary>
            /// 鄰
            /// </summary>
            public string Neighbor { get; set; }

            /// <summary>
            /// 路
            /// </summary>
            public string Road { get; set; }

            /// <summary>
            /// 段
            /// </summary>
            public string Section { get; set; }

            /// <summary>
            /// 巷
            /// </summary>
            public string Lane { get; set; }

            /// <summary>
            /// 弄
            /// </summary>
            public string Alley { get; set; }

            /// <summary>
            /// 號
            /// </summary>
            public string No { get; set; }

            /// <summary>
            /// 序號
            /// </summary>
            public string Seq { get; set; }

            /// <summary>
            /// 樓
            /// </summary>
            public string Floor { get; set; }

            /// <summary>
            /// 室
            /// </summary>
            public string Room { get; set; }

            /// <summary>
            /// 其他
            /// </summary>
            public string Others { get; set; }

            /// <summary>
            /// 是否符合pattern規範
            /// </summary>
            public bool IsParseSuccessed { get; set; }

            /// <summary>
            /// 原始傳入的地址
            /// </summary>
            public string OrginalAddress { get; set; }
        }
    }
}
