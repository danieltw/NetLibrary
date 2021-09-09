using System.Windows.Forms;

namespace NetLibrary.Tools
{
    /// <summary>
    /// 剪貼簿應用
    /// </summary>
    public class ClipboardTools
    {
        /// <summary>
        /// 將資料複製到剪貼簿
        /// </summary>
        /// <param name="SourceGridView">來源資料檢視器</param>
        /// <param name="CopyAll">複製整行資料</param>
        public static void CopyToClipboard(DataGridView SourceGridView, bool CopyAll)
        {
            string sCopy = "";
            if (CopyAll)
            {
                if ((SourceGridView.SelectedCells == null) || (SourceGridView.SelectedCells.Count <= 0)) return;
                int cRowIndex = -1;

                foreach (DataGridViewCell sCell in SourceGridView.SelectedCells)
                {
                    string tCopy = "";
                    if (cRowIndex == sCell.RowIndex) continue;

                    DataGridViewRow sRow = SourceGridView.Rows[sCell.RowIndex];
                    foreach (DataGridViewCell tCell in sRow.Cells)
                    {
                        if (!tCell.Visible) continue;
                        if (tCell.Value == null)
                            tCopy += (tCopy == "" ? "" : "\t");
                        else
                            tCopy += (tCopy == "" ? "" : "\t") + string.Format("{0}", tCell.FormattedValue.ToString());
                    }
                    sCopy = tCopy + "\n" + sCopy;
                    cRowIndex = sCell.RowIndex;
                }
                sCopy = sCopy.Substring(0, sCopy.Length - 1);
            }
            else
            {
                if ((SourceGridView.SelectedCells == null) || (SourceGridView.SelectedCells.Count <= 0)) return;
                int cRowIndex = -1;
                foreach (DataGridViewCell tCell in SourceGridView.SelectedCells)
                {
                    if (cRowIndex == -1) cRowIndex = tCell.RowIndex;

                    if (cRowIndex != tCell.RowIndex)
                    {
                        sCopy = "\n" + sCopy;
                        cRowIndex = tCell.RowIndex;
                    }

                    if (tCell.Value == null)
                        sCopy = (sCopy == "" ? "" : "\t") + sCopy;
                    else
                        sCopy = string.Format("{0}", tCell.FormattedValue.ToString()) + (sCopy == "" ? "" : "\t") + sCopy;
                }
            }

            Clipboard.Clear();
            if ((sCopy == null) || (sCopy.Trim() == "")) return;
            Clipboard.SetText(sCopy.Trim());
        }
    }
}
