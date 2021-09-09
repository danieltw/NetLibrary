using System;
using System.Net;
using System.Net.NetworkInformation;

namespace NetLibrary.Tools
{
    /// <summary>
    /// IP 位址處理
    /// </summary>
    /// <remarks>網路卡 IP 與 MAC 資料處理</remarks>
    public class IPTools
    {
        #region IP 位址
        /// <summary>
        /// 取回所有網路卡 IP 位址
        /// </summary>
        /// <remarks>取回電腦上所有的 IP 位址資料(字串格式 xxx.xxx.xxx.xxx)。</remarks>
        /// <returns>IP 位址字串陣列</returns>
        public static string[] GetAllNicIPAddr()
        {
            string strHostName, tmpString;
            int intCount, tmpCount = 0;
            string[] strReturn = new string[0];
            try
            {
                strHostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
                intCount = System.Net.Dns.GetHostEntry(strHostName).AddressList.Length;
                strReturn = new string[intCount - 1];

                for (int i = 0; i < intCount; i++)
                {
                    IPAddress tmpIPaddr = Dns.GetHostEntry(strHostName).AddressList[i];
                    tmpString = tmpIPaddr.ToString();
                    if (!tmpIPaddr.IsIPv6LinkLocal)
                    {
                        strReturn[tmpCount] = "";
                    }
                    else
                    {
                        if (tmpString.IndexOf('.') <= 0)
                        {
                            strReturn[tmpCount] = "";
                        }
                        else
                        {
                            string[] sIP = tmpString.Split('.');
                            tmpString = Convert.ToInt32(sIP[0]).ToString("000") + "." + Convert.ToInt32(sIP[1]).ToString("000") + "." + Convert.ToInt32(sIP[2]).ToString("000") + "." + Convert.ToInt32(sIP[3]).ToString("000");
                        }
                        tmpCount++;
                    }
                }

                Array.Resize<string>(ref strReturn, tmpCount);
            }
            catch { }
            return strReturn;
        }

        /// <summary>
        /// 取回所有網路卡 IP 位址(數值模式)
        /// </summary>
        /// <remarks>取回電腦上所有的 IP 位址資料(10進位數值格式)。</remarks>
        /// <returns>IP 位址數值陣列</returns>
        public static long[] GetAllNicIPAddrNumber()
        {
            long[] lngReturn = new long[0];
            try
            {
                string[] strReturn = GetAllNicIPAddr();

                lngReturn = new long[strReturn.Length - 1];

                for (int i = 0; i < strReturn.Length; i++)
                {
                    if (strReturn[i].Trim() == "")
                    {
                        lngReturn[i] = 0;
                    }
                    else
                    {
                        lngReturn[i] = IP2DEC(strReturn[i]);
                    }
                }
            }
            catch { }
            return lngReturn;
        }

        /// <summary>
        /// 取回網路卡 IP 位址(字串)
        /// </summary>
        /// <remarks>取回電腦中的第一個 IP 位址(字串格式 xxx.xxx.xxx.xxx)。</remarks>
        /// <returns>IP 位址</returns>
        public static string GetNicIPAddr()
        {
            return GetNicIPAddr(0);
        }

        /// <summary>
        /// 取回指定編號的網路卡 IP 位址(字串)
        /// </summary>
        /// <param name="NICIndex">編號</param>
        /// <remarks>取回電腦中指定編號的 IP 位址(字串格式 xxx.xxx.xxx.xxx)。</remarks>
        /// <returns>IP 位址</returns>
        public static string GetNicIPAddr(int NICIndex)
        {
            string strHostName = "", tmpString = "";
            try
            {
                strHostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
                tmpString = System.Net.Dns.GetHostEntry(strHostName).AddressList[NICIndex].ToString();
                if ((tmpString.Trim() == "") || (tmpString.IndexOf('.') <= 0))
                {
                    tmpString = GetNicIPAddr(NICIndex + 1);
                }
                else
                {
                    string[] sIP = tmpString.Split('.');
                    tmpString = Convert.ToInt32(sIP[0]).ToString("000") + "." + Convert.ToInt32(sIP[1]).ToString("000") + "." + Convert.ToInt32(sIP[2]).ToString("000") + "." + Convert.ToInt32(sIP[3]).ToString("000");
                }
            }
            catch
            {
                tmpString = "";
            }
            return tmpString;
        }

        /// <summary>
        /// 取回網路卡 IP 位址(數值模式)
        /// </summary>
        /// <remarks>取回電腦中的第一個 IP 位址(10進位數值格式)。</remarks>
        /// <returns>IP 位址數值資料</returns>
        public static long GetNicIPAddrNumber()
        {
            return GetNicIPAddrNumber(0);
        }

