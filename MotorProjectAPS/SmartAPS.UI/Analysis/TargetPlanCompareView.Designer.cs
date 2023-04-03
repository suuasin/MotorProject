namespace SmartAPS.MozartStudio.Analysis
{
	partial class TargetPlanCompareView
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
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetPlanCompareView));
			DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
			DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
			DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.pivotGridControl1 = new DevExpress.XtraPivotGrid.PivotGridControl();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.editAreaId = new DevExpress.XtraBars.BarEditItem();
			this.repoAreaId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editShopId = new DevExpress.XtraBars.BarEditItem();
			this.repoShopId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editStartDate = new DevExpress.XtraBars.BarEditItem();
			this.repoStartDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
			this.editEndDate = new DevExpress.XtraBars.BarEditItem();
			this.repoEndDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
			this.checkAccQty = new DevExpress.XtraBars.BarCheckItem();
			this.checkOnlyMain = new DevExpress.XtraBars.BarCheckItem();
			this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
			this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup7 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup8 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup9 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup6 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.repoVersionNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repoStepId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repoProdType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repoProdId = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
			this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
			this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
			this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
			this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
			this.dockPanel1.SuspendLayout();
			this.dockPanel1_Container.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
			this.SuspendLayout();
			// 
			// panelControl1
			// 
			this.panelControl1.Controls.Add(this.pivotGridControl1);
			this.panelControl1.Controls.Add(this.ribbonControl1);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(0, 0);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(1225, 602);
			this.panelControl1.TabIndex = 4;
			// 
			// pivotGridControl1
			// 
			this.pivotGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pivotGridControl1.Location = new System.Drawing.Point(2, 103);
			this.pivotGridControl1.Name = "pivotGridControl1";
			this.pivotGridControl1.Size = new System.Drawing.Size(1221, 497);
			this.pivotGridControl1.TabIndex = 3;
			// 
			// ribbonControl1
			// 
			this.ribbonControl1.AutoSizeItems = true;
			this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
			this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editAreaId,
            this.editShopId,
            this.editStartDate,
            this.editEndDate,
            this.checkAccQty,
            this.checkOnlyMain,
            this.buttonLoad,
            this.buttonExcel});
			this.ribbonControl1.Location = new System.Drawing.Point(2, 2);
			this.ribbonControl1.MaxItemId = 16;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoVersionNo,
            this.repoAreaId,
            this.repoShopId,
            this.repoStepId,
            this.repoProdType,
            this.repoProdId,
            this.repoStartDate,
            this.repoEndDate,
            this.repositoryItemCheckEdit1});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(1221, 101);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// editAreaId
			// 
			this.editAreaId.Caption = "AREA ";
			this.editAreaId.Edit = this.repoAreaId;
			this.editAreaId.EditWidth = 120;
			this.editAreaId.Id = 3;
			this.editAreaId.Name = "editAreaId";
			this.editAreaId.EditValueChanged += new System.EventHandler(this.editAreaId_EditValueChanged);
			// 
			// repoAreaId
			// 
			this.repoAreaId.AutoHeight = false;
			this.repoAreaId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoAreaId.Name = "repoAreaId";
			// 
			// editShopId
			// 
			this.editShopId.Caption = "SHOP ";
			this.editShopId.Edit = this.repoShopId;
			this.editShopId.EditWidth = 120;
			this.editShopId.Id = 4;
			this.editShopId.Name = "editShopId";
			this.editShopId.EditValueChanged += new System.EventHandler(this.shopIdComboBox_SelectedIndexChanged);
			// 
			// repoShopId
			// 
			this.repoShopId.AutoHeight = false;
			this.repoShopId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoShopId.Name = "repoShopId";
			// 
			// editStartDate
			// 
			this.editStartDate.Caption = "START ";
			this.editStartDate.Edit = this.repoStartDate;
			this.editStartDate.EditWidth = 150;
			this.editStartDate.Id = 8;
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
			this.editEndDate.Caption = "END ";
			this.editEndDate.Edit = this.repoEndDate;
			this.editEndDate.EditWidth = 150;
			this.editEndDate.Id = 9;
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
			// checkAccQty
			// 
			this.checkAccQty.Caption = "SHOW CUM QTY";
			this.checkAccQty.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.checkAccQty.Id = 12;
			this.checkAccQty.Name = "checkAccQty";
			this.checkAccQty.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.checkAccQty_CheckedChanged);
			// 
			// checkOnlyMain
			// 
			this.checkOnlyMain.Caption = "ONLY MAIN";
			this.checkOnlyMain.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.checkOnlyMain.Id = 13;
			this.checkOnlyMain.Name = "checkOnlyMain";
			this.checkOnlyMain.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.checkOnlyMain_CheckedChanged);
			// 
			// buttonLoad
			// 
			this.buttonLoad.Caption = "LOAD";
			this.buttonLoad.Id = 14;
			this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
			this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
			// 
			// buttonExcel
			// 
			this.buttonExcel.Caption = "EXCEL";
			this.buttonExcel.Id = 15;
			this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
			this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
			this.buttonExcel.Name = "buttonExcel";
			this.buttonExcel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.buttonExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonExcel_ItemClick);
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup7,
            this.ribbonPageGroup2,
            this.ribbonPageGroup8,
            this.ribbonPageGroup3,
            this.ribbonPageGroup9,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5,
            this.ribbonPageGroup6});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup7
			// 
			this.ribbonPageGroup7.Name = "ribbonPageGroup7";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.ItemLinks.Add(this.editAreaId);
			this.ribbonPageGroup2.ItemLinks.Add(this.editShopId);
			this.ribbonPageGroup2.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.TwoRows;
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup2.Text = "ribbonPageGroup2";
			// 
			// ribbonPageGroup8
			// 
			this.ribbonPageGroup8.Name = "ribbonPageGroup8";
			this.ribbonPageGroup8.Text = "\\";
			// 
			// ribbonPageGroup3
			// 
			this.ribbonPageGroup3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup3.ImageOptions.Image")));
			this.ribbonPageGroup3.ItemLinks.Add(this.editStartDate);
			this.ribbonPageGroup3.ItemLinks.Add(this.editEndDate);
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.Text = "DATE";
			// 
			// ribbonPageGroup9
			// 
			this.ribbonPageGroup9.Name = "ribbonPageGroup9";
			// 
			// ribbonPageGroup4
			// 
			this.ribbonPageGroup4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup4.ImageOptions.Image")));
			this.ribbonPageGroup4.ItemLinks.Add(this.checkAccQty);
			this.ribbonPageGroup4.ItemLinks.Add(this.checkOnlyMain);
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			this.ribbonPageGroup4.Text = "CONDITION";
			// 
			// ribbonPageGroup5
			// 
			this.ribbonPageGroup5.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup5.ItemLinks.Add(this.buttonLoad);
			this.ribbonPageGroup5.ItemLinks.Add(this.buttonExcel);
			this.ribbonPageGroup5.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
			this.ribbonPageGroup5.Name = "ribbonPageGroup5";
			this.ribbonPageGroup5.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup5.Text = "ribbonPageGroup5";
			// 
			// ribbonPageGroup6
			// 
			this.ribbonPageGroup6.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup6.Name = "ribbonPageGroup6";
			// 
			// repoVersionNo
			// 
			this.repoVersionNo.AutoHeight = false;
			this.repoVersionNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoVersionNo.Name = "repoVersionNo";
			// 
			// repoStepId
			// 
			this.repoStepId.AutoHeight = false;
			this.repoStepId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoStepId.Name = "repoStepId";
			// 
			// repoProdType
			// 
			this.repoProdType.AutoHeight = false;
			this.repoProdType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoProdType.Name = "repoProdType";
			// 
			// repoProdId
			// 
			this.repoProdId.AutoHeight = false;
			this.repoProdId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoProdId.Name = "repoProdId";
			// 
			// repositoryItemCheckEdit1
			// 
			this.repositoryItemCheckEdit1.AutoHeight = false;
			this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
			// 
			// dockManager1
			// 
			this.dockManager1.Form = this;
			this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
			this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane"});
			// 
			// dockPanel1
			// 
			this.dockPanel1.Controls.Add(this.dockPanel1_Container);
			this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
			this.dockPanel1.ID = new System.Guid("c0127932-3f32-44a8-bdc9-09c6a2febc50");
			this.dockPanel1.Location = new System.Drawing.Point(0, 602);
			this.dockPanel1.Name = "dockPanel1";
			this.dockPanel1.Options.ShowCloseButton = false;
			this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
			this.dockPanel1.Size = new System.Drawing.Size(1225, 200);
			this.dockPanel1.Text = "Compare Chart";
			// 
			// dockPanel1_Container
			// 
			this.dockPanel1_Container.Controls.Add(this.chartControl1);
			this.dockPanel1_Container.Location = new System.Drawing.Point(4, 24);
			this.dockPanel1_Container.Name = "dockPanel1_Container";
			this.dockPanel1_Container.Size = new System.Drawing.Size(1217, 172);
			this.dockPanel1_Container.TabIndex = 0;
			// 
			// chartControl1
			// 
			xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
			xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
			this.chartControl1.Diagram = xyDiagram1;
			this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartControl1.Legend.Name = "Default Legend";
			this.chartControl1.Location = new System.Drawing.Point(0, 0);
			this.chartControl1.Name = "chartControl1";
			series1.Name = "Series 1";
			series2.Name = "Series 2";
			this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
			this.chartControl1.Size = new System.Drawing.Size(1217, 172);
			this.chartControl1.SmallChartText.Text = "Increase the chart\'s size,\r\nto view its layout.\r\n    ";
			this.chartControl1.TabIndex = 2;
			// 
			// TargetPlanCompareView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.dockPanel1);
			this.Name = "TargetPlanCompareView";
			this.Size = new System.Drawing.Size(1225, 802);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
			this.dockPanel1.ResumeLayout(false);
			this.dockPanel1_Container.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraBars.Docking.DockManager dockManager1;
		private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
		private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
		private DevExpress.XtraCharts.ChartControl chartControl1;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoVersionNo;
		private DevExpress.XtraBars.BarEditItem editAreaId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoAreaId;
		private DevExpress.XtraBars.BarEditItem editShopId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoShopId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStepId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoProdType;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoProdId;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoStartDate;
		private DevExpress.XtraBars.BarEditItem editEndDate;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoEndDate;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
		private DevExpress.XtraBars.BarCheckItem checkAccQty;
		private DevExpress.XtraBars.BarCheckItem checkOnlyMain;
		private DevExpress.XtraBars.BarButtonItem buttonLoad;
		private DevExpress.XtraBars.BarButtonItem buttonExcel;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup6;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup7;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup8;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup9;
		private DevExpress.XtraBars.BarEditItem editStartDate;
		private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl1;
	}
}
