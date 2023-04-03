namespace SmartAPS.MozartStudio.Analysis
{
    partial class ArrangeAnalysisView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArrangeAnalysisView));
			this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
			this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
			this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.gridControl2 = new DevExpress.XtraGrid.GridControl();
			this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.gridControl3 = new DevExpress.XtraGrid.GridControl();
			this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
			this.isOnlyIssue = new DevExpress.XtraBars.BarCheckItem();
			this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
			this.dockPanel1.SuspendLayout();
			this.dockPanel1_Container.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			this.SuspendLayout();
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
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl"});
			// 
			// dockPanel1
			// 
			this.dockPanel1.Controls.Add(this.dockPanel1_Container);
			this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
			this.dockPanel1.FloatSize = new System.Drawing.Size(600, 300);
			this.dockPanel1.ID = new System.Guid("6cb16075-7680-4ae9-bdc6-ca99dc29d9d4");
			this.dockPanel1.Location = new System.Drawing.Point(0, 493);
			this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.dockPanel1.Name = "dockPanel1";
			this.dockPanel1.Options.ShowCloseButton = false;
			this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 250);
			this.dockPanel1.Size = new System.Drawing.Size(966, 250);
			this.dockPanel1.TabText = "Matching Info";
			this.dockPanel1.Text = "Matching Info";
			// 
			// dockPanel1_Container
			// 
			this.dockPanel1_Container.Controls.Add(this.tabControl1);
			this.dockPanel1_Container.Location = new System.Drawing.Point(4, 24);
			this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.dockPanel1_Container.Name = "dockPanel1_Container";
			this.dockPanel1_Container.Size = new System.Drawing.Size(958, 222);
			this.dockPanel1_Container.TabIndex = 0;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(958, 222);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.gridControl2);
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage1.Size = new System.Drawing.Size(950, 195);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "EA MATCH";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// gridControl2
			// 
			this.gridControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl2.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl2.Location = new System.Drawing.Point(3, 2);
			this.gridControl2.MainView = this.gridView2;
			this.gridControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl2.Name = "gridControl2";
			this.gridControl2.Size = new System.Drawing.Size(944, 191);
			this.gridControl2.TabIndex = 6;
			this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
			// 
			// gridView2
			// 
			this.gridView2.Appearance.FocusedRow.BackColor = System.Drawing.Color.White;
			this.gridView2.Appearance.FocusedRow.Options.UseBackColor = true;
			this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
			this.gridView2.GridControl = this.gridControl2;
			this.gridView2.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
			this.gridView2.Name = "gridView2";
			this.gridView2.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridView2.OptionsView.ShowAutoFilterRow = true;
			this.gridView2.OptionsView.ShowGroupPanel = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.gridControl3);
			this.tabPage2.Location = new System.Drawing.Point(4, 23);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage2.Size = new System.Drawing.Size(950, 195);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "MA MATCH";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// gridControl3
			// 
			this.gridControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl3.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl3.Location = new System.Drawing.Point(3, 2);
			this.gridControl3.MainView = this.gridView3;
			this.gridControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl3.Name = "gridControl3";
			this.gridControl3.Size = new System.Drawing.Size(944, 191);
			this.gridControl3.TabIndex = 6;
			this.gridControl3.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView3});
			// 
			// gridView3
			// 
			this.gridView3.Appearance.FocusedRow.BackColor = System.Drawing.Color.White;
			this.gridView3.Appearance.FocusedRow.Options.UseBackColor = true;
			this.gridView3.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
			this.gridView3.GridControl = this.gridControl3;
			this.gridView3.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
			this.gridView3.Name = "gridView3";
			this.gridView3.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridView3.OptionsView.ShowAutoFilterRow = true;
			this.gridView3.OptionsView.ShowGroupPanel = false;
			// 
			// gridControl1
			// 
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl1.Location = new System.Drawing.Point(0, 101);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(966, 392);
			this.gridControl1.TabIndex = 5;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.White;
			this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
			this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridView1.OptionsView.ShowAutoFilterRow = true;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.GridView1_FocusedRowChanged);
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
            this.buttonLoad,
            this.isOnlyIssue,
            this.buttonExcel});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ribbonControl1.MaxItemId = 9;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
			this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonExcel);
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(966, 101);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// buttonLoad
			// 
			this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.buttonLoad.Caption = "LOAD";
			this.buttonLoad.Id = 4;
			this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
			this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
			// 
			// isOnlyIssue
			// 
			this.isOnlyIssue.Caption = "ONLY ISSUE";
			this.isOnlyIssue.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.isOnlyIssue.Id = 6;
			this.isOnlyIssue.Name = "isOnlyIssue";
			this.isOnlyIssue.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.isOnlyIssue_CheckedChanged);
			// 
			// buttonExcel
			// 
			this.buttonExcel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.buttonExcel.Caption = "Excel";
			this.buttonExcel.Id = 8;
			this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
			this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
			this.buttonExcel.Name = "buttonExcel";
			this.buttonExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonExcel_ItemClick);
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup2.ItemLinks.Add(this.isOnlyIssue);
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.Text = "ribbonPageGroup2";
			// 
			// ribbonPageGroup3
			// 
			this.ribbonPageGroup3.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup3.ImageOptions.Image")));
			this.ribbonPageGroup3.ItemLinks.Add(this.buttonLoad);
			this.ribbonPageGroup3.ItemLinks.Add(this.buttonExcel);
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup3.Text = "ribbonPageGroup3";
			// 
			// ribbonPageGroup4
			// 
			this.ribbonPageGroup4.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			// 
			// ArrangeAnalysisView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridControl1);
			this.Controls.Add(this.dockPanel1);
			this.Controls.Add(this.ribbonControl1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "ArrangeAnalysisView";
			this.Size = new System.Drawing.Size(966, 743);
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
			this.dockPanel1.ResumeLayout(false);
			this.dockPanel1_Container.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion
    private DevExpress.XtraBars.Docking.DockManager dockManager1;
    private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
    private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private DevExpress.XtraGrid.GridControl gridControl1;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    private DevExpress.XtraGrid.GridControl gridControl2;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
    private DevExpress.XtraGrid.GridControl gridControl3;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
    private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
    private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
    private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
    private DevExpress.XtraBars.BarButtonItem buttonLoad;        
    private DevExpress.XtraBars.BarCheckItem isOnlyIssue;
    private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
	private DevExpress.XtraBars.BarButtonItem buttonExcel;
	private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
	}
}