        /// <summary>
        /// 取回指定編號的網路卡 IP 位址(數值模式)
        /// </summary>
        /// <param name="NICIndex">編號。</param>
        /// <remarks>取回電腦中指定編號的 IP 位址(10進位數值格式)。</remarks>
        /// <returns>IP 位址數值資料</returns>
        public static long GetNicIPAddrNumber(int NICIndex)
        {
            string strAddr = GetNicIPAddr(NICIndex);
            if (strAddr.Trim() == "") return 0;

            return IP2DEC(strAddr);
        }
        #endregion

        #region MAC 位址
        /// <summary>
        /// 取回所有網路卡 MAC 位址
        /// </summary>
        /// <remarks>取回電腦中所有網路卡的 MAC 位址。</remarks>
        /// <returns>MAC 位址字串陣列</returns>
        public static string[] GetAllNICMac()
        {
            try
            {
                int intCount = NetworkInterface.GetAllNetworkInterfaces().Length;
                NetworkInterface[] allNetworkInterface = NetworkInterface.GetAllNetworkInterfaces();
                string tmpString;
                string[] strReturn = new string[intCount - 1];
                NetworkInterface tmpAdapter;

                for (int i = 0; i < intCount; i++)
                {
                    tmpAdapter = allNetworkInterface[i];
                    if ((tmpAdapter.Supports(NetworkInterfaceComponent.IPv4)) && (tmpAdapter.OperationalStatus == OperationalStatus.Up) && (!tmpAdapter.IsReceiveOnly))
                    {
                        tmpString = tmpAdapter.GetPhysicalAddress().ToString();
                        if (tmpString.Trim() == "")
                        {
                            strReturn[i] = "";
                        }
                        else
                        {
                            strReturn[i] = TransferNICMac(tmpString, ':');
                        }
                    }
                }
                return strReturn;
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 取回網路卡 MAC 位址 (aa:bb:cc:dd:ee:ff)
        /// </summary>
        /// <remarks>取回電腦中目前作用中的網路卡 MAC 位址。</remarks>
        /// <returns>MAC 位址字串</returns>
        public static string GetNICMac()
        {
            return GetNICMac(0);
        }

        /// <summary>
        /// 取回指定編號的網路卡 MAC 位址 (aa:bb:cc:dd:ee:ff)
        /// </summary>
        /// <param name="NICIndex">網卡編號。</param>
        /// <remarks>取回電腦中指定編號網路卡 MAC 位址。</remarks>
        /// <returns>MAC 位址字串</returns>
        public static string GetNICMac(int NICIndex)
        {
            NetworkInterface curNetworkInterface;
            NetworkInterface[] allNetworkInterface;
            string strReturn = "";
            try
            {
                allNetworkInterface = NetworkInterface.GetAllNetworkInterfaces();
                curNetworkInterface = allNetworkInterface[NICIndex];

                strReturn = curNetworkInterface.GetPhysicalAddress().ToString();
                if ((strReturn.Trim() == "") || (strReturn.Trim().Length != 12)) strReturn = GetNICMac(NICIndex + 1);
                strReturn = TransferNICMac(strReturn, ':');
            }
            catch
            {
                strReturn = "";
            }
            return strReturn;
        }

        /// <summary>
        /// 將 MAC 位址加上指定的分隔符號
        /// </summary>
        /// <param name="MACs">原始 MAC 字串</param>
        /// <param name="SeparateChar">分隔字元</param>
        private static string TransferNICMac(string MACs, char SeparateChar)
        {
            if (MACs.Trim() == "") return "";
            string strReturn = "";

            try
            {
                if ((MACs.Length % 2) == 1) MACs = "0" + MACs;

                for (int i = 0; i < MACs.Length; i = i + 2)
                {
                    strReturn = (strReturn == "" ? "" : strReturn + ":") + MACs[i] + MACs[i + 1];
                }
            }
            catch
            {
                strReturn = "";
            }
            return strReturn;
        }
        #endregion

        #region IP 位址轉數值
        /// <summary>
        /// 將 IP 位址轉換為 IP 數值
        /// </summary>
        /// <param name="IPAddress">IPAddress 格式的 IP 位址</param>
        /// <remarks>將 IPAddress 格式的 IP 位址資料，轉換為 10 進位格式的資料。</remarks>
        /// <returns>IP 代表數值資料</returns>
        public static long IP2DEC(IPAddress IPAddress)
        {
            return IP2DEC(IPAddress.ToString());
        }

        /// <summary>
        /// 將 IP 位址 (nnn.nnn.nnn.nnn) 轉換為 IP 數值(低位元到高位元)
        /// </summary>
        /// <param name="IPAddress">文字格式的 IP 位址</param>
        /// <remarks>將文字格式的 IP 位址轉換為 10 進位格式的資料。<br />
        /// 轉換模式為：低位元到高位元。</remarks>
        /// <returns>IP 代表數值資料</returns>
        public static long IP2DEC(string IPAddress)
        {
            return IP2DEC(IPAddress, false);
        }

        /// <summary>
        /// 將 IP 位址 (nnn.nnn.nnn.nnn) 依指定轉換模式，轉換為 IP 數值
        /// </summary>
        /// <param name="IPAddress">文字格式的 IP 位址</param>
        /// <param name="IsReverse">是否為低位元到高位元</param>
        /// <remarks>將文字格式的 IP 位址轉換為 10 進位格式的資料。</remarks>
        /// <returns>IP 代表數值資料</returns>
        public static long IP2DEC(string IPAddress, bool IsReverse)
        {
            string[] strIPs;
            long lngReturn = 0;
            strIPs = IPAddress.Split(new char[] { '.' });
            if (strIPs.Length != 4) return 0;

            try
            {
                for (int i = 0; i < strIPs.Length; i++)
                {

                    if ((Convert.ToInt32(strIPs[i]) < 0) || (Convert.ToInt32(strIPs[i]) > 255)) return 0;
                }

                if (IsReverse)
                {
                    lngReturn = Convert.ToInt64(strIPs[3]) * (long)Math.Pow(256, 3) + Convert.ToInt64(strIPs[2]) * (long)Math.Pow(256, 2) + Convert.ToInt64(strIPs[1]) * (long)Math.Pow(256, 1) + Convert.ToInt64(strIPs[0]);
                }
                else
                {
                    lngReturn = Convert.ToInt64(strIPs[0]) * (long)Math.Pow(256, 3) + Convert.ToInt64(strIPs[1]) * (long)Math.Pow(256, 2) + Convert.ToInt64(strIPs[2]) * (long)Math.Pow(256, 1) + Convert.ToInt64(strIPs[3]);
                }
            }
            catch
            {
                lngReturn = 0;
            }
            return lngReturn;
        }
        #endregion

        #region IP 數值轉為 IP 位址
        /// <summary>
        /// 將 IP 數值轉換為 IP 位址 (nnn.nnn.nnn.nnn)
        /// </summary>
        /// <param name="IPNumbers">IP 數值</param>
        /// <remarks>將 10 進位格式的 IP 位址資料，轉換為文字格式，數值固定為 3 碼。</remarks>
        /// <returns>文字格式的 IP 位址</returns>
        public static string DEC2IP(long IPNumbers)
        {
            return DEC2IP(IPNumbers, true, false);
        }

        /// <summary>
        /// 將 IP 數值轉換為 IP 位址
        /// </summary>
        /// <param name="IPNumbers">IP 數值</param>
        /// <param name="IsFixCode">數值是否固定 3 碼</param>
        /// <remarks>將 10 進位格式的 IP 位址資料，依設定轉換為文字格式。</remarks>
        /// <returns>文字格式的 IP 位址</returns>
        public static string DEC2IP(long IPNumbers, bool IsFixCode, bool IsBigEndian)
        {

            string strReturn = "";
            int[] intIPs = new int[4];
            if ((IPNumbers < 0) || (IPNumbers > 4294916294)) return "";

            try
            {
                if (IsBigEndian)
                {
                    intIPs[3] = (int)(IPNumbers >> 24);
                    intIPs[2] = (int)((IPNumbers % 16777216) >> 16);
                    intIPs[1] = (int)((IPNumbers % 16777216 % 65536) >> 8);
                    intIPs[0] = (int)(IPNumbers % 16777216 % 65536 % 256);
                }
                else
                {
                    intIPs[0] = (int)(IPNumbers >> 24);
                    intIPs[1] = (int)((IPNumbers % 16777216) >> 16);
                    intIPs[2] = (int)((IPNumbers % 16777216 % 65536) >> 8);
                    intIPs[3] = (int)(IPNumbers % 16777216 % 65536 % 256);
                }


                if (IsFixCode)
                {
                    strReturn = intIPs[0].ToString("000") + "." + intIPs[1].ToString("000") + "." + intIPs[2].ToString("000") + "." + intIPs[3].ToString("000");
                }
                else
                {
                    strReturn = intIPs[0].ToString() + "." + intIPs[1].ToString() + "." + intIPs[2].ToString() + "." + intIPs[3].ToString();
                }
            }
            catch
            {
                strReturn = "0.0.0.0";
            }

            return strReturn;
        }
        #endregion
    }
}
