using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace NetLibrary.Tools
{
    /// <summary>
    /// 視窗程式應用
    /// </summary>
    public class WinFormTools
    {
        #region 文字方塊輸入資料判斷
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
        #endregion

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
