using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace NetLibrary.Tools
{
    /// <summary>
    /// 視窗程式應用
    /// </summary>
    public class WindowsForm
    {
        /// <summary>
        /// TextBox 工具
        /// </summary>
        public class TextBoxKeyPress
        {
            /// <summary>
            /// 判斷輸入的字元是否為數值(含小數點)
            /// </summary>
            /// <param name="SourceTextBox">來源 TextBox 控制項</param>
            /// <param name="e">KeyPress 事件參數值</param>
            /// <param name="DecimalPlaces">小數位數，整數時請設為 0</param>
            /// <param name="AllowMinus">是否允許負值</param>
            public static void OnlyNumeric(TextBox SourceTextBox, KeyPressEventArgs e, int DecimalPlaces, bool AllowMinus)
            {
                if (((e.KeyChar > 47) && (e.KeyChar < 58)) || (e.KeyChar == 8) || (e.KeyChar == 45) || (e.KeyChar == 46))
                {
                    if ((!AllowMinus) && (e.KeyChar == 45))
                    {
                        e.Handled = true;
                        return;
                    }
                    else if (AllowMinus && ((e.KeyChar == 45) && (SourceTextBox.Text.StartsWith("-"))))
                    {
                        e.Handled = true;
                        return;
                    }

                    if (DecimalPlaces > 0)
                    {
                        if ((SourceTextBox.Text.Trim() == "") || (SourceTextBox.Text.Contains('.')))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            if (SourceTextBox.Text.Split('.')[1].Length > (DecimalPlaces - 1))
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                    else
                    {
                        if (e.KeyChar == 46)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            /// <summary>
            /// 判斷輸入的字元只允許輸入數字
            /// </summary>
            public static void OnlyNumeric(KeyPressEventArgs e)
            {
                if (((e.KeyChar > 47) && (e.KeyChar < 58)) || (e.KeyChar == 8))
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            /// <summary>
            /// TextBox 只允許輸入英文字母
            /// </summary>
            /// <param name="AllowSpace">是否允許輸入空白</param>
            public static void OnlyCharacter(KeyPressEventArgs e, bool AllowSpace)
            {
                if (((e.KeyChar > 64) && (e.KeyChar < 91)) ||
                    ((e.KeyChar > 96) && (e.KeyChar < 123)) ||
                    (e.KeyChar == 8) ||
                    ((e.KeyChar == 32) & AllowSpace) ||
                    (e.KeyChar > 127))
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            /// <summary>
            /// 判斷輸入的字元只允許輸入數字和英文字母
            /// </summary>
            /// <param name="AllowSpace">是否允許輸入空白</param>
            public static void OnlyNumericCharacter(KeyPressEventArgs e, bool AllowSpace)
            {
                if (((e.KeyChar > 47) && (e.KeyChar < 58)) ||
                    ((e.KeyChar > 64) && (e.KeyChar < 91)) ||
                    ((e.KeyChar > 96) && (e.KeyChar < 123)) ||
                    (e.KeyChar == 8) ||
                    ((e.KeyChar == 32) && AllowSpace))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
        }

        /// <summary>
        /// 剪貼簿應用
        /// </summary>
        public class DataGridViewTools
        {
            /// <summary>
            /// 將 DataGridView 資料複製到剪貼簿
            /// </summary>
            /// <param name="SourceGridView">來源資料檢視器</param>
            /// <param name="CopyAll">複製整行資料</param>
            public static void CopyToClipboard(DataGridView SourceGridView, bool CopyAll)
            {
                try
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 調整 DataGridView 來源資料集的欄位對應與欄位順序
            /// </summary>
            /// <param name="dgvTargetView">目地的 DataGridView 控制項</param>
            /// <param name="dtSourceTable">來源資料集</param>
            public static void DataTableReMapDataGridView(DataGridView dgvTargetView, DataTable dtSourceTable)
            {
                try
                {
                    List<string> KeepColumn = dgvTargetView.Columns.Cast<DataGridViewColumn>().ToList().Where(x => x.DataPropertyName != "").Select(x => x.DataPropertyName).ToList();
                    List<string> SourceColumnName = dtSourceTable.Columns.Cast<DataColumn>().ToList().Select(x => x.ColumnName).ToList();

                    foreach (string tColumnName in SourceColumnName)
                    {
                        if (KeepColumn.Contains(tColumnName)) continue;
                        dtSourceTable.Columns.Remove(tColumnName);
                    }

                    int SN = 0;
                    for (int i = 0; i < dgvTargetView.Columns.Count; i++)
                    {
                        if ((!dgvTargetView.Columns[i].Visible) ||
                           ((dgvTargetView.Columns[i].DataPropertyName == null) || (dgvTargetView.Columns[i].DataPropertyName == "")) ||
                           (!dtSourceTable.Columns.Contains(dgvTargetView.Columns[i].DataPropertyName))) continue;

                        dtSourceTable.Columns[dgvTargetView.Columns[i].DataPropertyName].SetOrdinal(SN);
                        SN++;
                    }
                    dtSourceTable.AcceptChanges();
                }
                catch { }
            }
        }

    }
}
