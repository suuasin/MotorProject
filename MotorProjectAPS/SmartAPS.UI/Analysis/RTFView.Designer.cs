namespace SmartAPS.UI.Analysis
{
    partial class RTFView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RTFView));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.editStartDate = new DevExpress.XtraBars.BarEditItem();
            this.repoStartDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
            this.editProdId = new DevExpress.XtraBars.BarEditItem();
            this.repoProdId = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.editEndDate = new DevExpress.XtraBars.BarEditItem();
            this.repoEndDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.checkShowAllDmd = new DevExpress.XtraBars.BarCheckItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repoProduct = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoVersionNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridDetail = new DevExpress.XtraGrid.GridControl();
            this.gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridMain = new DevExpress.XtraGrid.GridControl();
            this.gvMain = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.SuspendLayout();
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
            this.editStartDate,
            this.buttonLoad,
            this.buttonExcel,
            this.editProdId,
            this.editEndDate,
            this.barStaticItem1,
            this.barStaticItem2,
            this.checkShowAllDmd});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ribbonControl1.MaxItemId = 16;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonExcel);
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoProduct,
            this.repoStartDate,
            this.repositoryItemSpinEdit1,
            this.repoVersionNo,
            this.repoProdId,
            this.repoEndDate,
            this.repositoryItemCheckEdit1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1154, 126);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // editStartDate
            // 
            this.editStartDate.Caption = "DATE ";
            this.editStartDate.Edit = this.repoStartDate;
            this.editStartDate.EditWidth = 120;
            this.editStartDate.Id = 2;
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
            // buttonLoad
            // 
            this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonLoad.Caption = "LOAD";
            this.buttonLoad.Id = 6;
            this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
            this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
            // 
            // buttonExcel
            // 
            this.buttonExcel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.buttonExcel.Caption = "EXCEL";
            this.buttonExcel.Id = 7;
            this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
            this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
            this.buttonExcel.Name = "buttonExcel";
            this.buttonExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonExcel_ItemClick);
            // 
            // editProdId
            // 
            this.editProdId.Caption = "PRODUCT ";
            this.editProdId.Edit = this.repoProdId;
            this.editProdId.EditWidth = 200;
            this.editProdId.Id = 8;
            this.editProdId.Name = "editProdId";
            // 
            // repoProdId
            // 
            this.repoProdId.AutoHeight = false;
            this.repoProdId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoProdId.Name = "repoProdId";
            // 
            // editEndDate
            // 
            this.editEndDate.Caption = "~  ";
            this.editEndDate.Edit = this.repoEndDate;
            this.editEndDate.EditWidth = 120;
            this.editEndDate.Id = 10;
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
            // barStaticItem1
            // 
            this.barStaticItem1.Id = 11;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Id = 12;
            this.barStaticItem2.Name = "barStaticItem2";
            // 
            // checkShowAllDmd
            // 
            this.checkShowAllDmd.Caption = "All Demands";
            this.checkShowAllDmd.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.checkShowAllDmd.Id = 15;
            this.checkShowAllDmd.Name = "checkShowAllDmd";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup3,
            this.ribbonPageGroup2,
            this.ribbonPageGroup5,
            this.ribbonPageGroup4});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barStaticItem1, true);
            this.ribbonPageGroup1.ItemLinks.Add(this.editProdId);
            this.ribbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.checkShowAllDmd);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "ribbonPageGroup3";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barStaticItem2, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.editStartDate);
            this.ribbonPageGroup2.ItemLinks.Add(this.editEndDate);
            this.ribbonPageGroup2.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup2.Text = "ribbonPageGroup2";
            this.ribbonPageGroup2.Visible = false;
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
            this.ribbonPageGroup5.Visible = false;
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "ribbonPageGroup4";
            // 
            // repoProduct
            // 
            this.repoProduct.AutoHeight = false;
            this.repoProduct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoProduct.Name = "repoProduct";
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
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
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridDetail
            // 
            this.gridDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDetail.Location = new System.Drawing.Point(0, 0);
            this.gridDetail.MainView = this.gvDetail;
            this.gridDetail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDetail.MenuManager = this.ribbonControl1;
            this.gridDetail.Name = "gridDetail";
            this.gridDetail.Size = new System.Drawing.Size(1144, 315);
            this.gridDetail.TabIndex = 1;
            this.gridDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDetail});
            this.gridDetail.DoubleClick += new System.EventHandler(this.gridDetail_DoubleClick);
            // 
            // gvDetail
            // 
            this.gvDetail.DetailHeight = 450;
            this.gvDetail.FixedLineWidth = 3;
            this.gvDetail.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gvDetail.GridControl = this.gridDetail;
            this.gvDetail.Name = "gvDetail";
            this.gvDetail.OptionsBehavior.Editable = false;
            this.gvDetail.OptionsBehavior.ReadOnly = true;
            this.gvDetail.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            this.gvDetail.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gvDetail.OptionsView.ShowFooter = true;
            this.gvDetail.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDetail_RowCellStyle);
            this.gvDetail.DoubleClick += new System.EventHandler(this.gvDetail_DoubleClick);
            // 
            // gridMain
            // 
            this.gridMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMain.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridMain.Location = new System.Drawing.Point(0, 126);
            this.gridMain.MainView = this.gvMain;
            this.gridMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridMain.MenuManager = this.ribbonControl1;
            this.gridMain.Name = "gridMain";
            this.gridMain.Size = new System.Drawing.Size(1154, 384);
            this.gridMain.TabIndex = 3;
            this.gridMain.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMain});
            this.gridMain.DoubleClick += new System.EventHandler(this.gridMain_DoubleClick);
            // 
            // gvMain
            // 
            this.gvMain.DetailHeight = 450;
            this.gvMain.FixedLineWidth = 3;
            this.gvMain.GridControl = this.gridMain;
            this.gvMain.Name = "gvMain";
            this.gvMain.OptionsBehavior.Editable = false;
            this.gvMain.OptionsView.ColumnAutoWidth = false;
            this.gvMain.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvMain_RowClick);
            this.gvMain.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvMain_RowCellStyle);
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
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockPanel1.ID = new System.Guid("ccf9badd-cfdb-44e2-aaa3-5659ba7ce89b");
            this.dockPanel1.Location = new System.Drawing.Point(0, 510);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 349);
            this.dockPanel1.Size = new System.Drawing.Size(1154, 349);
            this.dockPanel1.Text = "Total by Demand";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.gridDetail);
            this.dockPanel1_Container.Location = new System.Drawing.Point(5, 29);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1144, 315);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // RTFView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridMain);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RTFView";
            this.Size = new System.Drawing.Size(1154, 859);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoProduct;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraGrid.GridControl gridDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail;
        private DevExpress.XtraBars.BarEditItem editStartDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoStartDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoVersionNo;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.BarButtonItem buttonExcel;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraBars.BarEditItem editProdId;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoProdId;
        private DevExpress.XtraBars.BarEditItem editEndDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoEndDate;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarCheckItem checkShowAllDmd;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.GridControl gridMain;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMain;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
    }
}
