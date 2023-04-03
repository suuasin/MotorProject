namespace SmartAPS.UI.Analysis
{
    partial class StepWipTrendView
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
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.LineSeriesView lineSeriesView1 = new DevExpress.XtraCharts.LineSeriesView();
            DevExpress.XtraCharts.PointSeriesLabel pointSeriesLabel1 = new DevExpress.XtraCharts.PointSeriesLabel();
            DevExpress.XtraCharts.LineSeriesView lineSeriesView2 = new DevExpress.XtraCharts.LineSeriesView();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepWipTrendView));
            DevExpress.Utils.Animation.PushTransition pushTransition1 = new DevExpress.Utils.Animation.PushTransition();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pivotGridControl1 = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.chartControl = new DevExpress.XtraCharts.ChartControl();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.editDateTime = new DevExpress.XtraBars.BarEditItem();
            this.repoDateTime = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.editDateSpin = new DevExpress.XtraBars.BarEditItem();
            this.repoDateSpin = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonSave = new DevExpress.XtraBars.BarButtonItem();
            this.CheckSyncChart = new DevExpress.XtraBars.BarCheckItem();
            this.radioPlanDate = new DevExpress.XtraBars.BarEditItem();
            this.repoPlanDate = new DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup7 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repoAreaId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repoShopId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repoStepId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repoVersionNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.workspaceManager1 = new DevExpress.Utils.WorkspaceManager(this.components);
            this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem4 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem5 = new DevExpress.XtraBars.BarStaticItem();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pointSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPlanDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoShopId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStepId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pivotGridControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 103);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1192, 421);
            this.panelControl1.TabIndex = 0;
            // 
            // pivotGridControl1
            // 
            this.pivotGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pivotGridControl1.Location = new System.Drawing.Point(2, 2);
            this.pivotGridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pivotGridControl1.Name = "pivotGridControl1";
            this.pivotGridControl1.OptionsChartDataSource.ProvideDataByColumns = false;
            this.pivotGridControl1.OptionsView.RowTotalsLocation = DevExpress.XtraPivotGrid.PivotRowTotalsLocation.Near;
            this.pivotGridControl1.OptionsView.ShowFilterSeparatorBar = false;
            this.pivotGridControl1.Size = new System.Drawing.Size(1188, 417);
            this.pivotGridControl1.TabIndex = 49;
            this.pivotGridControl1.CustomFieldSort += new DevExpress.XtraPivotGrid.PivotGridCustomFieldSortEventHandler(this.pivotGridControl1_CustomFieldSort);
            this.pivotGridControl1.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.pivotGridControl1_CellDoubleClick);
            this.pivotGridControl1.CellSelectionChanged += new System.EventHandler(this.pivotGridControl1_CellSelectionChanged);
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
            this.dockPanel1.FloatVertical = true;
            this.dockPanel1.ID = new System.Guid("ed670ac2-d3d2-436c-b92e-ac4da6f81cd0");
            this.dockPanel1.Location = new System.Drawing.Point(0, 524);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Options.ShowCloseButton = false;
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 306);
            this.dockPanel1.Size = new System.Drawing.Size(1192, 306);
            this.dockPanel1.Text = "Chart";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.chartControl);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 24);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1184, 278);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // chartControl
            // 
            xyDiagram1.AxisX.Label.Angle = 45;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            this.chartControl.Diagram = xyDiagram1;
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.Legend.MaxHorizontalPercentage = 30D;
            this.chartControl.Legend.Name = "Default Legend";
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chartControl.Name = "chartControl";
            series1.Name = "Series 1";
            series1.View = lineSeriesView1;
            this.chartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.chartControl.SeriesTemplate.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Qualitative;
            pointSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True;
            this.chartControl.SeriesTemplate.Label = pointSeriesLabel1;
            this.chartControl.SeriesTemplate.View = lineSeriesView2;
            this.chartControl.Size = new System.Drawing.Size(1184, 278);
            this.chartControl.SmallChartText.Text = "Increase the chart\'s size,\r\nto view its layout.\r\n    ";
            this.chartControl.TabIndex = 2;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editDateTime,
            this.barStaticItem1,
            this.editDateSpin,
            this.barStaticItem2,
            this.buttonLoad,
            this.buttonSave,
            this.CheckSyncChart,
            this.radioPlanDate,
            this.barStaticItem3,
            this.barStaticItem4,
            this.barStaticItem5});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 25;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonSave);
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoAreaId,
            this.repoShopId,
            this.repoStepId,
            this.repoDateTime,
            this.repoDateSpin,
            this.repoVersionNo,
            this.repositoryItemCheckEdit1,
            this.repoPlanDate});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1192, 103);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // editDateTime
            // 
            this.editDateTime.Caption = "DATE ";
            this.editDateTime.Edit = this.repoDateTime;
            this.editDateTime.EditWidth = 180;
            this.editDateTime.Id = 4;
            this.editDateTime.Name = "editDateTime";
            // 
            // repoDateTime
            // 
            this.repoDateTime.Appearance.BackColor = System.Drawing.Color.White;
            this.repoDateTime.Appearance.Options.UseBackColor = true;
            this.repoDateTime.AutoHeight = false;
            this.repoDateTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDateTime.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.repoDateTime.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDateTime.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.repoDateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repoDateTime.EditFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.repoDateTime.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repoDateTime.Mask.EditMask = "yyyy-MM-dd HH:mm:ss";
            this.repoDateTime.Name = "repoDateTime";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "~";
            this.barStaticItem1.Id = 5;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // editDateSpin
            // 
            this.editDateSpin.Edit = this.repoDateSpin;
            this.editDateSpin.Id = 6;
            this.editDateSpin.Name = "editDateSpin";
            // 
            // repoDateSpin
            // 
            this.repoDateSpin.AutoHeight = false;
            this.repoDateSpin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDateSpin.MaxValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.repoDateSpin.Name = "repoDateSpin";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Caption = "DAYS";
            this.barStaticItem2.Id = 7;
            this.barStaticItem2.Name = "barStaticItem2";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonLoad.Caption = "LOAD";
            this.buttonLoad.Id = 10;
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
            this.buttonSave.Id = 11;
            this.buttonSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.ImageOptions.Image")));
            this.buttonSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonSave.ImageOptions.LargeImage")));
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonSave_ItemClick);
            // 
            // CheckSyncChart
            // 
            this.CheckSyncChart.BindableChecked = true;
            this.CheckSyncChart.Caption = "SYNC CHART";
            this.CheckSyncChart.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.CheckSyncChart.Checked = true;
            this.CheckSyncChart.Id = 16;
            this.CheckSyncChart.Name = "CheckSyncChart";
            this.CheckSyncChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.CheckSyncChart.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.checkSyncChart_CheckedChanged);
            // 
            // radioPlanDate
            // 
            this.radioPlanDate.Edit = this.repoPlanDate;
            this.radioPlanDate.EditWidth = 70;
            this.radioPlanDate.Id = 18;
            this.radioPlanDate.Name = "radioPlanDate";
            this.radioPlanDate.EditValueChanged += new System.EventHandler(this.radioPlanDate_EditValueChanged);
            // 
            // repoPlanDate
            // 
            this.repoPlanDate.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("H", "HOUR"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("D", "DAY")});
            this.repoPlanDate.ItemsLayout = DevExpress.XtraEditors.RadioGroupItemsLayout.Flow;
            this.repoPlanDate.Name = "repoPlanDate";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup4,
            this.ribbonPageGroup7,
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup4.ImageOptions.Image")));
            this.ribbonPageGroup4.ItemLinks.Add(this.barStaticItem3, true);
            this.ribbonPageGroup4.ItemLinks.Add(this.editDateTime);
            this.ribbonPageGroup4.ItemLinks.Add(this.barStaticItem1);
            this.ribbonPageGroup4.ItemLinks.Add(this.editDateSpin);
            this.ribbonPageGroup4.ItemLinks.Add(this.barStaticItem2);
            this.ribbonPageGroup4.ItemLinks.Add(this.barStaticItem5);
            this.ribbonPageGroup4.ItemLinks.Add(this.radioPlanDate);
            this.ribbonPageGroup4.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup4.Text = "DATE";
            // 
            // ribbonPageGroup7
            // 
            this.ribbonPageGroup7.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup7.ItemLinks.Add(this.buttonLoad);
            this.ribbonPageGroup7.ItemLinks.Add(this.buttonSave);
            this.ribbonPageGroup7.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup7.Name = "ribbonPageGroup7";
            this.ribbonPageGroup7.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup7.Text = "ribbonPageGroup7";
            this.ribbonPageGroup7.Visible = false;
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barStaticItem4, true);
            this.ribbonPageGroup1.ItemLinks.Add(this.CheckSyncChart);
            this.ribbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // repoAreaId
            // 
            this.repoAreaId.AutoHeight = false;
            this.repoAreaId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoAreaId.Name = "repoAreaId";
            // 
            // repoShopId
            // 
            this.repoShopId.AutoHeight = false;
            this.repoShopId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoShopId.Name = "repoShopId";
            // 
            // repoStepId
            // 
            this.repoStepId.AutoHeight = false;
            this.repoStepId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoStepId.Name = "repoStepId";
            // 
            // repoVersionNo
            // 
            this.repoVersionNo.AutoHeight = false;
            this.repoVersionNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoVersionNo.Name = "repoVersionNo";
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Caption = "";
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // workspaceManager1
            // 
            this.workspaceManager1.TargetControl = this;
            this.workspaceManager1.TransitionType = pushTransition1;
            // 
            // barStaticItem3
            // 
            this.barStaticItem3.Id = 22;
            this.barStaticItem3.Name = "barStaticItem3";
            // 
            // barStaticItem4
            // 
            this.barStaticItem4.Id = 23;
            this.barStaticItem4.Name = "barStaticItem4";
            // 
            // barStaticItem5
            // 
            this.barStaticItem5.Id = 24;
            this.barStaticItem5.Name = "barStaticItem5";
            // 
            // StepWipTrendView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "StepWipTrendView";
            this.Size = new System.Drawing.Size(1192, 830);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pointSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPlanDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoShopId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStepId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private System.Windows.Forms.Button btnSaveLayOut;
        private DevExpress.XtraCharts.ChartControl chartControl;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup7;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoAreaId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoShopId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStepId;
        private DevExpress.XtraBars.BarEditItem editDateTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoDateTime;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarEditItem editDateSpin;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoDateSpin;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.BarButtonItem buttonSave;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoVersionNo;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.BarCheckItem CheckSyncChart;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.BarEditItem radioPlanDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup repoPlanDate;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.Utils.WorkspaceManager workspaceManager1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem3;
        private DevExpress.XtraBars.BarStaticItem barStaticItem4;
        private DevExpress.XtraBars.BarStaticItem barStaticItem5;
    }
}
