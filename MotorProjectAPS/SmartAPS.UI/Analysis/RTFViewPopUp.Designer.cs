namespace SmartAPS.UI.Analysis
{
    partial class RTFViewPopUp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RTFViewPopUp));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridDetail = new DevExpress.XtraGrid.GridControl();
            this.gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).BeginInit();
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
            this.barStaticItem1});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.ribbonControl1.MaxItemId = 2;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.Size = new System.Drawing.Size(998, 124);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Id = 1;
            this.barStaticItem1.ItemAppearance.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.barStaticItem1.ItemAppearance.Normal.Options.UseFont = true;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barStaticItem1);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gridDetail);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 124);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(998, 466);
            this.panelControl1.TabIndex = 1;
            // 
            // gridDetail
            // 
            this.gridDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.gridDetail.Location = new System.Drawing.Point(2, 2);
            this.gridDetail.MainView = this.gvDetail;
            this.gridDetail.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.gridDetail.MenuManager = this.ribbonControl1;
            this.gridDetail.Name = "gridDetail";
            this.gridDetail.Size = new System.Drawing.Size(994, 462);
            this.gridDetail.TabIndex = 2;
            this.gridDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDetail});
            // 
            // gvDetail
            // 
            this.gvDetail.DetailHeight = 450;
            this.gvDetail.FixedLineWidth = 3;
            this.gvDetail.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gvDetail.GridControl = this.gridDetail;
            this.gvDetail.LevelIndent = 0;
            this.gvDetail.Name = "gvDetail";
            this.gvDetail.OptionsBehavior.Editable = false;
            this.gvDetail.OptionsBehavior.ReadOnly = true;
            this.gvDetail.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            this.gvDetail.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gvDetail.OptionsView.ShowFooter = true;
            this.gvDetail.PreviewIndent = 0;
            // 
            // RTFViewPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.Name = "RTFViewPopUp";
            this.Size = new System.Drawing.Size(998, 590);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraGrid.GridControl gridDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail;
    }
}
