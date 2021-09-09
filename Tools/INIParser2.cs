using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetLibrary.Tools
{
    public class INIParser2
    {
        #region 讀取資料
        #region 所有資料
        /// <summary>
        /// 自檔案中讀取設定資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        public static ConfigData LoadFromFile(string FileName)
        {
            try
            {
                if (!File.Exists(FileName)) return new ConfigData();
                Encoding _FileEncoding = GetFileEncoding(FileName);

                return LoadFromFile(FileName, _FileEncoding);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自檔案中讀取設定資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="FileEncoding">檔案編碼</param>
        public static ConfigData LoadFromFile(string FileName, Encoding FileEncoding)
        {
            ConfigData objReturn = new ConfigData();
            try
            {
                if (!File.Exists(FileName)) return objReturn;

                List<string> AllTextLine = new List<string>();
                using (System.IO.StreamReader FileReader = new StreamReader(FileName, FileEncoding))
                {
                    while (FileReader.Peek() > -1)
                    {
                        AllTextLine.Add(FileReader.ReadLine());
                    }
                }

                objReturn = ProcessText(AllTextLine);
                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自文字字串讀取設定資料
        /// </summary>
        /// <param name="AllConfigString">文字字串</param>
        public static ConfigData LoadFromString(string AllConfigString)
        {
            try
            {
                List<string> AllTextLine = AllConfigString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList<string>();
                return ProcessText(AllTextLine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 區段資料
        /// <summary>
        /// 讀取區段資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        public static SectionItem ReadSectionData(string FileName, string SectionName)
        {
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName)) return new SectionItem();
                Encoding FileEncoding = GetFileEncoding(FileName);

                return ReadSectionData(FileName, SectionName, FileEncoding);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        /// <param name="FileEncoding">檔案編碼</param>
        public static SectionItem ReadSectionData(string FileName, string SectionName, Encoding FileEncoding)
        {
            SectionItem objReturn = new SectionItem();
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName)) return objReturn;
                using (System.IO.StreamReader FileReader = new StreamReader(FileName, FileEncoding))
                {
                    bool _chkF = false;
                    int _chdSN = 0;
                    while (FileReader.Peek() > -1)
                    {
                        string tmpS = FileReader.ReadLine();
                        if (tmpS.ToUpper().StartsWith(string.Format("[{0}]", SectionName.ToUpper())))
                        {
                            if (!_chkF)
                            {
                                _chkF = true;
                                objReturn.SectionSN = 0;
                                objReturn.SectionName = SectionName;
                                if (tmpS.IndexOf(';') > 0)
                                {
                                    if (tmpS.Length > (tmpS.IndexOf(';') + 1)) objReturn.SectionDesc = tmpS.Substring(tmpS.IndexOf(';') + 1);
                                }
                                _chdSN = 1;
                            }
                        }
                        else if (_chkF)
                        {
                            if (tmpS.StartsWith("[")) break;

                            if (tmpS.TrimStart().StartsWith(";") || tmpS.TrimStart().StartsWith("#"))
                            {
                                objReturn.ChildItems.Add(new KeyItem()
                                {
                                    KeySN = _chdSN,
                                    KeyDesc = tmpS
                                });
                            }
                            else
                            {
                                string _keyDesc = "";
                                string _keyName = "";
                                string _keyValue = "";

                                if (tmpS.IndexOf('=') < 1)
                                {
                                    _keyDesc = tmpS;
                                }
                                else
                                {
                                    if (tmpS.IndexOf(';') > 0)
                                    {
                                        _keyDesc = tmpS.Length > (tmpS.IndexOf(';') + 1) ? tmpS.Substring(tmpS.IndexOf(';') + 1) : "";
                                        string _keyText = tmpS.Substring(0, tmpS.IndexOf(';')).Trim();

                                        _keyName = _keyText.Split('=')[0].Trim();
                                        _keyValue = _keyText.Split('=')[1].Trim();
                                    }
                                    else
                                    {
                                        _keyName = tmpS.Split('=')[0].Trim();
                                        _keyValue = tmpS.Split('=')[1].Trim();
                                    }
                                }

                                objReturn.ChildItems.Add(new KeyItem()
                                {
                                    KeySN = _chdSN,
                                    KeyName = _keyName,
                                    KeyValue = _keyValue,
                                    KeyDesc = _keyDesc
                                });
                            }
                            _chdSN++;
                        }
                    }
                }

                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionNames">區段名稱清單</param>
        public static List<SectionItem> ReadSectionData(string FileName, List<string> SectionNames)
        {
            try
            {
                if (!File.Exists(FileName) || (SectionNames.Count() == 0)) return new List<SectionItem>();
                Encoding FileEncoding = GetFileEncoding(FileName);

                return ReadSectionData(FileName, SectionNames, FileEncoding);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionNames">區段名稱清單</param>
        /// <param name="FileEncoding">檔案編碼</param>
        public static List<SectionItem> ReadSectionData(string FileName, List<string> SectionNames, Encoding FileEncoding)
        {
            List<SectionItem> objReturn = new List<SectionItem>();
            try
            {
                if (!File.Exists(FileName) || (SectionNames.Count() == 0)) return objReturn;

                SectionNames.ForEach(x =>
                {
                    objReturn.Add(ReadSectionData(FileName, x, FileEncoding));
                });

                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 項目資料
        /// <summary>
        /// 讀取區段內指定鍵的資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        /// <param name="KeyName">鍵名稱</param>
        public static string ReadItemValue(string FileName, string SectionName, string KeyName)
        {
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName) || string.IsNullOrWhiteSpace(KeyName)) return string.Empty;
                Encoding FileEncoding = GetFileEncoding(FileName);

                return ReadItemValue(FileName, SectionName, KeyName, FileEncoding);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段內指定鍵的資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        /// <param name="KeyName">鍵名稱</param>
        /// <param name="FileEncoding">檔案編碼</param>
        public static string ReadItemValue(string FileName, string SectionName, string KeyName, Encoding FileEncoding)
        {
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName) || string.IsNullOrWhiteSpace(KeyName)) return string.Empty;

                SectionItem objSection = ReadSectionData(FileName, SectionName, FileEncoding);
                return objSection.ChildItems.SingleOrDefault(x => x.KeyName.ToUpper() == KeyName.ToUpper()).KeyValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段內多筆鍵的資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        /// <param name="KeyNames">鍵名稱清單</param>
        public static Dictionary<string, string> ReadItemData(string FileName, string SectionName, List<string> KeyNames)
        {
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName) || (KeyNames.Count == 0)) return new Dictionary<string, string>();
                Encoding FileEncoding = GetFileEncoding(FileName);

                return ReadItemData(FileName, SectionName, KeyNames, FileEncoding);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 讀取區段內多筆鍵的資料
        /// </summary>
        /// <param name="FileName">檔案名稱</param>
        /// <param name="SectionName">區段名稱</param>
        /// <param name="KeyNames">鍵名稱清單</param>
        /// <param name="FileEncoding">檔案編碼</param>
        public static Dictionary<string, string> ReadItemData(string FileName, string SectionName, List<string> KeyNames, Encoding FileEncoding)
        {
            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            try
            {
                if (!File.Exists(FileName) || string.IsNullOrWhiteSpace(SectionName) || (KeyNames.Count == 0)) return dicReturn;

                SectionItem objSection = ReadSectionData(FileName, SectionName, FileEncoding);

                KeyNames.ForEach(x =>
                {
                    dicReturn.Add(x, objSection.ChildItems.SingleOrDefault(z => z.KeyName.ToUpper() == x.ToUpper()).KeyValue);

                });

                return dicReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        #region 儲存檔案
        /// <summary>
        /// 儲存設定資料
        /// </summary>
        /// <param name="configData">設定資料</param>
        /// <param name="FullFileName">檔案名稱</param>
        public static bool SaveFile(ConfigData configData, string FullFileName)
        {
            return SaveFile(configData, FullFileName, Encoding.Unicode);
        }

        /// <summary>
        /// 儲存設定資料
        /// </summary>
        /// <param name="configData">設定資料</param>
        /// <param name="FullFileName">檔案名稱</param>
        /// <param name="fileEncoding">檔案編碼</param>
        public static bool SaveFile(ConfigData configData, string FullFileName, Encoding fileEncoding)
        {
            if (configData.Items.Count == 0) return false;

            try
            {
                using (System.IO.StreamWriter FW = new StreamWriter(FullFileName, false, fileEncoding))
                {
                    if (configData.HeaderDesc.Count > 0)
                    {
                        configData.HeaderDesc.ForEach(x => { FW.WriteLine(string.Format("{0}", x)); });
                        FW.WriteLine("");
                    }

                    configData.Items.OrderBy(x => x.SectionSN).ToList().ForEach(x =>
                    {
                        if (x.SectionState != StateENUM.Deleted)
                        {
                            if (x.IsDescLine)
                            {
                                FW.WriteLine(string.Format("{0}", x.SectionDesc));
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(x.SectionDesc))
                                    FW.WriteLine(string.Format("[{0}]", x.SectionName));
                                else
                                    FW.WriteLine(string.Format("[{0}]   ; {1}", x.SectionName, x.SectionDesc));

                                if (x.HasChild)
                                {
                                    x.ChildItems.OrderBy(z => z.KeySN).ToList().ForEach(y =>
                                    {
                                        if (y.KeyState != StateENUM.Deleted)
                                        {
                                            if (y.IsDescLine)
                                            {
                                                FW.WriteLine(string.Format("{0}", y.KeyDesc));
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(y.KeyDesc))
                                                    FW.WriteLine(string.Format("{0} = {1}", y.KeyName, y.KeyValue));
                                                else
                                                    FW.WriteLine(string.Format("{0} = {1}   ; {2}", y.KeyName, y.KeyValue, y.KeyDesc));
                                            }
                                        }
                                    });
                                }
                            }

                            FW.WriteLine("");
                        }
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 區段處理
        #region 新增區段
        public static bool CreateSection(string FullFileName, string sectionName, string sectionDataText)
        {
            return true;
        }

        public static bool CreateSection(string FullFileName, string sectionName, string sectionDataText, Encoding fileEncoding)
        {
            return true;
        }

        public static bool CreateSection(string FullFileName, SectionItem sectionData)
        {
            return true;
        }

        public static bool CreateSection(string FullFileName, SectionItem sectionData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion

        #region 更新區段
        public static bool UpdateSection(string FullFileName, string sectionName, string sectionDataText)
        {
            return true;
        }

        public static bool UpdateSection(string FullFileName, string sectionName, string sectionDataText, Encoding fileEncoding)
        {
            return true;
        }

        public static bool UpdateSection(string FullFileName, SectionItem sectionData)
        {
            return true;
        }

        public static bool UpdateSection(string FullFileName, SectionItem sectionData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion

        #region 刪除區段
        public static bool DeleteSection(string FullFileName, string sectionName)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, string sectionName, Encoding fileEncoding)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, List<string> sectionNames)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, List<string> sectionNames, Encoding fileEncoding)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, SectionItem sectionData)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, SectionItem sectionData, Encoding fileEncoding)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, List<SectionItem> sectionsData)
        {
            return true;
        }

        public static bool DeleteSection(string FullFileName, List<SectionItem> sectionsData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion
        #endregion

        #region 項目處理
        #region 新增項目
        public static bool CreateItem(string FullFileName, string sectionName, string ItemName, string ItemValue)
        {
            return true;
        }

        public static bool CreateItem(string FullFileName, string sectionName, string ItemName, string ItemValue, Encoding fileEncoding)
        {
            return true;
        }

        public static bool CreateItem(string FullFileName, string sectionName, KeyItem ItemData)
        {
            return true;
        }

        public static bool CreateItem(string FullFileName, string sectionName, KeyItem ItemData, Encoding fileEncoding)
        {
            return true;
        }

        public static bool CreateItem(string FullFileName, string sectionName, List<KeyItem> ItemData)
        {
            return true;
        }

        public static bool CreateItem(string FullFileName, string sectionName, List<KeyItem> ItemData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion

        #region 更新項目
        public static bool UpdateItem(string FullFileName, string sectionName, string ItemName, string ItemValue)
        {
            return true;
        }

        public static bool UpdateItem(string FullFileName, string sectionName, string ItemName, string ItemValue, Encoding fileEncoding)
        {
            return true;
        }

        public static bool UpdateItem(string FullFileName, string sectionName, KeyItem ItemData)
        {
            return true;
        }

        public static bool UpdateItem(string FullFileName, string sectionName, KeyItem ItemData, Encoding fileEncoding)
        {
            return true;
        }

        public static bool UpdateItem(string FullFileName, string sectionName, List<KeyItem> ItemData)
        {
            return true;
        }

        public static bool UpdateItem(string FullFileName, string sectionName, List<KeyItem> ItemData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion

        #region 刪除項目
        public static bool DeleteItem(string FullFileName, string sectionName, string ItemName)
        {
            return true;
        }

        public static bool DeleteItem(string FullFileName, string sectionName, string ItemName, Encoding fileEncoding)
        {
            return true;
        }

        public static bool DeleteItem(string FullFileName, string sectionName, List<string> ItemNames)
        {
            return true;
        }

        public static bool DeleteItem(string FullFileName, string sectionName, List<string> ItemNames, Encoding fileEncoding)
        {
            return true;
        }

        public static bool DeleteItem(string FullFileName, string sectionName, List<KeyItem> ItemData)
        {
            return true;
        }

        public static bool DeleteItem(string FullFileName, string sectionName, List<KeyItem> ItemData, Encoding fileEncoding)
        {
            return true;
        }
        #endregion
        #endregion

        /// <summary>
        /// 設定資料類別
        /// </summary>
        public class ConfigData
        {
            public List<string> HeaderDesc { get; set; }
            public List<SectionItem> Items { get; set; }
        }

        /// <summary>
        /// 區段資料類別
        /// </summary>
        public class SectionItem
        {
            /// <summary>
            /// 初始化
            /// </summary>
            public SectionItem()
            {
                ChildItems = null;
                SectionState = StateENUM.UnChanged;
            }

            /// <summary>
            /// 區段序號
            /// </summary>
            public int SectionSN { get; set; }

            /// <summary>
            /// 區段名稱
            /// </summary>
            public string SectionName { get; set; }

            /// <summary>
            /// 區段狀態
            /// </summary>
            public StateENUM SectionState { get; set; }

            /// <summary>
            /// 區段註解
            /// </summary>
            public string SectionDesc { get; set; }

            /// <summary>
            /// 項目資料
            /// </summary>
            public List<KeyItem> ChildItems { get; set; }

            /// <summary>
            /// 取回是否有子項目資料
            /// </summary>
            public bool HasChild { get { return (ChildItems == null || ChildItems.Count() <= 0) ? false : true; } }

            /// <summary>
            /// 是否為註解行
            /// </summary>
            public bool IsDescLine { get { return string.IsNullOrWhiteSpace(SectionName); } }
        }

        /// <summary>
        /// 項目資料類別
        /// </summary>
        public class KeyItem
        {
            /// <summary>
            /// 初始化
            /// </summary>
            public KeyItem()
            {
                KeyState = StateENUM.UnChanged;
            }

            /// <summary>
            /// 項目序號
            /// </summary>
            public int KeySN { get; set; }

            /// <summary>
            /// 項目名稱
            /// </summary>
            public string KeyName { get; set; }

            /// <summary>
            /// 項目資料
            /// </summary>
            public string KeyValue { get; set; }

            /// <summary>
            /// 項目狀態
            /// </summary>
            public StateENUM KeyState { get; set; }

            /// <summary>
            /// 項目註解
            /// </summary>
            public string KeyDesc { get; set; }

            /// <summary>
            /// 是否為註解行
            /// </summary>
            public bool IsDescLine { get { return string.IsNullOrWhiteSpace(KeyName); } }
        }

        /// <summary>
        /// 項目狀態清單
        /// </summary>
        public enum StateENUM : int
        {
            /// <summary>
            /// 未變更
            /// </summary>
            UnChanged = 0,
            /// <summary>
            /// 新資料
            /// </summary>
            New = 1,
            /// <summary>
            /// 已異動
            /// </summary>
            Modified = 5,
            /// <summary>
            /// 已刪除
            /// </summary>
            Deleted = 9
        }

        /// <summary>
        /// 檢查檔案的編碼格式
        /// </summary>
        /// <param name="FullFileName">完整的檔案路徑</param>
        /// <remarks>讀取文字檔案進行檔案編碼的判斷。</remarks>
        /// <returns>檔案編碼格式</returns>
        private static Encoding GetFileEncoding(string FullFileName)
        {
            if (FullFileName.Trim() == "") return System.Text.Encoding.Default;
            int byeFirst, byeSecond, byeThird, byeFourth;
            try
            {
                using (FileStream fsReader = System.IO.File.OpenRead(FullFileName))
                {
                    byeFirst = fsReader.ReadByte();
                    byeSecond = fsReader.ReadByte();
                    byeThird = fsReader.ReadByte();
                    byeFourth = fsReader.ReadByte();
                }

                if ((byeFirst == 255) && (byeSecond == 254))
                {
                    if (byeThird == 0)
                        return System.Text.Encoding.UTF32;
                    else
                        return System.Text.Encoding.Unicode;
                }
                else if ((byeFirst == 254) && (byeSecond == 255))
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if ((byeFirst == 239) && (byeSecond == 187) && (byeThird == 191))
                {
                    return System.Text.Encoding.UTF8;
                }

                return System.Text.Encoding.Default;
            }
            catch
            {
                return System.Text.Encoding.Default;
            }
        }

        /// <summary>
        /// 在字串中去除指定字元後的所有資料
        /// </summary>
        /// <param name="OriginString">原始字串</param>
        /// <param name="TrimChar">要去除的指定字元</param>
        private static string TrimString(string OriginString, string TrimChar)
        {
            if (TrimChar.Trim() == "") return OriginString;
            string strReturn = OriginString;
            try
            {
                if (strReturn.IndexOf(TrimChar) >= 0)
                {
                    strReturn = strReturn.Substring(0, strReturn.IndexOf(TrimChar));
                    strReturn = strReturn.Trim();
                }
                return strReturn;
            }
            catch
            {
                return OriginString;
            }
        }

        /// <summary>
        /// 處理來源的設定文字資料
        /// </summary>
        /// <param name="SourceText">設定文字資料</param>
        private static ConfigData ProcessText(List<string> SourceText)
        {
            ConfigData objReturn = new ConfigData();
            try
            {
                if (SourceText.Count == 0) return objReturn;

                int iSecSN = 0;
                int iKeySN = 0;
                foreach (string tmpS in SourceText)
                {
                    SectionItem objSection = new SectionItem();
                    if (tmpS.TrimStart().StartsWith(";") || tmpS.TrimStart().StartsWith("#"))
                    {
                        // desc
                        if (iSecSN == 0)
                        {
                            objReturn.HeaderDesc.Add(tmpS);
                        }
                        else
                        {
                            
                        }
                    }
                }


                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
