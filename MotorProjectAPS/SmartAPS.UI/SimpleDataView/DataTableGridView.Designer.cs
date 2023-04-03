namespace SmartAPS.UI.SimpleDataView
{
    partial class DataTableGridView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataTableGridView));
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.editTable = new DevExpress.XtraBars.BarEditItem();
			this.repoTable = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// ribbonControl1
			// 
			this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
			this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.editTable,
            this.buttonLoad});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 3;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoTable});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(712, 103);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// editTable
			// 
			this.editTable.Caption = "TABLE";
			this.editTable.Edit = this.repoTable;
			this.editTable.EditWidth = 200;
			this.editTable.Id = 1;
			this.editTable.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barEditItem1.ImageOptions.Image")));
			this.editTable.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barEditItem1.ImageOptions.LargeImage")));
			this.editTable.Name = "editTable";
			// 
			// repoTable
			// 
			this.repoTable.Appearance.BackColor = System.Drawing.Color.White;
			this.repoTable.Appearance.Options.UseBackColor = true;
			this.repoTable.AutoHeight = false;
			this.repoTable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoTable.Name = "repoTable";
			// 
			// buttonLoad
			// 
			this.buttonLoad.Caption = "LOAD";
			this.buttonLoad.Id = 2;
			this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItemLoad.ImageOptions.Image")));
			this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItemLoad.ImageOptions.LargeImage")));
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemQuery_ItemClick);
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup1
			// 
			this.ribbonPageGroup1.ItemLinks.Add(this.editTable);
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup2.ItemLinks.Add(this.buttonLoad);
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			// 
			// gridControl1
			// 
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.Location = new System.Drawing.Point(0, 103);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.MenuManager = this.ribbonControl1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(712, 526);
			this.gridControl1.TabIndex = 1;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.DetailHeight = 408;
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			// 
			// DataTableGridView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridControl1);
			this.Controls.Add(this.ribbonControl1);
			this.Name = "DataTableGridView";
			this.Size = new System.Drawing.Size(712, 629);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraBars.BarEditItem editTable;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoTable;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
    }
}
