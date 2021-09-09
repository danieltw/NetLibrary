using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;

namespace NetLibrary.ReportViewer
{
    /// <summary>
    /// 報表列印
    /// </summary>
    public class PrintReportDocument : PrintDocument
    {
        private PageSettings m_pageSettings;
        private int m_currentPage;
        private List<Stream> m_pages = new List<Stream>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverReport">伺服器報表</param>
        public PrintReportDocument(ServerReport serverReport) : this((Report)serverReport)
        {
            RenderAllServerReportPages(serverReport);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="localReport">本地端報表</param>
        public PrintReportDocument(LocalReport localReport) : this((Report)localReport)
        {
            RenderAllLocalReportPages(localReport);
        }

        private PrintReportDocument(Report report)
        {
            ReportPageSettings reportPageSettings = report.GetDefaultPageSettings();
            m_pageSettings = new PageSettings();
            m_pageSettings.PaperSize = reportPageSettings.PaperSize;
            m_pageSettings.Margins = reportPageSettings.Margins;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (Stream s in m_pages)
                {
                    s.Dispose();
                }
                m_pages.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            m_currentPage = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            Stream pageToPrint = m_pages[m_currentPage];
            pageToPrint.Position = 0;
            using (Metafile pageMetaFile = new Metafile(pageToPrint))
            {
                Rectangle adjustedRect = new Rectangle(e.PageBounds.Left - (int)e.PageSettings.HardMarginX, e.PageBounds.Top - (int)e.PageSettings.HardMarginY, e.PageBounds.Width, e.PageBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, adjustedRect);
                e.Graphics.DrawImage(pageMetaFile, adjustedRect);
                m_currentPage++;
                e.HasMorePages = m_currentPage < m_pages.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            e.PageSettings = (PageSettings)m_pageSettings.Clone();
        }

        private void RenderAllServerReportPages(ServerReport serverReport)
        {
            string deviceInfo = CreateEMFDeviceInfo();
            NameValueCollection firstPageParameters = new NameValueCollection();
            firstPageParameters.Add("rs:PersistStreams", "True");
            NameValueCollection nonFirstPageParameters = new NameValueCollection();
            nonFirstPageParameters.Add("rs:GetNextStream", "True");

            string mimeType;
            string fileExtension;
            Stream pageStream = serverReport.Render("IMAGE", deviceInfo, firstPageParameters, out mimeType, out fileExtension);

            while (pageStream.Length > 0)
            {
                m_pages.Add(pageStream);
                pageStream = serverReport.Render("IMAGE", deviceInfo, nonFirstPageParameters, out mimeType, out fileExtension);
            }
        }

        private void RenderAllLocalReportPages(LocalReport localReport)
        {
            string deviceInfo = CreateEMFDeviceInfo();

            Warning[] warnings;
            localReport.Render("IMAGE", deviceInfo, LocalReportCreateStreamCallback, out warnings);
        }

        private Stream LocalReportCreateStreamCallback(string name, string extension, Encoding encoding, string mimeType, bool willSeek)
        {
            MemoryStream stream = new MemoryStream();
            m_pages.Add(stream);
            return stream;
        }

        private string CreateEMFDeviceInfo()
        {
            PaperSize paperSize = m_pageSettings.PaperSize;
            Margins margins = m_pageSettings.Margins;

            return string.Format(CultureInfo.InvariantCulture,
                "<DeviceInfo><OutputFormat>emf</OutputFormat><StartPage>0</StartPage><EndPage>0</EndPage><MarginTop>{0}</MarginTop><MarginLeft>{1}</MarginLeft><MarginRight>{2}</MarginRight><MarginBottom>{3}</MarginBottom><PageHeight>{4}</PageHeight><PageWidth>{5}</PageWidth></DeviceInfo>",
                ToInches(margins.Top), ToInches(margins.Left), ToInches(margins.Right), ToInches(margins.Bottom), ToInches(paperSize.Height), ToInches(paperSize.Width));
        }

        private static string ToInches(int hundrethsOfInch)
        {
            double inches = hundrethsOfInch / 100.0;
            return inches.ToString(CultureInfo.InvariantCulture) + "in";
        }
    }
}
