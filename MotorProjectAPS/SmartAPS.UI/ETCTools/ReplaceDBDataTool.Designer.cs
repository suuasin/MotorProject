namespace SmartAPS.UI.ETCTools
{
    partial class ReplaceDBDataTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceDBDataTool));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItemLoad = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barEditItemTable = new DevExpress.XtraBars.BarEditItem();
            this.repoEditItemTable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.barButtonItemReplace = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEditItemTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            this.SuspendLayout();
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
            this.barButtonItemLoad,
            this.barButtonItemSave,
            this.barEditItemTable,
            this.barButtonItemReplace});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 5;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoEditItemTable});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.Size = new System.Drawing.Size(842, 124);
            // 
            // barButtonItemLoad
            // 
            this.barButtonItemLoad.Caption = "Load";
            this.barButtonItemLoad.Id = 1;
            this.barButtonItemLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItemLoad.ImageOptions.Image")));
            this.barButtonItemLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItemLoad.ImageOptions.LargeImage")));
            this.barButtonItemLoad.Name = "barButtonItemLoad";
            this.barButtonItemLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemLoad_ItemClick);
            // 
            // barButtonItemSave
            // 
            this.barButtonItemSave.Caption = "Save";
            this.barButtonItemSave.Id = 2;
            this.barButtonItemSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItemSave.ImageOptions.Image")));
            this.barButtonItemSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItemSave.ImageOptions.LargeImage")));
            this.barButtonItemSave.Name = "barButtonItemSave";
            this.barButtonItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemSave_ItemClick);
            // 
            // barEditItemTable
            // 
            this.barEditItemTable.Caption = "Table";
            this.barEditItemTable.Edit = this.repoEditItemTable;
            this.barEditItemTable.EditWidth = 300;
            this.barEditItemTable.Id = 3;
            this.barEditItemTable.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barEditItemTable.ImageOptions.SvgImage")));
            this.barEditItemTable.Name = "barEditItemTable";
            // 
            // repoEditItemTable
            // 
            this.repoEditItemTable.AutoHeight = false;
            this.repoEditItemTable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoEditItemTable.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.repoEditItemTable.Name = "repoEditItemTable";
            // 
            // barButtonItemReplace
            // 
            this.barButtonItemReplace.Caption = "Replace";
            this.barButtonItemReplace.Id = 4;
            this.barButtonItemReplace.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barButtonItemReplace.ImageOptions.SvgImage")));
            this.barButtonItemReplace.Name = "barButtonItemReplace";
            this.barButtonItemReplace.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemReplace_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup3,
            this.ribbonPageGroup2,
            this.ribbonPageGroup4});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ReplaceDBDataTool";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItemLoad);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup3.ItemLinks.Add(this.barButtonItemReplace);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItemSave);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.barEditItemTable);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // xtraTabControl
            // 
            this.xtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl.Location = new System.Drawing.Point(0, 124);
            this.xtraTabControl.Name = "xtraTabControl";
            this.xtraTabControl.Size = new System.Drawing.Size(842, 519);
            this.xtraTabControl.TabIndex = 5;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "left_16x16.png");
            this.imageCollection1.Images.SetKeyName(1, "right_16x16.png");
            // 
            // ReplaceDBDataTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xtraTabControl);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "ReplaceDBDataTool";
            this.Size = new System.Drawing.Size(842, 643);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEditItemTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemLoad;
        private DevExpress.XtraBars.BarButtonItem barButtonItemSave;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarEditItem barEditItemTable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoEditItemTable;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl;
        private DevExpress.XtraBars.BarButtonItem barButtonItemReplace;
        private DevExpress.Utils.ImageCollection imageCollection1;
    }
}
