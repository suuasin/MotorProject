using DevExpress.Utils.Extensions;

namespace SmartAPS.UI.Analysis
{
	partial class DemandvsPlan
	{
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemandvsPlan));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pivotGridControl1 = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.editProdId = new DevExpress.XtraBars.BarEditItem();
            this.repoProdId = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.editStartDate = new DevExpress.XtraBars.BarEditItem();
            this.repoStartDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.editEndDate = new DevExpress.XtraBars.BarEditItem();
            this.repoEndDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.checkShowCumulative = new DevExpress.XtraBars.BarCheckItem();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup6 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup7 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pivotGridControl1);
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1101, 595);
            this.panelControl1.TabIndex = 12;
            // 
            // pivotGridControl1
            // 
            this.pivotGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pivotGridControl1.Location = new System.Drawing.Point(2, 104);
            this.pivotGridControl1.Name = "pivotGridControl1";
            this.pivotGridControl1.Size = new System.Drawing.Size(1097, 489);
            this.pivotGridControl1.TabIndex = 13;
            this.pivotGridControl1.ToolTipController = this.toolTipController1;
            this.pivotGridControl1.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.PivotGridControl1_CustomCellDisplayText);
            this.pivotGridControl1.CellClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.PivotGridControl1_CellClick);
            this.pivotGridControl1.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.PivotGridControl1_CustomAppearance);
            // 
            // toolTipController1
            // 
            this.toolTipController1.InitialDelay = 1;
            this.toolTipController1.ReshowDelay = 1;
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.ToolTipController1_GetActiveObjectInfo);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.ribbonControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(2, 2);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1097, 102);
            this.panelControl2.TabIndex = 14;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.AutoSizeItems = true;
            this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbonControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editProdId,
            this.editStartDate,
            this.editEndDate,
            this.checkShowCumulative,
            this.buttonLoad,
            this.buttonExcel});
            this.ribbonControl1.Location = new System.Drawing.Point(2, 2);
            this.ribbonControl1.MaxItemId = 10;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1,
            this.repoProdId,
            this.repoStartDate,
            this.repoEndDate,
            this.repositoryItemCheckEdit1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1093, 98);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // editProdId
            // 
            this.editProdId.Caption = "PRODUCT ";
            this.editProdId.Edit = this.repoProdId;
            this.editProdId.EditWidth = 120;
            this.editProdId.Id = 2;
            this.editProdId.Name = "editProdId";
            // 
            // repoProdId
            // 
            this.repoProdId.AutoHeight = false;
            this.repoProdId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoProdId.Name = "repoProdId";
            // 
            // editStartDate
            // 
            this.editStartDate.Caption = "DATE";
            this.editStartDate.Edit = this.repoStartDate;
            this.editStartDate.EditWidth = 120;
            this.editStartDate.Id = 3;
            this.editStartDate.Name = "editStartDate";
            // 
            // repoStartDate
            // 
            this.repoStartDate.AutoHeight = false;
            this.repoStartDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoStartDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoStartDate.Name = "repoStartDate";
            // 
            // editEndDate
            // 
            this.editEndDate.Caption = "~  ";
            this.editEndDate.Edit = this.repoEndDate;
            this.editEndDate.EditWidth = 120;
            this.editEndDate.Id = 4;
            this.editEndDate.Name = "editEndDate";
            // 
            // repoEndDate
            // 
            this.repoEndDate.AutoHeight = false;
            this.repoEndDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoEndDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoEndDate.Name = "repoEndDate";
            // 
            // checkShowCumulative
            // 
            this.checkShowCumulative.BindableChecked = true;
            this.checkShowCumulative.Caption = "Show Cumulative Values";
            this.checkShowCumulative.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.checkShowCumulative.Checked = true;
            this.checkShowCumulative.Id = 6;
            this.checkShowCumulative.Name = "checkShowCumulative";
            this.checkShowCumulative.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.checkShowCumulative_CheckedChanged);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Caption = "LOAD";
            this.buttonLoad.Id = 7;
            this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
            this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
            // 
            // buttonExcel
            // 
            this.buttonExcel.Caption = "EXCEL";
            this.buttonExcel.Id = 8;
            this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
            this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
            this.buttonExcel.Name = "buttonExcel";
            this.buttonExcel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonExcel_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5,
            this.ribbonPageGroup6,
            this.ribbonPageGroup7});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.editProdId);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup3.ImageOptions.Image")));
            this.ribbonPageGroup3.ItemLinks.Add(this.editStartDate);
            this.ribbonPageGroup3.ItemLinks.Add(this.editEndDate);
            this.ribbonPageGroup3.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "DATE";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // ribbonPageGroup5
            // 
            this.ribbonPageGroup5.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup5.ImageOptions.Image")));
            this.ribbonPageGroup5.ItemLinks.Add(this.checkShowCumulative);
            this.ribbonPageGroup5.Name = "ribbonPageGroup5";
            this.ribbonPageGroup5.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup5.Text = "CONDITION";
            // 
            // ribbonPageGroup6
            // 
            this.ribbonPageGroup6.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup6.ItemLinks.Add(this.buttonLoad);
            this.ribbonPageGroup6.ItemLinks.Add(this.buttonExcel);
            this.ribbonPageGroup6.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup6.Name = "ribbonPageGroup6";
            this.ribbonPageGroup6.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup6.Text = "ribbonPageGroup6";
            this.ribbonPageGroup6.Visible = false;
            // 
            // ribbonPageGroup7
            // 
            this.ribbonPageGroup7.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup7.Name = "ribbonPageGroup7";
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // DemandvsPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "DemandvsPlan";
            this.Size = new System.Drawing.Size(1101, 595);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl1;
        private DevExpress.Utils.ToolTipController toolTipController1;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
		private DevExpress.XtraBars.BarEditItem editProdId;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoProdId;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private DevExpress.XtraBars.BarEditItem editStartDate;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoStartDate;
		private DevExpress.XtraBars.BarEditItem editEndDate;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoEndDate;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
		private DevExpress.XtraBars.BarCheckItem checkShowCumulative;
		private DevExpress.XtraBars.BarButtonItem buttonLoad;
		private DevExpress.XtraBars.BarButtonItem buttonExcel;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup6;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup7;
        private DevExpress.XtraEditors.PanelControl panelControl2;
    }
}
