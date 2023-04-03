namespace SmartAPS.UI.Analysis
{
    partial class StepWipSnapshotView
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
            DevExpress.XtraCharts.StackedBarSeriesView stackedBarSeriesView1 = new DevExpress.XtraCharts.StackedBarSeriesView();
            DevExpress.XtraCharts.StackedBarSeriesLabel stackedBarSeriesLabel1 = new DevExpress.XtraCharts.StackedBarSeriesLabel();
            DevExpress.XtraCharts.StackedBarSeriesView stackedBarSeriesView2 = new DevExpress.XtraCharts.StackedBarSeriesView();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepWipSnapshotView));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pivotGridControl1 = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.chartControl = new DevExpress.XtraCharts.ChartControl();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.editPlanDate = new DevExpress.XtraBars.BarEditItem();
            this.repoDateCombo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barCheckItem1 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem2 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem3 = new DevExpress.XtraBars.BarCheckItem();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonSave = new DevExpress.XtraBars.BarButtonItem();
            this.CheckSyncChart = new DevExpress.XtraBars.BarCheckItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup6 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repoAreaId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pivotGridControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 103);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1192, 401);
            this.panelControl1.TabIndex = 0;
            // 
            // pivotGridControl1
            // 
            this.pivotGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pivotGridControl1.Location = new System.Drawing.Point(2, 2);
            this.pivotGridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pivotGridControl1.Name = "pivotGridControl1";
            this.pivotGridControl1.OptionsChartDataSource.ProvideDataByColumns = false;
            this.pivotGridControl1.OptionsView.RowTotalsLocation = DevExpress.XtraPivotGrid.PivotRowTotalsLocation.Near;
            this.pivotGridControl1.OptionsView.ShowFilterSeparatorBar = false;
            this.pivotGridControl1.Size = new System.Drawing.Size(1188, 397);
            this.pivotGridControl1.TabIndex = 49;
            this.pivotGridControl1.CustomFieldSort += new DevExpress.XtraPivotGrid.PivotGridCustomFieldSortEventHandler(this.pivotGridControl1_CustomFieldSort);
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
            this.dockPanel1.ID = new System.Guid("ed670ac2-d3d2-436c-b92e-ac4da6f81cd0");
            this.dockPanel1.Location = new System.Drawing.Point(0, 504);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Options.ShowCloseButton = false;
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 325);
            this.dockPanel1.Size = new System.Drawing.Size(1192, 325);
            this.dockPanel1.Text = "Chart";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.chartControl);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 24);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1184, 297);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // chartControl
            // 
            xyDiagram1.AxisX.NumericScaleOptions.AutoGrid = false;
            xyDiagram1.AxisX.NumericScaleOptions.GridAlignment = DevExpress.XtraCharts.NumericGridAlignment.Custom;
            xyDiagram1.AxisX.NumericScaleOptions.GridSpacing = 0.5D;
            xyDiagram1.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.True;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisX.VisualRange.Auto = false;
            xyDiagram1.AxisX.VisualRange.MaxValueSerializable = "9";
            xyDiagram1.AxisX.VisualRange.MinValueSerializable = "0";
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            this.chartControl.Diagram = xyDiagram1;
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.Legend.Border.Visibility = DevExpress.Utils.DefaultBoolean.False;
            this.chartControl.Legend.Font = new System.Drawing.Font("Tahoma", 7F);
            this.chartControl.Legend.MarkerMode = DevExpress.XtraCharts.LegendMarkerMode.CheckBoxAndMarker;
            this.chartControl.Legend.MaxHorizontalPercentage = 30D;
            this.chartControl.Legend.Name = "Default Legend";
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chartControl.Name = "chartControl";
            series1.Name = "Series 1";
            series1.View = stackedBarSeriesView1;
            this.chartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.chartControl.SeriesTemplate.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Qualitative;
            stackedBarSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True;
            this.chartControl.SeriesTemplate.Label = stackedBarSeriesLabel1;
            this.chartControl.SeriesTemplate.View = stackedBarSeriesView2;
            this.chartControl.Size = new System.Drawing.Size(1184, 297);
            this.chartControl.SmallChartText.Text = "Increase the chart\'s size,\r\nto view its layout.\r\n    ";
            this.chartControl.TabIndex = 1;
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
            this.editPlanDate,
            this.barCheckItem1,
            this.barCheckItem2,
            this.barCheckItem3,
            this.buttonLoad,
            this.buttonSave,
            this.CheckSyncChart,
            this.barStaticItem1,
            this.barStaticItem2,
            this.barStaticItem3});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 17;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonSave);
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoAreaId,
            this.repoDateCombo});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1192, 103);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // editPlanDate
            // 
            this.editPlanDate.Caption = "PLAN DATE ";
            this.editPlanDate.Edit = this.repoDateCombo;
            this.editPlanDate.EditWidth = 150;
            this.editPlanDate.Id = 2;
            this.editPlanDate.Name = "editPlanDate";
            // 
            // repoDateCombo
            // 
            this.repoDateCombo.AutoHeight = false;
            this.repoDateCombo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDateCombo.Name = "repoDateCombo";
            // 
            // barCheckItem1
            // 
            this.barCheckItem1.Caption = "barCheckItem1";
            this.barCheckItem1.Id = 3;
            this.barCheckItem1.Name = "barCheckItem1";
            // 
            // barCheckItem2
            // 
            this.barCheckItem2.Caption = "barCheckItem2";
            this.barCheckItem2.Id = 4;
            this.barCheckItem2.Name = "barCheckItem2";
            // 
            // barCheckItem3
            // 
            this.barCheckItem3.Caption = "barCheckItem3";
            this.barCheckItem3.Id = 5;
            this.barCheckItem3.Name = "barCheckItem3";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonLoad.Caption = "LOAD";
            this.buttonLoad.Id = 9;
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
            this.buttonSave.Id = 10;
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
            this.CheckSyncChart.Id = 13;
            this.CheckSyncChart.Name = "CheckSyncChart";
            this.CheckSyncChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.CheckSyncChart.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.checkSyncChart_CheckedChanged);
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Id = 14;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Id = 15;
            this.barStaticItem2.Name = "barStaticItem2";
            // 
            // barStaticItem3
            // 
            this.barStaticItem3.Id = 16;
            this.barStaticItem3.Name = "barStaticItem3";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup6,
            this.ribbonPageGroup1});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barStaticItem2, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.editPlanDate);
            this.ribbonPageGroup2.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup2.Text = "DATE RANGE";
            // 
            // ribbonPageGroup6
            // 
            this.ribbonPageGroup6.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup6.ItemLinks.Add(this.buttonLoad);
            this.ribbonPageGroup6.ItemLinks.Add(this.buttonSave);
            this.ribbonPageGroup6.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup6.Name = "ribbonPageGroup6";
            this.ribbonPageGroup6.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup6.Text = "ribbonPageGroup6";
            this.ribbonPageGroup6.Visible = false;
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barStaticItem3);
            this.ribbonPageGroup1.ItemLinks.Add(this.CheckSyncChart);
            this.ribbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            this.ribbonPageGroup1.Visible = false;
            // 
            // repoAreaId
            // 
            this.repoAreaId.AutoHeight = false;
            this.repoAreaId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoAreaId.Name = "repoAreaId";
            // 
            // StepWipSnapshotView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "StepWipSnapshotView";
            this.Size = new System.Drawing.Size(1192, 829);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDateCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraCharts.ChartControl chartControl;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoAreaId;
        private DevExpress.XtraBars.BarEditItem editPlanDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoDateCombo;
        private DevExpress.XtraBars.BarCheckItem barCheckItem1;
        private DevExpress.XtraBars.BarCheckItem barCheckItem2;
        private DevExpress.XtraBars.BarCheckItem barCheckItem3;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.BarButtonItem buttonSave;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup6;
		private DevExpress.XtraBars.BarCheckItem CheckSyncChart;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarStaticItem barStaticItem3;
    }
}
