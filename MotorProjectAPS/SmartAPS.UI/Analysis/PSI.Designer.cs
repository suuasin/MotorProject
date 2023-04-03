namespace SmartAPS.UI.Analysis
{
    partial class PSI
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PSI));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.ResultPivot = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.editVersionNo = new DevExpress.XtraBars.BarEditItem();
            this.editDateTime = new DevExpress.XtraBars.BarEditItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.editDateSpin = new DevExpress.XtraBars.BarEditItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonSave = new DevExpress.XtraBars.BarButtonItem();
            this.buttonMore = new DevExpress.XtraBars.BarButtonItem();
            this.radioPlanDate = new DevExpress.XtraBars.BarEditItem();
            this.editProdId = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemCheckedComboBoxEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTimeSpanEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
            this.dateTimePicker1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.numericUpDownWeek = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.checkEditAccuMoreOrLess = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem4 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem5 = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup10 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup9 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repositoryItemTimeEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultPivot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckedComboBoxEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeSpanEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.AutoSize = true;
            this.panelControl1.Controls.Add(this.ResultPivot);
            this.panelControl1.Controls.Add(this.ribbonControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1143, 971);
            this.panelControl1.TabIndex = 0;
            // 
            // ResultPivot
            // 
            this.ResultPivot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultPivot.Location = new System.Drawing.Point(2, 128);
            this.ResultPivot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ResultPivot.Name = "ResultPivot";
            this.ResultPivot.OptionsView.ShowColumnGrandTotalHeader = false;
            this.ResultPivot.OptionsView.ShowColumnTotals = false;
            this.ResultPivot.OptionsView.ShowRowGrandTotalHeader = false;
            this.ResultPivot.OptionsView.ShowRowGrandTotals = false;
            this.ResultPivot.OptionsView.ShowRowTotals = false;
            this.ResultPivot.Size = new System.Drawing.Size(1139, 841);
            this.ResultPivot.TabIndex = 1;
            this.ResultPivot.CustomFieldSort += new DevExpress.XtraPivotGrid.PivotGridCustomFieldSortEventHandler(this.ResultPivot_CustomFieldSort);
            this.ResultPivot.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.ResultPivot_FieldValueDisplayText);
            this.ResultPivot.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.ResultPivot_CustomCellDisplayText);
            this.ResultPivot.CustomCellValue += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCellValueEventArgs>(this.ResultPivot_CustomCellValue);
            this.ResultPivot.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.ResultPivot_CellDoubleClick);
            this.ResultPivot.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.ResultPivot_CustomAppearance);
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.AutoSizeItems = true;
            this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editVersionNo,
            this.editDateTime,
            this.barStaticItem1,
            this.editDateSpin,
            this.barStaticItem2,
            this.buttonLoad,
            this.buttonSave,
            this.buttonMore,
            this.radioPlanDate,
            this.editProdId,
            this.barEditItem2,
            this.dateTimePicker1,
            this.numericUpDownWeek,
            this.checkEditAccuMoreOrLess,
            this.barStaticItem3,
            this.barStaticItem4,
            this.barStaticItem5});
            this.ribbonControl1.Location = new System.Drawing.Point(2, 2);
            this.ribbonControl1.MaxItemId = 38;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonSave);
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckedComboBoxEdit1,
            this.repositoryItemTimeEdit1,
            this.repositoryItemTimeSpanEdit1,
            this.repositoryItemDateEdit1,
            this.repositoryItemSpinEdit1,
            this.repositoryItemCheckEdit1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1139, 126);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // editVersionNo
            // 
            this.editVersionNo.Caption = "VERSION ";
            this.editVersionNo.Edit = null;
            this.editVersionNo.EditWidth = 200;
            this.editVersionNo.Id = 1;
            this.editVersionNo.Name = "editVersionNo";
            // 
            // editDateTime
            // 
            this.editDateTime.Caption = "DATE ";
            this.editDateTime.Edit = null;
            this.editDateTime.EditWidth = 180;
            this.editDateTime.Id = 6;
            this.editDateTime.Name = "editDateTime";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "~";
            this.barStaticItem1.Id = 7;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // editDateSpin
            // 
            this.editDateSpin.Edit = null;
            this.editDateSpin.Id = 8;
            this.editDateSpin.Name = "editDateSpin";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Caption = "Days";
            this.barStaticItem2.Id = 9;
            this.barStaticItem2.Name = "barStaticItem2";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonLoad.Caption = "LOAD";
            this.buttonLoad.Id = 13;
            this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
            this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
            // 
            // buttonSave
            // 
            this.buttonSave.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonSave.Caption = "EXCEL";
            this.buttonSave.Id = 14;
            this.buttonSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.ImageOptions.Image")));
            this.buttonSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonSave.ImageOptions.LargeImage")));
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonSave_ItemClick);
            // 
            // buttonMore
            // 
            this.buttonMore.Caption = "MORE";
            this.buttonMore.Id = 16;
            this.buttonMore.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonMore.ImageOptions.Image")));
            this.buttonMore.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonMore.ImageOptions.LargeImage")));
            this.buttonMore.Name = "buttonMore";
            this.buttonMore.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
            // 
            // radioPlanDate
            // 
            this.radioPlanDate.Edit = null;
            this.radioPlanDate.EditWidth = 70;
            this.radioPlanDate.Id = 22;
            this.radioPlanDate.Name = "radioPlanDate";
            // 
            // editProdId
            // 
            this.editProdId.Caption = "PRODUCT ";
            this.editProdId.Edit = this.repositoryItemCheckedComboBoxEdit1;
            this.editProdId.EditWidth = 200;
            this.editProdId.Id = 29;
            this.editProdId.Name = "editProdId";
            // 
            // repositoryItemCheckedComboBoxEdit1
            // 
            this.repositoryItemCheckedComboBoxEdit1.AutoHeight = false;
            this.repositoryItemCheckedComboBoxEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCheckedComboBoxEdit1.Name = "repositoryItemCheckedComboBoxEdit1";
            // 
            // barEditItem2
            // 
            this.barEditItem2.Caption = "barEditItem2";
            this.barEditItem2.Edit = this.repositoryItemTimeSpanEdit1;
            this.barEditItem2.Id = 31;
            this.barEditItem2.Name = "barEditItem2";
            // 
            // repositoryItemTimeSpanEdit1
            // 
            this.repositoryItemTimeSpanEdit1.AutoHeight = false;
            this.repositoryItemTimeSpanEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemTimeSpanEdit1.Name = "repositoryItemTimeSpanEdit1";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Caption = "DATE ";
            this.dateTimePicker1.Edit = this.repositoryItemDateEdit1;
            this.dateTimePicker1.EditWidth = 120;
            this.dateTimePicker1.Id = 32;
            this.dateTimePicker1.Name = "dateTimePicker1";
            // 
            // repositoryItemDateEdit1
            // 
            this.repositoryItemDateEdit1.AutoHeight = false;
            this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
            // 
            // numericUpDownWeek
            // 
            this.numericUpDownWeek.Caption = "~  ";
            this.numericUpDownWeek.Edit = this.repositoryItemSpinEdit1;
            this.numericUpDownWeek.EditWidth = 50;
            this.numericUpDownWeek.Id = 33;
            this.numericUpDownWeek.Name = "numericUpDownWeek";
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // checkEditAccuMoreOrLess
            // 
            this.checkEditAccuMoreOrLess.Caption = "MoreOrLessCarryForward";
            this.checkEditAccuMoreOrLess.Edit = this.repositoryItemCheckEdit1;
            this.checkEditAccuMoreOrLess.Id = 34;
            this.checkEditAccuMoreOrLess.Name = "checkEditAccuMoreOrLess";
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // barStaticItem3
            // 
            this.barStaticItem3.Caption = "WEEKS";
            this.barStaticItem3.Id = 35;
            this.barStaticItem3.Name = "barStaticItem3";
            // 
            // barStaticItem4
            // 
            this.barStaticItem4.Id = 36;
            this.barStaticItem4.Name = "barStaticItem4";
            // 
            // barStaticItem5
            // 
            this.barStaticItem5.Id = 37;
            this.barStaticItem5.Name = "barStaticItem5";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup10,
            this.ribbonPageGroup2,
            this.ribbonPageGroup9,
            this.ribbonPageGroup1});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup10
            // 
            this.ribbonPageGroup10.ItemLinks.Add(this.barStaticItem4, true);
            this.ribbonPageGroup10.ItemLinks.Add(this.editProdId);
            this.ribbonPageGroup10.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup10.Name = "ribbonPageGroup10";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barStaticItem5, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.dateTimePicker1);
            this.ribbonPageGroup2.ItemLinks.Add(this.numericUpDownWeek);
            this.ribbonPageGroup2.ItemLinks.Add(this.barStaticItem3);
            this.ribbonPageGroup2.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup9
            // 
            this.ribbonPageGroup9.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup9.ItemLinks.Add(this.buttonLoad);
            this.ribbonPageGroup9.ItemLinks.Add(this.buttonSave);
            this.ribbonPageGroup9.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup9.Name = "ribbonPageGroup9";
            this.ribbonPageGroup9.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup9.Text = "ribbonPageGroup9";
            this.ribbonPageGroup9.Visible = false;
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.checkEditAccuMoreOrLess, true);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            this.ribbonPageGroup1.Visible = false;
            // 
            // repositoryItemTimeEdit1
            // 
            this.repositoryItemTimeEdit1.AutoHeight = false;
            this.repositoryItemTimeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemTimeEdit1.Name = "repositoryItemTimeEdit1";
            // 
            // PSI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PSI";
            this.Size = new System.Drawing.Size(1143, 971);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultPivot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckedComboBoxEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeSpanEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeEdit1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraPivotGrid.PivotGridControl ResultPivot;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarEditItem editVersionNo;
        private DevExpress.XtraBars.BarEditItem editDateTime;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarEditItem editDateSpin;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.BarButtonItem buttonSave;
        private DevExpress.XtraBars.BarButtonItem buttonMore;
        private DevExpress.XtraBars.BarEditItem radioPlanDate;
        private DevExpress.XtraBars.BarEditItem editProdId;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit1;
        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup10;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup9;
        private DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit repositoryItemTimeEdit1;
        private DevExpress.XtraBars.BarEditItem dateTimePicker1;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit1;
        private DevExpress.XtraBars.BarEditItem numericUpDownWeek;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraBars.BarEditItem checkEditAccuMoreOrLess;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem3;
        private DevExpress.XtraBars.BarStaticItem barStaticItem4;
        private DevExpress.XtraBars.BarStaticItem barStaticItem5;
    }
}
