namespace SmartAPS.MozartStudio.Analysis
{
    partial class DispatchingLogView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DispatchingLogView));
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.editShopId = new DevExpress.XtraBars.BarEditItem();
			this.repoShopId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editEqpGroup = new DevExpress.XtraBars.BarEditItem();
			this.repoEqpGroup = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
			this.editDateTime = new DevExpress.XtraBars.BarEditItem();
			this.repoDateTime = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
			this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
			this.editDateSpin = new DevExpress.XtraBars.BarEditItem();
			this.repoDateSpin = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
			this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
			this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
			this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup7 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEqpGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			this.SuspendLayout();
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
            this.editShopId,
            this.editEqpGroup,
            this.editDateTime,
            this.barStaticItem1,
            this.editDateSpin,
            this.barStaticItem2,
            this.buttonLoad,
            this.buttonExcel});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 11;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoShopId,
            this.repoEqpGroup,
            this.repoDateTime,
            this.repoDateSpin});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(992, 101);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// editShopId
			// 
			this.editShopId.Caption = "SHOP ";
			this.editShopId.Edit = this.repoShopId;
			this.editShopId.EditWidth = 120;
			this.editShopId.Id = 3;
			this.editShopId.Name = "editShopId";
			this.editShopId.EditValueChanged += new System.EventHandler(this.editShopId_SelectedIndexChanged);
			// 
			// repoShopId
			// 
			this.repoShopId.AutoHeight = false;
			this.repoShopId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoShopId.Name = "repoShopId";
			// 
			// editEqpGroup
			// 
			this.editEqpGroup.Caption = "EQP GROUP ";
			this.editEqpGroup.Edit = this.repoEqpGroup;
			this.editEqpGroup.EditWidth = 120;
			this.editEqpGroup.Id = 4;
			this.editEqpGroup.Name = "editEqpGroup";
			// 
			// repoEqpGroup
			// 
			this.repoEqpGroup.AutoHeight = false;
			this.repoEqpGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoEqpGroup.Name = "repoEqpGroup";
			// 
			// editDateTime
			// 
			this.editDateTime.Caption = "DATE ";
			this.editDateTime.Edit = this.repoDateTime;
			this.editDateTime.EditWidth = 120;
			this.editDateTime.Id = 5;
			this.editDateTime.Name = "editDateTime";
			// 
			// repoDateTime
			// 
			this.repoDateTime.AutoHeight = false;
			this.repoDateTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoDateTime.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoDateTime.Name = "repoDateTime";
			// 
			// barStaticItem1
			// 
			this.barStaticItem1.Caption = "~";
			this.barStaticItem1.Id = 6;
			this.barStaticItem1.Name = "barStaticItem1";
			// 
			// editDateSpin
			// 
			this.editDateSpin.Edit = this.repoDateSpin;
			this.editDateSpin.Id = 7;
			this.editDateSpin.Name = "editDateSpin";
			// 
			// repoDateSpin
			// 
			this.repoDateSpin.AutoHeight = false;
			this.repoDateSpin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoDateSpin.Name = "repoDateSpin";
			// 
			// barStaticItem2
			// 
			this.barStaticItem2.Caption = "Days";
			this.barStaticItem2.Id = 8;
			this.barStaticItem2.Name = "barStaticItem2";
			// 
			// buttonLoad
			// 
			this.buttonLoad.Caption = "LOAD";
			this.buttonLoad.Id = 9;
			this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.Image")));
			this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonLoad.ImageOptions.LargeImage")));
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonLoad_ItemClick);
			// 
			// buttonExcel
			// 
			this.buttonExcel.Caption = "EXCEL";
			this.buttonExcel.Id = 10;
			this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
			this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
			this.buttonExcel.Name = "buttonExcel";
			this.buttonExcel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.buttonExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buttonExcel_ItemClick);
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup7,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.ItemLinks.Add(this.editShopId);
			this.ribbonPageGroup2.ItemLinks.Add(this.editEqpGroup);
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup2.Text = "ribbonPageGroup2";
			// 
			// ribbonPageGroup7
			// 
			this.ribbonPageGroup7.Name = "ribbonPageGroup7";
			// 
			// ribbonPageGroup3
			// 
			this.ribbonPageGroup3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup3.ImageOptions.Image")));
			this.ribbonPageGroup3.ItemLinks.Add(this.editDateTime);
			this.ribbonPageGroup3.ItemLinks.Add(this.barStaticItem1);
			this.ribbonPageGroup3.ItemLinks.Add(this.editDateSpin);
			this.ribbonPageGroup3.ItemLinks.Add(this.barStaticItem2);
			this.ribbonPageGroup3.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.Text = "DATE";
			// 
			// ribbonPageGroup4
			// 
			this.ribbonPageGroup4.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup4.ItemLinks.Add(this.buttonLoad);
			this.ribbonPageGroup4.ItemLinks.Add(this.buttonExcel);
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			this.ribbonPageGroup4.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup4.Text = "ribbonPageGroup4";
			// 
			// ribbonPageGroup5
			// 
			this.ribbonPageGroup5.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup5.Name = "ribbonPageGroup5";
			// 
			// gridControl1
			// 
			this.gridControl1.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.Location = new System.Drawing.Point(0, 101);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(992, 514);
			this.gridControl1.TabIndex = 5;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsSelection.MultiSelect = true;
			this.gridView1.OptionsView.ShowAutoFilterRow = true;
			this.gridView1.OptionsView.ShowFooter = true;
			// 
			// DispatchingLogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridControl1);
			this.Controls.Add(this.ribbonControl1);
			this.Name = "DispatchingLogView";
			this.Size = new System.Drawing.Size(992, 615);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoShopId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoEqpGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateSpin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraGrid.GridControl gridControl1;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private DevExpress.XtraBars.BarEditItem editShopId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoShopId;
		private DevExpress.XtraBars.BarEditItem editEqpGroup;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoEqpGroup;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.BarEditItem editDateTime;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoDateTime;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private DevExpress.XtraBars.BarStaticItem barStaticItem1;
		private DevExpress.XtraBars.BarEditItem editDateSpin;
		private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoDateSpin;
		private DevExpress.XtraBars.BarStaticItem barStaticItem2;
		private DevExpress.XtraBars.BarButtonItem buttonLoad;
		private DevExpress.XtraBars.BarButtonItem buttonExcel;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup7;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
	}
}
