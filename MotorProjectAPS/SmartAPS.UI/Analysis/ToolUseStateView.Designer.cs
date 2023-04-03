namespace SmartAPS.MozartStudio.Analysis
{
	partial class ToolUseStateView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolUseStateView));
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.editAreaId = new DevExpress.XtraBars.BarEditItem();
			this.repoAreaId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editProdId = new DevExpress.XtraBars.BarEditItem();
			this.repoProdId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editStepId = new DevExpress.XtraBars.BarEditItem();
			this.repoStepId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.editDateTime = new DevExpress.XtraBars.BarEditItem();
			this.repoDateTime = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
			this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
			this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup6 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.repoVersionNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).BeginInit();
			this.SuspendLayout();
			// 
			// gridControl1
			// 
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl1.Location = new System.Drawing.Point(0, 101);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(1039, 612);
			this.gridControl1.TabIndex = 6;
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
			this.gridView1.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridView1_CustomDrawCell);
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
            this.editProdId,
            this.editStepId,
            this.editDateTime,
            this.buttonLoad,
            this.buttonExcel});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ribbonControl1.MaxItemId = 10;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
			this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonExcel);
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoAreaId,
            this.repoProdId,
            this.repoStepId,
            this.repoDateTime,
            this.repoVersionNo});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
			this.ribbonControl1.Size = new System.Drawing.Size(1039, 101);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			// 
			// editAreaId
			// 
			this.editAreaId.Caption = "AREA ";
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
			// editProdId
			// 
			this.editProdId.Caption = "PRODUCT ";
			this.editProdId.Edit = this.repoProdId;
			this.editProdId.EditWidth = 120;
			this.editProdId.Id = 2;
			this.editProdId.Name = "editProdId";
			// 
			// repoProdId
			// 
			this.repoProdId.AutoHeight = false;
			this.repoProdId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoProdId.Name = "repoProdId";
			// 
			// editStepId
			// 
			this.editStepId.Caption = "STEP ";
			this.editStepId.Edit = this.repoStepId;
			this.editStepId.EditWidth = 120;
			this.editStepId.Id = 3;
			this.editStepId.Name = "editStepId";
			// 
			// repoStepId
			// 
			this.repoStepId.AutoHeight = false;
			this.repoStepId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoStepId.Name = "repoStepId";
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
			// buttonLoad
			// 
			this.buttonLoad.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.buttonLoad.Caption = "LOAD";
			this.buttonLoad.Id = 5;
			this.buttonLoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnQeury.ImageOptions.Image")));
			this.buttonLoad.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnQeury.ImageOptions.LargeImage")));
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.buttonLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnQeury_ItemClick);
			// 
			// buttonExcel
			// 
			this.buttonExcel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
			this.buttonExcel.Caption = "EXCEL";
			this.buttonExcel.Id = 9;
			this.buttonExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.Image")));
			this.buttonExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("buttonExcel.ImageOptions.LargeImage")));
			this.buttonExcel.Name = "buttonExcel";
			this.buttonExcel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup1,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4,
            this.ribbonPageGroup6,
            this.ribbonPageGroup5});
			this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
			this.ribbonPage1.Name = "ribbonPage1";
			// 
			// ribbonPageGroup2
			// 
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			// 
			// ribbonPageGroup1
			// 
			this.ribbonPageGroup1.ItemLinks.Add(this.editAreaId);
			this.ribbonPageGroup1.ItemLinks.Add(this.editProdId);
			this.ribbonPageGroup1.ItemLinks.Add(this.editStepId);
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
			this.ribbonPageGroup4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPageGroup4.ImageOptions.Image")));
			this.ribbonPageGroup4.ItemLinks.Add(this.editDateTime);
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			this.ribbonPageGroup4.Text = "DATE";
			// 
			// ribbonPageGroup6
			// 
			this.ribbonPageGroup6.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup6.ItemLinks.Add(this.buttonLoad);
			this.ribbonPageGroup6.ItemLinks.Add(this.buttonExcel);
			this.ribbonPageGroup6.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
			this.ribbonPageGroup6.Name = "ribbonPageGroup6";
			this.ribbonPageGroup6.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
			this.ribbonPageGroup6.Text = "ribbonPageGroup6";
			// 
			// ribbonPageGroup5
			// 
			this.ribbonPageGroup5.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
			this.ribbonPageGroup5.Name = "ribbonPageGroup5";
			// 
			// repoVersionNo
			// 
			this.repoVersionNo.AutoHeight = false;
			this.repoVersionNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repoVersionNo.Name = "repoVersionNo";
			// 
			// ToolUseStateView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridControl1);
			this.Controls.Add(this.ribbonControl1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "ToolUseStateView";
			this.Size = new System.Drawing.Size(1039, 713);
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoAreaId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoProdId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoStepId)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoDateTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repoVersionNo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private DevExpress.XtraGrid.GridControl gridControl1;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.BarEditItem editAreaId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoAreaId;
		private DevExpress.XtraBars.BarEditItem editProdId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoProdId;
		private DevExpress.XtraBars.BarEditItem editStepId;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStepId;
		private DevExpress.XtraBars.BarEditItem editDateTime;
		private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoDateTime;
		private DevExpress.XtraBars.BarButtonItem buttonLoad;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup6;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoVersionNo;
		private DevExpress.XtraBars.BarButtonItem buttonExcel;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
	}
}
