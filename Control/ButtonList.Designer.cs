namespace NetLibrary.CustomControl
{
    partial class ButtonList
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tlpButtonList = new System.Windows.Forms.TableLayoutPanel();
            this.ttpGlobal = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tlpButtonList
            // 
            this.tlpButtonList.ColumnCount = 1;
            this.tlpButtonList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpButtonList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtonList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtonList.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpButtonList.Location = new System.Drawing.Point(0, 0);
            this.tlpButtonList.Margin = new System.Windows.Forms.Padding(0);
            this.tlpButtonList.Name = "tlpButtonList";
            this.tlpButtonList.RowCount = 1;
            this.tlpButtonList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpButtonList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpButtonList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpButtonList.Size = new System.Drawing.Size(133, 52);
            this.tlpButtonList.TabIndex = 1;
            // 
            // ttpGlobal
            // 
            this.ttpGlobal.AutomaticDelay = 200;
            this.ttpGlobal.AutoPopDelay = 3000;
            this.ttpGlobal.InitialDelay = 100;
            this.ttpGlobal.ReshowDelay = 80;
            this.ttpGlobal.UseFading = false;
            // 
            // ButtonList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tlpButtonList);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(152, 52);
            this.Name = "ButtonList";
            this.Size = new System.Drawing.Size(133, 50);
            this.Load += new System.EventHandler(this.ButtonList_Load);
            this.SizeChanged += new System.EventHandler(this.ButtonList_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpButtonList;
        private System.Windows.Forms.ToolTip ttpGlobal;
    }
}
