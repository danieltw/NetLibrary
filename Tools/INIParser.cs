using System.Collections;
using System.IO;
using System.Text;

namespace NetLibrary.Tools
{
    /// <summary>
    /// INI 設定檔分析
    /// </summary>
    /// <remarks>對 INI 設定檔進行新增/查詢/修改與刪除的動作。</remarks>
    public class INIParser
    {
        /// <summary>
        /// 定義檔案的編碼格式
        /// </summary>
        private static Encoding _FileEncoding = Encoding.Unicode;

        #region 取得資料
        /// <summary>
        /// 取回指定鍵值的資料
        /// </summary>
        /// <param name="KeyName">鍵值名稱</param>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在設定檔案中，將指定區塊中的指定鍵值的資料取出。</remarks>
        /// <returns>指定鍵值的資料</returns>
        public static string GetValue(string KeyName, string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return "";
            _FileEncoding = GetFileEncoding(IniFileName);

            KeyName = KeyName.Trim();
            SectionName = SectionName.Trim();

            if ((KeyName == "") || (SectionName == "")) return "";

            System.IO.StreamReader FileReader = new StreamReader(IniFileName, _FileEncoding);
            try
            {
                string tmpLine1, tmpLine2, newLine1, newLine2;
                while (FileReader.Peek() > -1)
                {
                    tmpLine1 = FileReader.ReadLine();
                    newLine1 = tmpLine1.Trim();
                    if (newLine1.StartsWith("[" + SectionName + "]", true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        while (FileReader.Peek() > -1)
                        {
                            tmpLine2 = FileReader.ReadLine();
                            newLine2 = tmpLine2.Trim();
                            if (newLine2.StartsWith(KeyName, true, System.Globalization.CultureInfo.CurrentCulture))
                            {
                                tmpLine2 = TrimString(tmpLine2, "'");
                                tmpLine2 = TrimString(tmpLine2, ";");
                                tmpLine2 = tmpLine2.Split('=').Length > 1 ? tmpLine2.Split('=')[1].Trim() : "";

                                FileReader.Dispose();
                                return tmpLine2;
                            }
                        }
                        break;
                    }
                }
                FileReader.Close();
                return "";
            }
            catch
            {
                return "";
            }
            finally
            {
                FileReader.Dispose();
            }
        }

        /// <summary>
        /// 取回指定區塊內的資料
        /// </summary>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在設定檔案中，將指定區塊中的資料取出。</remarks>
        /// <returns>指定區塊的資料</returns>
        public static Hashtable GetValue(string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return new Hashtable();
            _FileEncoding = GetFileEncoding(IniFileName);

            SectionName = SectionName.Trim();
            if (SectionName == "") return new Hashtable();

            System.IO.StreamReader FileReader = new StreamReader(IniFileName, _FileEncoding);
            try
            {
                Hashtable htReturn = new Hashtable();
                string tmpLine1, tmpLine2, newLine1, newLine2;

                while (FileReader.Peek() > -1)
                {
                    tmpLine1 = FileReader.ReadLine();
                    newLine1 = tmpLine1.Trim();
                    if (newLine1.StartsWith("[" + SectionName + "]", true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        while (FileReader.Peek() > -1)
                        {
                            tmpLine2 = FileReader.ReadLine();
                            newLine2 = tmpLine2.Trim();
                            if (newLine2.StartsWith("[", true, System.Globalization.CultureInfo.CurrentCulture))
                            {
                                break;
                            }
                            else
                            {
                                tmpLine2 = TrimString(tmpLine2, "'");
                                tmpLine2 = TrimString(tmpLine2, ";");
                                if (tmpLine2 != "")
                                {
                                    if (tmpLine2.Split(new char[] { '=' }).Length < 2)
                                        htReturn.Add(tmpLine2.Split(new char[] { '=' })[0].Trim().ToUpper(), "");
                                    else
                                        htReturn.Add(tmpLine2.Split(new char[] { '=' })[0].Trim().ToUpper(), tmpLine2.Split(new char[] { '=' })[1].Trim());
                                }
                            }
                        }
                        break;
                    }
                }
                FileReader.Close();
                return htReturn;
            }
            catch
            {
                return new Hashtable();
            }
            finally
            {
                FileReader.Dispose();
            }
        }

        /// <summary>
        /// 取回指定設定檔的所有資料
        /// </summary>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>將指定設定檔案中，所有的資料取出。</remarks>
        /// <returns>指定檔所有的資料</returns>
        public static Hashtable GetValue(string IniFileName)
        {
            if (!File.Exists(IniFileName)) return new Hashtable();
            _FileEncoding = GetFileEncoding(IniFileName);

            System.IO.StreamReader FileReader = new StreamReader(IniFileName, _FileEncoding);
            try
            {
                Hashtable htReturn = new Hashtable();
                string tmpLine1, tmpSectionName;

                while (FileReader.Peek() > -1)
                {
                    tmpLine1 = FileReader.ReadLine();
                    tmpLine1 = TrimString(tmpLine1, "'");
                    tmpLine1 = TrimString(tmpLine1, ";");
                    if (tmpLine1 != "")
                    {
                        if (tmpLine1.StartsWith("[", false, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            tmpSectionName = tmpLine1.Substring(1, tmpLine1.IndexOf("]") - 1);
                            htReturn.Add(tmpSectionName.Trim().ToUpper(), GetValue(tmpSectionName, IniFileName));
                        }
                    }
                }

                FileReader.Close();
                return htReturn;
            }
            catch
            {
                return new Hashtable();
            }
            finally
            {
                FileReader.Dispose();
            }
        }
        #endregion

        #region 設定資料
        /// <summary>
        /// 設定指定鍵值的資料
        /// </summary>
        /// <param name="KeyValue">新資料</param>
        /// <param name="KeyName">鍵值名稱</param>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在設定檔中，更新指定區塊下的指定鍵的資料。</remarks>
        /// <returns>是否成功設定</returns>
        public static bool SetValue(string KeyValue, string KeyName, string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return false;

            KeyName = KeyName.Trim();
            SectionName = SectionName.Trim();
            KeyValue = KeyValue.Trim();

            if ((KeyName == "") || (SectionName == "")) return false;

            Hashtable AllData = GetValue(IniFileName);
            if (!AllData.ContainsKey(SectionName.ToUpper())) return false;
            Hashtable SectionData = (Hashtable)AllData[SectionName];
            if (!SectionData.ContainsKey(KeyName.ToUpper()))
                SectionData.Add(KeyName, KeyValue);
            else
                SectionData[KeyName] = KeyValue;

            return WriteConfigData(AllData, IniFileName);
        }
        #endregion

        #region 鍵值處理
        /// <summary>
        /// 新增指定鍵值
        /// </summary>
        /// <param name="KeyValue">新資料</param>
        /// <param name="KeyName">新鍵值名稱</param>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在 INI 設定檔案中，指定區塊下新增所指定的鍵值。</remarks>
        /// <returns>是否成功新增</returns>
        public static bool AddKey(string KeyValue, string KeyName, string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return false;

            KeyName = KeyName.Trim();
            SectionName = SectionName.Trim();
            KeyValue = KeyValue.Trim();

            if ((KeyName == "") || (SectionName == "")) return false;

            Hashtable AllData = GetValue(IniFileName);
            if (!AllData.ContainsKey(SectionName.ToUpper())) return false;
            Hashtable SectionData = (Hashtable)AllData[SectionName];

            if (SectionData.ContainsKey(KeyName.ToUpper()))
                SectionData[KeyName] = KeyValue;
            else
                SectionData.Add(KeyName, KeyValue);

            return WriteConfigData(AllData, IniFileName);
        }

        /// <summary>
        /// 刪除指定鍵值
        /// </summary>
        /// <param name="KeyName">鍵值名稱</param>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在 INI 設定檔案中，刪除指定區塊下所指定的鍵值。</remarks>
        /// <returns>是否成功刪除</returns>
        public static bool DeleteKey(string KeyName, string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return false;

            KeyName = KeyName.Trim();
            SectionName = SectionName.Trim();

            if ((KeyName == "") || (SectionName == "")) return false;

            Hashtable AllData = GetValue(IniFileName);
            if (!AllData.ContainsKey(SectionName.ToUpper())) return false;
            Hashtable SectionData = (Hashtable)AllData[SectionName];
            if (!SectionData.ContainsKey(KeyName.ToUpper())) return false;

            SectionData.Remove(KeyName);

            return WriteConfigData(AllData, IniFileName);
        }
        #endregion

        #region 區塊處理(Section)
        /// <summary>
        /// 新增設定區塊
        /// </summary>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在 INI 設定檔案中，新增指定區塊。</remarks>
        /// <returns>是否成功新增</returns>
        public static bool AddSection(string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return false;
            SectionName = SectionName.Trim();
            if (SectionName == "") return false;

            Hashtable AllData = GetValue(IniFileName);
            if (AllData.ContainsKey(SectionName.ToUpper())) return false;

            AllData.Add(SectionName, new Hashtable());

            return WriteConfigData(AllData, IniFileName);
        }

        /// <summary>
        /// 刪除設定區塊
        /// </summary>
        /// <param name="SectionName">區塊名稱</param>
        /// <param name="IniFileName">設定檔的完整檔案名稱</param>
        /// <remarks>在 INI 設定檔案中，刪除指定區塊。</remarks>
        /// <returns>是否成功刪除</returns>
        public static bool DeleteSection(string SectionName, string IniFileName)
        {
            if (!File.Exists(IniFileName)) return false;
            SectionName = SectionName.Trim();
            if (SectionName == "") return false;

            Hashtable AllData = GetValue(IniFileName);
            if (!AllData.ContainsKey(SectionName.ToUpper())) return false;

            AllData.Remove(SectionName);

            return WriteConfigData(AllData, IniFileName);
        }
        #endregion

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
        /// 檢查檔案的編碼格式
        /// </summary>
        /// <param name="FullFileName">完整的檔案路徑</param>
        /// <remarks>讀取文字檔案進行檔案編碼的判斷。</remarks>
        /// <returns>檔案編碼格式</returns>
        public static Encoding GetFileEncoding(string FullFileName)
        {
            if (FullFileName.Trim() == "") return System.Text.Encoding.Default;
            int byeFirst, byeSecond, byeThird, byeFourth;
            try
            {
                FileStream fsReader = System.IO.File.OpenRead(FullFileName);
                byeFirst = fsReader.ReadByte();
                byeSecond = fsReader.ReadByte();
                byeThird = fsReader.ReadByte();
                byeFourth = fsReader.ReadByte();
                fsReader.Close();
                fsReader.Dispose();

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
        /// 寫入設定檔
        /// </summary>
        /// <param name="AllData">要寫入的資料</param>
        /// <param name="IniFileName">完整的檔案路徑</param>
        /// <returns>是否成功寫入</returns>
        public static bool WriteConfigData(Hashtable AllData, string IniFileName)
        {
            if (AllData.Count == 0) return false;

            if (File.Exists(IniFileName))
                _FileEncoding = GetFileEncoding(IniFileName);
            else
                _FileEncoding = Encoding.Unicode;

            System.IO.StreamWriter FileWriter = new StreamWriter(IniFileName, false, _FileEncoding);
            try
            {
                foreach (string _KeyName in AllData.Keys)
                {
                    if (AllData[_KeyName].GetType().Equals(typeof(Hashtable)))
                    {
                        FileWriter.WriteLine(string.Format("[{0}]", _KeyName));
                        WriteSectionData(FileWriter, (Hashtable)AllData[_KeyName]);
                    }
                    else
                        FileWriter.WriteLine(string.Format("{0} = {1}", _KeyName, AllData[_KeyName]));
                }
                FileWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                FileWriter.Dispose();
            }
        }

        /// <summary>
        /// 寫入區段資料
        /// </summary>
        /// <param name="fsWriter">寫入的資料流</param>
        /// <param name="SectionData">區段資料</param>
        private static void WriteSectionData(StreamWriter fsWriter, Hashtable SectionData)
        {
            try
            {
                foreach (string _KeyName in SectionData.Keys)
                {
                    if (SectionData[_KeyName].GetType().Equals(typeof(Hashtable)))
                    {
                        fsWriter.WriteLine(string.Format("[{0}]", _KeyName));
                        WriteSectionData(fsWriter, (Hashtable)SectionData[_KeyName]);
                    }
                    else
                        fsWriter.WriteLine(string.Format("{0} = {1}", _KeyName, SectionData[_KeyName]));
                }
                fsWriter.WriteLine();
            }
            catch
            {
                throw;
            }
        }
    }
}
