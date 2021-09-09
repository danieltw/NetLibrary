using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace NetLibrary.CustomControl
{
    /// <summary>
    /// 自定的按鍵清單控制項
    /// </summary>
    /// <remarks>建立系統共用的按鍵清單，控制項可以加入到工具箱中來快速引用。</remarks>
    [System.Drawing.ToolboxBitmap(typeof(ListView))]
    [ToolboxItem(true)]
    public partial class ButtonList : UserControl
    {
        #region 私有變數
        private int _ButtonWidth = 150;
        private int _ButtonHeight = 50;
        private int _CTLWidth;
        private int _CTLHeight;
        private int _TableHeight;
        private int _MaxColCount = 1;
        private int _MaxRowCount = 1;
        private Timer _tDelayedResize = new Timer();
        private int _MaxItemData = 100;
        private CustomButton curButton = null;
        #endregion

        #region 公用屬性
        /// <summary>
        /// 按鍵清單內容
        /// </summary>
        [ToolboxItem(typeof(ButtonListContent))]
        [DisplayName("ButtonItems")]
        [DefaultValue(null)]
        [Description("按鍵清單內容")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("自定按鍵")]
        public List<ButtonListContent> ItemData { get; set; }

        /// <summary>
        /// 清單最大項目數量 (系統最大為1000、最小為1、預設值為100)
        /// </summary>
        [Description("清單最大項目數量 (系統最大為1000、最小為1、預設值為100)")]
        [Category("自定按鍵")]
        public int MaxItemData
        {
            get { return _MaxItemData; }
            set
            {
                if (value <= 0)
                    _MaxItemData = 1;
                else if (value > 1000)
                    _MaxItemData = 1000;
                else
                    _MaxItemData = value;
            }
        }

        /// <summary>
        /// 按鍵大小
        /// </summary>
        [Description("按鍵的大小")]
        [Category("自定按鍵")]
        public Size ButtonSize
        {
            get { return new Size(_ButtonWidth, _ButtonHeight); }
            set { _ButtonHeight = value.Height; _ButtonWidth = value.Width; }
        }

        /// <summary>
        /// 按鍵間距
        /// </summary>
        [ToolboxItem(typeof(Padding))]
        [Description("按鍵之間的間距")]
        [Category("自定按鍵")]
        public Padding ButtonMargin { get; set; }

        /// <summary>
        /// 選取按鍵的背景顏色
        /// </summary>
        [ToolboxItem(typeof(Color))]
        [Description("選取按鍵的背景顏色")]
        [Category("自定按鍵")]
        [DefaultValue(typeof(Color), "Control")]
        public Color SelectedBackColor { get; set; }

        /// <summary>
        /// 多選
        /// </summary>
        [ToolboxItem(typeof(Boolean))]
        [Description("多選")]
        [Category("自定按鍵")]
        public Boolean MultiSelect { get; set; }
        #endregion

        #region 自定事件
        #region 滑鼠點選按鍵
        /// <summary>
        /// 宣告委派滑鼠點選按鍵的事件
        /// </summary>
        /// <remarks>委派滑鼠在Button控制項上的點選事件</remarks>
        public delegate void Button_OnClick(object sender, EventArgs e);
        /// <summary>
        /// 宣告按下按鍵的事件
        /// </summary>
        /// <remarks>滑鼠點選按鍵產生的 Click 事件。</remarks>
        [Description("當按下一按鍵時發生")]
        [Category("Mouse")]
        public event Button_OnClick ButtonClick;
        /// <summary>
        /// 點選按鍵
        /// </summary>
        private void Button_Click(object sender, EventArgs e)
        {
            if (MultiSelect)
            {
                curButton = (CustomButton)sender;
                curButton.Selected = !curButton.Selected;
                foreach (ButtonListContent tmpButtonListContent in ItemData.Where(x => x.ReturnValue == curButton.ReturnValue))
                {
                    tmpButtonListContent.Selected = curButton.Selected;
                }

                if (curButton.Selected)
                    curButton.BackColor = SelectedBackColor;
                else
                    curButton.BackColor = Color.FromName("Control");
            }
            else
            {
                foreach (ButtonListContent tmpButtonListContent in ItemData.Where(x => x.Selected))
                {
                    tmpButtonListContent.Selected = false;
                }

                foreach (Control tmpControl in tlpButtonList.Controls)
                {
                    if (tmpControl.BackColor != Color.FromName("Control")) tmpControl.BackColor = Color.FromName("Control");
                }


                CustomButton tmpButton = (CustomButton)sender;
                tmpButton.Selected = true;
                tmpButton.BackColor = SelectedBackColor;

                foreach (ButtonListContent tmpButtonListContent in ItemData.Where(x => x.ReturnValue == tmpButton.ReturnValue))
                {
                    tmpButtonListContent.Selected = tmpButton.Selected;
                }
                curButton = tmpButton;
            }
            if (ButtonClick != null) ButtonClick(sender, e);
        }
        #endregion

        #region 滑鼠移入按鍵
        /// <summary>
        /// 宣告委派滑鼠移入按鍵的事件
        /// </summary>
        public delegate void Button_OnMouseHover(object sender, EventArgs e);
        /// <summary>
        /// 宣告滑鼠移入按鍵的事件
        /// </summary>
        [Description("當滑鼠移入按鍵時發生")]
        [Category("Mouse")]
        public event Button_OnMouseHover ButtonMouseHover;
        /// <summary>
        /// 滑鼠移入按鍵
        /// </summary>
        private void Button_MouseHover(object sender, EventArgs e)
        {
            CustomButton tmpBTN = (CustomButton)sender;
            ttpGlobal.Show(tmpBTN.ToolTipText, tmpBTN);
            if (ButtonMouseHover != null) ButtonMouseHover(sender, e);
        }
        #endregion

        #region 滑鼠移出按鍵
        /// <summary>
        /// 宣告委派滑鼠移出按鍵的事件
        /// </summary>
        public delegate void Button_OnMouseLeave(object sender, EventArgs e);
        /// <summary>
        /// 宣告滑鼠移出按鍵的事件
        /// </summary>
        [Description("當滑鼠移出按鍵時發生")]
        [Category("Mouse")]
        public event Button_OnMouseLeave ButtonMouseLeave;
        /// <summary>
        /// 滑鼠移出按鍵
        /// </summary>
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            CustomButton tmpBTN = (CustomButton)sender;
            ttpGlobal.Hide(tmpBTN);
            if (ButtonMouseLeave != null) ButtonMouseLeave(sender, e);
        }
        #endregion
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化控制項
        /// </summary>
        public ButtonList()
        {
            InitializeComponent();
            _CTLHeight = this.Height;
            _CTLWidth = this.Width;
            tlpButtonList.Width = _CTLWidth;
            tlpButtonList.Height = _CTLHeight;
            _TableHeight = tlpButtonList.Height;
            _tDelayedResize = new Timer { Interval = 300 };
            _tDelayedResize.Tick += this_ResizeEnd;

            ItemData = new List<ButtonListContent>();
            ButtonMargin = new Padding(0);
            MultiSelect = false;
            SelectedBackColor = Color.FromArgb(255, 224, 192);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 結束變更大小
        /// </summary>
        private void this_ResizeEnd(object sender, EventArgs e)
        {
            _tDelayedResize.Stop();
            ReLayoutTable();
        }

        /// <summary>
        /// 載入控制項
        /// </summary>
        private void ButtonList_Load(object sender, EventArgs e)
        {
            ReLayoutTable();
        }

        /// <summary>
        /// 變更大小
        /// </summary>
        private void ButtonList_SizeChanged(object sender, EventArgs e)
        {
            _CTLHeight = this.Height;
            _CTLWidth = this.Width;
            _TableHeight = tlpButtonList.Height;

            _tDelayedResize.Stop();
            _tDelayedResize.Start();
        }

        /// <summary>
        /// 建立按鍵
        /// </summary>
        private void CreateButton()
        {
            int tmpColIndex = 0;
            int tmpRowIndex = 0;

            tlpButtonList.Controls.Clear();
            int _ct = 0;
            foreach (ButtonListContent tmpCT in ItemData)
            {

                CustomButton btnTemp = new CustomButton
                {
                    Dock = DockStyle.Fill,
                    Margin = ButtonMargin,
                    Text = tmpCT.DisplayText,
                    ReturnValue = tmpCT.ReturnValue,
                    ReturnObject = tmpCT.ReturnObject,
                    Selected = tmpCT.Selected,
                    Enabled = !tmpCT.Disabled,
                    ToolTipText = tmpCT.ToolTipText
                };

                if (btnTemp.Selected)
                    btnTemp.BackColor = SelectedBackColor;
                else
                    btnTemp.BackColor = Color.FromName("Control");

                btnTemp.Click += new System.EventHandler(this.Button_Click);
                btnTemp.MouseHover += new System.EventHandler(this.Button_MouseHover);
                btnTemp.MouseLeave += new System.EventHandler(this.Button_MouseLeave);

                tlpButtonList.Controls.Add(btnTemp);

                tlpButtonList.SetColumn(btnTemp, tmpColIndex);
                tlpButtonList.SetRow(btnTemp, tmpRowIndex);

                tmpColIndex++;
                if (tmpColIndex == _MaxColCount)
                {
                    tmpColIndex = 0;
                    tmpRowIndex++;
                }

                _ct++;
                if (_ct >= MaxItemData) break;
            }
        }
        #endregion

        #region 自定方法
        /// <summary>
        /// 按鍵清單重新排版
        /// </summary>
        public void ReLayoutTable()
        {
            if ((ItemData == null) || (ItemData.Count == 0))
            {
                tlpButtonList.RowCount = 0;
                tlpButtonList.ColumnCount = 0;
                tlpButtonList.Controls.Clear();
                return;
            }

            tlpButtonList.SuspendLayout();
            bool IsOnlyColOne = false;
            double ScrollWidth = 18.0;

            if (_TableHeight <= _CTLHeight)
                ScrollWidth = 0.0;
            else
                ScrollWidth = 18.0;

            if (ScrollWidth == 0)
            {
                if ((_CTLWidth / (_ButtonWidth + ButtonMargin.Left)) > 1 && (_CTLWidth / (_ButtonWidth + ButtonMargin.Left)) < 2) IsOnlyColOne = true;
            }
            else
            {
                if (((_CTLWidth - ScrollWidth) / (_ButtonWidth + ButtonMargin.Horizontal)) > 1 && ((_CTLWidth - ScrollWidth) / (_ButtonWidth + ButtonMargin.Horizontal)) < 2) IsOnlyColOne = true;
            }

            _MaxColCount = (_CTLWidth - (int)ScrollWidth) / (_ButtonWidth + ButtonMargin.Horizontal);
            if (_MaxColCount < 1) _MaxColCount = 1;
            _MaxRowCount = 1;

            if (ItemData.Count > _MaxColCount)
            {
                _MaxRowCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ItemData.Count) / _MaxColCount));
            }

            tlpButtonList.RowCount = _MaxRowCount;
            tlpButtonList.ColumnCount = _MaxColCount;

            tlpButtonList.Height = _MaxRowCount * (_ButtonHeight + ButtonMargin.Vertical);

            tlpButtonList.RowStyles.Clear();
            if (_MaxRowCount > 1)
            {
                tlpButtonList.RowCount = _MaxRowCount;
                for (int i = 0; i < _MaxRowCount; i++)
                {
                    RowStyle tmpRowS = new RowStyle
                    {
                        SizeType = SizeType.Absolute,
                        Height = _ButtonHeight + ButtonMargin.Vertical
                    };
                    tlpButtonList.RowStyles.Add(tmpRowS);
                }
            }
            else if (_MaxRowCount == 1)
            {
                tlpButtonList.RowCount = _MaxRowCount;
                RowStyle tmpRowS = new RowStyle
                {
                    SizeType = SizeType.Absolute,
                    Height = _ButtonHeight
                };
                tlpButtonList.RowStyles.Add(tmpRowS);
            }

            tlpButtonList.ColumnStyles.Clear();
            if ((_MaxColCount >= 1) || (IsOnlyColOne))
            {
                tlpButtonList.ColumnCount = _MaxColCount + 1;
                for (int i = 0; i < _MaxColCount; i++)
                {
                    ColumnStyle tmpColS = new ColumnStyle
                    {
                        SizeType = SizeType.Absolute,
                        Width = _ButtonWidth + ButtonMargin.Horizontal
                    };
                    tlpButtonList.ColumnStyles.Add(tmpColS);
                }
            }

            ColumnStyle tmpColE = new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 100
            };
            tlpButtonList.ColumnStyles.Add(tmpColE);

            CreateButton();
            tlpButtonList.ResumeLayout();
        }

        /// <summary>
        /// 取回已被選取的按鍵
        /// </summary>
        public List<ButtonListContent> SelectedButton()
        {
            try
            {
                if (!MultiSelect) return new List<ButtonListContent>();
                return ItemData.Where(x => x.Selected).ToList();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 自定按鍵
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(Button))]
    [Description("自定義按鍵，新增字串屬性 ReturnValue")]
    public class CustomButton : Button
    {
        #region 公用屬性
        /// <summary>
        /// 回傳值
        /// </summary>
        [DefaultValue("")]
        [ToolboxItem(typeof(RichTextBox))]
        [Description("點選按鍵的回傳值")]
        [Category("自定")]
        public string ReturnValue { get; set; }

        /// <summary>
        /// 回傳物件
        /// </summary>
        [DefaultValue(null)]
        [ToolboxItem(typeof(object))]
        [Description("點選按鍵的回傳物件")]
        [Category("自定")]
        public object ReturnObject { get; set; }

        /// <summary>
        /// 選取狀態
        /// </summary>
        [DefaultValue(false)]
        [ToolboxItem(typeof(bool))]
        [Description("按鍵是否已選取")]
        [Category("自定")]
        public bool Selected { get; set; }

        /// <summary>
        /// 提示文字
        /// </summary>
        [DefaultValue("")]
        [ToolboxItem(typeof(string))]
        [Description("滑鼠移入後顯示的提示文字")]
        [Category("自定")]
        public string ToolTipText { get; set; }
        #endregion
    }

    /// <summary>
    /// 按鍵內容類別
    /// </summary>
    [Serializable]
    [DisplayName("CustomButton")]
    [Description("按鍵內容類別，定義顯示文字、回傳資料與標記資料")]
    public class ButtonListContent
    {
        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        public ButtonListContent()
        {
            DisplayText = "";
            ReturnValue = "";
            ReturnObject = "";
            Selected = false;
            ToolTipText = "";
        }

        /// <summary>
        /// 初始化並設定顯示文字與回傳資料
        /// </summary>
        /// <param name="displayText">顯示文字</param>
        /// <param name="returnValue">回傳資料</param>
        public ButtonListContent(string displayText, string returnValue)
            : this()
        {
            DisplayText = displayText;
            ReturnValue = returnValue;
            ToolTipText = "";
        }

        /// <summary>
        /// 初始化並設定顯示文字與回傳資料
        /// </summary>
        /// <param name="displayText">顯示文字</param>
        /// <param name="returnValue">回傳資料</param>
        /// <param name="IsSelected">選取狀態</param>
        public ButtonListContent(string displayText, string returnValue, bool IsSelected)
            : this(displayText, returnValue)
        {
            Selected = IsSelected;
            ToolTipText = "";
        }

        /// <summary>
        /// 初始化並設定顯示文字、回傳資料與回傳物件
        /// </summary>
        /// <param name="displayText">顯示文字</param>
        /// <param name="returnValue">回傳資料</param>
        /// <param name="returnObject">回傳物件</param>
        public ButtonListContent(string displayText, string returnValue, object returnObject)
            : this(displayText, returnValue)
        {
            ReturnObject = returnObject;
            ToolTipText = "";
        }

        /// <summary>
        /// 初始化並設定顯示文字、回傳資料、回傳物件與選取狀態
        /// </summary>
        /// <param name="displayText">顯示文字</param>
        /// <param name="returnValue">回傳資料</param>
        /// <param name="returnObject">回傳物件</param>
        /// <param name="IsSelected">選取狀態</param>
        public ButtonListContent(string displayText, string returnValue, object returnObject, bool IsSelected)
            : this(displayText, returnValue, returnObject)
        {
            Selected = IsSelected;
            ToolTipText = "";
        }
        #endregion

        #region 公用屬性
        /// <summary>
        /// 顯示文字
        /// </summary>
        [DefaultValue("")]
        [ToolboxItem(typeof(RichTextBox))]
        [Description("按鍵顯示文字")]
        [Category("Appearance")]
        public string DisplayText { get; set; }

        /// <summary>
        /// 回傳值
        /// </summary>
        [DefaultValue("")]
        [ToolboxItem(typeof(RichTextBox))]
        [Description("點選按鍵的回傳值")]
        [Category("Appearance")]
        public string ReturnValue { get; set; }

        /// <summary>
        /// 回傳物件
        /// </summary>
        [DefaultValue(null)]
        [ToolboxItem(typeof(object))]
        [Description("回傳物件")]
        [Category("Data")]
        public object ReturnObject { get; set; }

        /// <summary>
        /// 選取狀態
        /// </summary>
        [DefaultValue(false)]
        [Description("按鍵是否已選取")]
        [Category("Behavior")]
        public bool Selected { get; set; }

        /// <summary>
        /// 啟用狀態
        /// </summary>
        [DefaultValue(false)]
        [ToolboxItem(typeof(bool))]
        [Description("按鍵是否已停用")]
        [Category("自定")]
        [Browsable(false)]
        public bool Disabled { get; set; }

        /// <summary>
        /// 提示文字
        /// </summary>
        [DefaultValue("")]
        [ToolboxItem(typeof(string))]
        [Description("滑鼠移入後顯示的提示文字")]
        [Category("Appearance")]
        public string ToolTipText { get; set; }
        #endregion
    }


}
