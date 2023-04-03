using System;

namespace SmartAPS.MozartStudio.Analysis
{
    partial class WipVsMoveView
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
			DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
			DevExpress.XtraCharts.StackedBarSeriesView stackedBarSeriesView1 = new DevExpress.XtraCharts.StackedBarSeriesView();
			DevExpress.XtraCharts.Series series3 = new DevExpress.XtraCharts.Series();
			DevExpress.XtraCharts.LineSeriesView lineSeriesView2 = new DevExpress.XtraCharts.LineSeriesView();
			DevExpress.XtraCharts.PointSeriesLabel pointSeriesLabel1 = new DevExpress.XtraCharts.PointSeriesLabel();
			DevExpress.XtraCharts.LineSeriesView lineSeriesView3 = new DevExpress.XtraCharts.LineSeriesView();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WipVsMoveView));
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
			this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
			this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
			this.chartControl = new DevExpress.XtraCharts.ChartControl();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.editAreaId = new DevExpress.XtraBars.BarEditItem();
			this.repoAreaId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editPlanDate = new DevExpress.XtraBars.BarEditItem();
			this.repoDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.CheckSubStep = new DevExpress.XtraBars.BarCheckItem();
			this.btnQuery = new DevExpress.XtraBars.BarButtonItem();
			this.btnExcel = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup9 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup7 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup6 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.repoShopId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repoStepId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repoDateSpin = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
			this.repoVersionNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
			this.dockPanel1.SuspendLayout();
			this.dockPanel1_Container.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(series3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(pointSeriesLabel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).BeginInit();
			this.SuspendLayout();
			// 
			// panelControl1
			// 
			this.panelControl1.Controls.Add(this.gridControl1);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(0, 462);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(1192, 367);
			this.panelControl1.TabIndex = 0;
			// 
			// gridControl1
			// 
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.Location = new System.Drawing.Point(2, 2);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(1188, 363);
			this.gridControl1.TabIndex = 49;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.DetailHeight = 408;
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
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
			this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
			this.dockPanel1.FloatVertical = true;
			this.dockPanel1.ID = new System.Guid("ed670ac2-d3d2-436c-b92e-ac4da6f81cd0");
			this.dockPanel1.Location = new System.Drawing.Point(0, 101);
			this.dockPanel1.Name = "dockPanel1";
			this.dockPanel1.Options.ShowCloseButton = false;
			this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 361);
			this.dockPanel1.Size = new System.Drawing.Size(1192, 361);
			this.dockPanel1.Text = "Chart";
			// 
			// dockPanel1_Container
			// 
			this.dockPanel1_Container.Controls.Add(this.chartControl);
			this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
			this.dockPanel1_Container.Name = "dockPanel1_Container";
			this.dockPanel1_Container.Size = new System.Drawing.Size(1184, 333);
			this.dockPanel1_Container.TabIndex = 0;
			// 
			// chartControl
			// 
			xyDiagram1.AxisX.Label.Angle = 45;
			xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
			xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
			this.chartControl.Diagram = xyDiagram1;
			this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartControl.Legend.MarkerMode = DevExpress.XtraCharts.LegendMarkerMode.CheckBox;
			this.chartControl.Legend.MaxHorizontalPercentage = 30D;
			this.chartControl.Legend.Name = "Default Legend";
			this.chartControl.Location = new System.Drawing.Point(0, 0);
			this.chartControl.Name = "chartControl";
			series1.Name = "Series 1";
			series1.View = lineSeriesView1;
			series2.Name = "Series 2";
			series2.View = stackedBarSeriesView1;
			series3.Name = "Series 3";
			series3.View = lineSeriesView2;
			this.chartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2,
        series3};
			this.chartControl.SeriesTemplate.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Qualitative;
			pointSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True;
			this.chartControl.SeriesTemplate.Label = pointSeriesLabel1;
			this.chartControl.SeriesTemplate.View = lineSeriesView3;
			this.chartControl.Size = new System.Drawing.Size(1184, 333);
			this.chartControl.SmallChartText.Text = "Increase the chart\'s size,\r\nto view its layout.\r\n    ";
			this.chartControl.TabIndex = 2;
			// 
			// ribbonControl1
			// 
			this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
			this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editAreaId,
            this.editPlanDate,
            this.CheckSubStep,
            this.btnQuery,
            this.btnExcel});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 16;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.PageHeaderItemLinks.Add(this.btnQuery);
			this.ribbonControl1.PageHeaderItemLinks.Add(this.btnExcel);
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoAreaId,
            this.repoShopId,
            this.repoStepId,
            this.repoDateEdit,
            this.repoDateSpin,
            this.repoVersionNo});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(1192, 101);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// editAreaId
			// 
			this.editAreaId.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.editAreaId.Caption = "AREA";
			this.editAreaId.Edit = this.repoAreaId;
			this.editAreaId.EditWidth = 120;
			this.editAreaId.Id = 1;
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
			// editPlanDate
			// 
			this.editPlanDate.Caption = "PLAN DATE";
			this.editPlanDate.Edit = this.repoDateEdit;
			this.editPlanDate.EditWidth = 150;
			this.editPlanDate.Id = 4;
			this.editPlanDate.Name = "editPlanDate";
			// 
			// repoDateEdit
			// 
			this.repoDateEdit.Appearance.BackColor = System.Drawing.Color.White;
			this.repoDateEdit.Appearance.Options.UseBackColor = true;
			this.repoDateEdit.AutoHeight = false;
			this.repoDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoDateEdit.Name = "repoDateEdit";
			// 
			// CheckSubStep
			// 
			this.CheckSubStep.Caption = "SHOW SUB STEP";
			this.CheckSubStep.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.CheckSubStep.Id = 8;
			this.CheckSubStep.Name = "CheckSubStep";
			this.CheckSubStep.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.CheckSubStep_CheckedChanged);
			// 
			// btnQuery
			// 
			this.btnQuery.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.btnQuery.Caption = "LOAD";
			this.btnQuery.Id = 10;
			this.btnQuery.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnQuery.ImageOptions.Image")));
			this.btnQuery.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnQuery.ImageOptions.LargeImage")));
			this.btnQuery.Name = "btnQuery";
			this.btnQuery.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btnQuery.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnQuery_ItemClick);
			// 
			// btnExcel
			// 
			this.btnExcel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.btnExcel.Caption = "EXCEL";
			this.btnExcel.Id = 11;
			this.btnExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.ImageOptions.Image")));
			this.btnExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnExcel.ImageOptions.LargeImage")));
			this.btnExcel.Name = "btnExcel";
			this.btnExcel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btnExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExcel_ItemClick);
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup9,
            this.ribbonPageGroup1,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5,
            this.ribbonPageGroup7,
            this.ribbonPageGroup6,
            this.ribbonPageGroup2});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup9
			// 
			this.ribbonPageGroup9.Name = "ribbonPageGroup9";
			// 
			// ribbonPageGroup1
			// 
			this.ribbonPageGroup1.ItemLinks.Add(this.editAreaId);
			this.ribbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.ThreeRows;
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			this.ribbonPageGroup1.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup1.Text = "ribbonPageGroup1";
			// 
			// ribbonPageGroup3
			// 
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			// 
			// ribbonPageGroup4
			// 
			this.ribbonPageGroup4.ItemLinks.Add(this.editPlanDate);
			this.ribbonPageGroup4.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			this.ribbonPageGroup4.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup4.Text = "DATE";
			// 
			// ribbonPageGroup5
			// 
			this.ribbonPageGroup5.Name = "ribbonPageGroup5";
			// 
			// ribbonPageGroup7
			// 
			this.ribbonPageGroup7.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup7.ItemLinks.Add(this.btnQuery);
			this.ribbonPageGroup7.ItemLinks.Add(this.btnExcel);
			this.ribbonPageGroup7.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
			this.ribbonPageGroup7.Name = "ribbonPageGroup7";
			this.ribbonPageGroup7.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup7.Text = "ribbonPageGroup7";
			// 
			// ribbonPageGroup6
			// 
			this.ribbonPageGroup6.ItemLinks.Add(this.CheckSubStep);
			this.ribbonPageGroup6.Name = "ribbonPageGroup6";
			this.ribbonPageGroup6.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup6.Text = "ribbonPageGroup6";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
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
			// repoDateSpin
			// 
			this.repoDateSpin.AutoHeight = false;
			this.repoDateSpin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoDateSpin.Name = "repoDateSpin";
			// 
			// repoVersionNo
			// 
			this.repoVersionNo.AutoHeight = false;
			this.repoVersionNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoVersionNo.Name = "repoVersionNo";
			// 
			// barButtonItem1
			// 
			this.barButtonItem1.Name = "barButtonItem1";
			// 
			// WipVsMoveView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.dockPanel1);
			this.Controls.Add(this.ribbonControl1);
			this.Name = "WipVsMoveView";
			this.Size = new System.Drawing.Size(1192, 829);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
			this.dockPanel1.ResumeLayout(false);
			this.dockPanel1_Container.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(stackedBarSeriesView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(series3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(pointSeriesLabel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(lineSeriesView3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private System.Windows.Forms.Button btnSaveLayOut;
        private DevExpress.XtraCharts.ChartControl chartControl;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup6;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup7;
        private DevExpress.XtraBars.BarEditItem editAreaId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoAreaId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoShopId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStepId;
        private DevExpress.XtraBars.BarEditItem editPlanDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoDateEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoDateSpin;
        private DevExpress.XtraBars.BarCheckItem CheckSubStep;
        private DevExpress.XtraBars.BarButtonItem btnQuery;
        private DevExpress.XtraBars.BarButtonItem btnExcel;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoVersionNo;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup9;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    }
}
