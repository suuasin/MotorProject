namespace SmartAPS.UI.Analysis
{
    partial class ProductRouteView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductRouteView));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.buttonLoad = new DevExpress.XtraBars.BarButtonItem();
            this.buttonExcel = new DevExpress.XtraBars.BarButtonItem();
            this.editProdId = new DevExpress.XtraBars.BarEditItem();
            this.repoProdId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.editDemandId = new DevExpress.XtraBars.BarEditItem();
            this.repoDemandId = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repoProduct = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repoStartDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repoEndDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.diagramControl1 = new DevExpress.XtraDiagram.DiagramControl();
            this.diagramOrgChartController1 = new DevExpress.XtraDiagram.DiagramOrgChartController(this.components);
            this.diagramContainer1 = new DevExpress.XtraDiagram.DiagramContainer();
            this.diagramShape1 = new DevExpress.XtraDiagram.DiagramShape();
            this.diagramShape2 = new DevExpress.XtraDiagram.DiagramShape();
            this.diagramConnector1 = new DevExpress.XtraDiagram.DiagramConnector();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.hideContainerBottom = new DevExpress.XtraBars.Docking.AutoHideContainer();
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.detailGrid = new DevExpress.XtraPivotGrid.PivotGridControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDemandId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diagramOrgChartController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diagramOrgChartController1.TemplateDiagram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.hideContainerBottom.SuspendLayout();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.detailGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.buttonLoad,
            this.buttonExcel,
            this.editProdId,
            this.editDemandId,
            this.barStaticItem1,
            this.barStaticItem2});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 14;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonLoad);
            this.ribbonControl1.PageHeaderItemLinks.Add(this.buttonExcel);
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoProduct,
            this.repoStartDate,
            this.repositoryItemSpinEdit1,
            this.repoProdId,
            this.repoEndDate,
            this.repoDemandId});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.Size = new System.Drawing.Size(1038, 101);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
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
            this.editProdId.EditWidth = 150;
            this.editProdId.Id = 8;
            this.editProdId.Name = "editProdId";
            this.editProdId.EditValueChanged += new System.EventHandler(this.editProdId_EditValueChanged);
            // 
            // repoProdId
            // 
            this.repoProdId.AutoHeight = false;
            this.repoProdId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoProdId.Name = "repoProdId";
            // 
            // editDemandId
            // 
            this.editDemandId.Caption = "DEMAND ";
            this.editDemandId.Edit = this.repoDemandId;
            this.editDemandId.EditWidth = 150;
            this.editDemandId.Id = 11;
            this.editDemandId.Name = "editDemandId";
            // 
            // repoDemandId
            // 
            this.repoDemandId.AutoHeight = false;
            this.repoDemandId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDemandId.Name = "repoDemandId";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Id = 12;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Id = 13;
            this.barStaticItem2.Name = "barStaticItem2";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup5,
            this.ribbonPageGroup4,
            this.ribbonPageGroup2});
            this.ribbonPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ribbonPage1.ImageOptions.Image")));
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barStaticItem1);
            this.ribbonPageGroup1.ItemLinks.Add(this.editProdId);
            this.ribbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
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
            this.ribbonPageGroup4.Alignment = DevExpress.XtraBars.Ribbon.RibbonPageGroupAlignment.Far;
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barStaticItem2);
            this.ribbonPageGroup2.ItemLinks.Add(this.editDemandId);
            this.ribbonPageGroup2.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "ribbonPageGroup2";
            // 
            // repoProduct
            // 
            this.repoProduct.AutoHeight = false;
            this.repoProduct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoProduct.Name = "repoProduct";
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
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
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
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.diagramControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 101);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1038, 598);
            this.panelControl1.TabIndex = 1;
            // 
            // diagramControl1
            // 
            this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagramControl1.Location = new System.Drawing.Point(2, 2);
            this.diagramControl1.Name = "diagramControl1";
            this.diagramControl1.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Content;
            this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection(new string[] {
            "BasicShapes",
            "BasicFlowchartShapes"});
            this.diagramControl1.OptionsBehavior.SelectionMode = DevExpress.Diagram.Core.SelectionMode.Single;
            this.diagramControl1.OptionsExport.PrintExportMode = DevExpress.Diagram.Core.PrintExportMode.Content;
            this.diagramControl1.OptionsSugiyamaLayout.ColumnsAlignment = DevExpress.Diagram.Core.Alignment.Near;
            this.diagramControl1.OptionsSugiyamaLayout.ColumnSpacing = 100F;
            this.diagramControl1.OptionsSugiyamaLayout.Direction = DevExpress.Diagram.Core.Layout.LayoutDirection.RightToLeft;
            this.diagramControl1.OptionsSugiyamaLayout.LayerSpacing = 100F;
            this.diagramControl1.OptionsTipOverTreeLayout.HorizontalAlignment = DevExpress.Diagram.Core.Alignment.Near;
            this.diagramControl1.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
            this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            this.diagramControl1.OptionsView.ShowGrid = false;
            this.diagramControl1.OptionsView.ShowPageBreaks = false;
            this.diagramControl1.OptionsView.ShowRulers = false;
            this.diagramControl1.OptionsView.Theme = DevExpress.Diagram.Core.DiagramThemes.Linear;
            this.diagramControl1.Size = new System.Drawing.Size(1034, 594);
            this.diagramControl1.TabIndex = 0;
            this.diagramControl1.Text = "diagramControl1";
            this.diagramControl1.SelectionChanged += new System.EventHandler<DevExpress.XtraDiagram.DiagramSelectionChangedEventArgs>(this.diagramControl1_SelectionChanged);
            this.diagramControl1.ItemDrawing += new System.EventHandler<DevExpress.XtraDiagram.DiagramItemDrawingEventArgs>(this.diagramControl1_ItemDrawing);
            this.diagramControl1.CustomDrawItem += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawItemEventArgs>(this.diagramControl1_CustomDrawItem);
            // 
            // diagramOrgChartController1
            // 
            this.diagramOrgChartController1.ChildrenPath = "Children";
            this.diagramOrgChartController1.Diagram = this.diagramControl1;
            this.diagramOrgChartController1.ExpandSubordinatesButtonMode = DevExpress.Diagram.Core.ExpandSubordinatesButtonMode.LookupChildrenInSource;
            this.diagramOrgChartController1.LayoutKind = DevExpress.Diagram.Core.DiagramLayoutKind.Sugiyama;
            // 
            // 
            // 
            this.diagramOrgChartController1.TemplateDiagram.Items.AddRange(new DevExpress.XtraDiagram.DiagramItem[] {
            this.diagramContainer1,
            this.diagramConnector1});
            this.diagramOrgChartController1.TemplateDiagram.Location = new System.Drawing.Point(0, 0);
            this.diagramOrgChartController1.TemplateDiagram.Name = "";
            this.diagramOrgChartController1.TemplateDiagram.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection(new string[] {
            "TemplateDesigner"});
            this.diagramOrgChartController1.TemplateDiagram.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
            this.diagramOrgChartController1.TemplateDiagram.OptionsView.PageSize = new System.Drawing.SizeF(368F, 188F);
            this.diagramOrgChartController1.TemplateDiagram.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.diagramOrgChartController1.TemplateDiagram.OptionsView.ShowPageBreaks = false;
            this.diagramOrgChartController1.TemplateDiagram.OptionsView.Theme = DevExpress.Diagram.Core.DiagramThemes.Linear;
            this.diagramOrgChartController1.TemplateDiagram.TabIndex = 0;
            // 
            // diagramContainer1
            // 
            this.diagramContainer1.Anchors = ((DevExpress.Diagram.Core.Sides)((DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top)));
            this.diagramContainer1.BackgroundId = DevExpress.Diagram.Core.DiagramThemeColorId.Accent1_2;
            this.diagramContainer1.CanAddItems = false;
            this.diagramContainer1.CanCopyWithoutParent = true;
            this.diagramContainer1.ConnectionPoints = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[] {
            new DevExpress.Utils.PointFloat(0.5F, 0F),
            new DevExpress.Utils.PointFloat(1F, 0.5F),
            new DevExpress.Utils.PointFloat(0.5F, 1F),
            new DevExpress.Utils.PointFloat(0F, 0.5F)});
            this.diagramContainer1.DragMode = DevExpress.Diagram.Core.ContainerDragMode.ByAnyPoint;
            this.diagramContainer1.Items.AddRange(new DevExpress.XtraDiagram.DiagramItem[] {
            this.diagramShape1,
            this.diagramShape2});
            this.diagramContainer1.ItemsCanAttachConnectorBeginPoint = false;
            this.diagramContainer1.ItemsCanAttachConnectorEndPoint = false;
            this.diagramContainer1.ItemsCanChangeParent = false;
            this.diagramContainer1.ItemsCanCopyWithoutParent = false;
            this.diagramContainer1.ItemsCanDeleteWithoutParent = false;
            this.diagramContainer1.ItemsCanEdit = false;
            this.diagramContainer1.ItemsCanMove = false;
            this.diagramContainer1.ItemsCanResize = false;
            this.diagramContainer1.ItemsCanRotate = false;
            this.diagramContainer1.ItemsCanSelect = false;
            this.diagramContainer1.ItemsCanSnapToOtherItems = false;
            this.diagramContainer1.ItemsCanSnapToThisItem = false;
            this.diagramContainer1.MoveWithSubordinates = true;
            this.diagramContainer1.Position = new DevExpress.Utils.PointFloat(24F, 24F);
            this.diagramContainer1.Size = new System.Drawing.SizeF(200F, 100F);
            this.diagramContainer1.StrokeId = DevExpress.Diagram.Core.DiagramThemeColorId.Accent1_2;
            this.diagramContainer1.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
            // 
            // diagramShape1
            // 
            this.diagramShape1.Anchors = ((DevExpress.Diagram.Core.Sides)((DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top)));
            this.diagramShape1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.diagramShape1.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.diagramShape1.Appearance.BorderSize = 0;
            this.diagramShape1.Appearance.Font = new System.Drawing.Font("Tahoma", 14F);
            this.diagramShape1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.diagramShape1.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "ID"));
            this.diagramShape1.CanAttachConnectorBeginPoint = false;
            this.diagramShape1.CanAttachConnectorEndPoint = false;
            this.diagramShape1.CanChangeParent = false;
            this.diagramShape1.CanCopyWithoutParent = false;
            this.diagramShape1.CanDeleteWithoutParent = false;
            this.diagramShape1.CanEdit = false;
            this.diagramShape1.CanMove = false;
            this.diagramShape1.CanResize = false;
            this.diagramShape1.CanRotate = false;
            this.diagramShape1.CanSelect = false;
            this.diagramShape1.MoveWithSubordinates = true;
            this.diagramShape1.Size = new System.Drawing.SizeF(200F, 50F);
            // 
            // diagramShape2
            // 
            this.diagramShape2.Anchors = ((DevExpress.Diagram.Core.Sides)((DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top)));
            this.diagramShape2.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.diagramShape2.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.diagramShape2.Appearance.BorderSize = 0;
            this.diagramShape2.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.diagramShape2.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.diagramShape2.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "StepRoute"));
            this.diagramShape2.CanAttachConnectorBeginPoint = false;
            this.diagramShape2.CanAttachConnectorEndPoint = false;
            this.diagramShape2.CanChangeParent = false;
            this.diagramShape2.CanCopyWithoutParent = false;
            this.diagramShape2.CanDeleteWithoutParent = false;
            this.diagramShape2.CanEdit = false;
            this.diagramShape2.CanMove = false;
            this.diagramShape2.CanResize = false;
            this.diagramShape2.CanRotate = false;
            this.diagramShape2.CanSelect = false;
            this.diagramShape2.MoveWithSubordinates = true;
            this.diagramShape2.Position = new DevExpress.Utils.PointFloat(0F, 50F);
            this.diagramShape2.Size = new System.Drawing.SizeF(200F, 50F);
            // 
            // diagramConnector1
            // 
            this.diagramConnector1.Appearance.ContentBackground = System.Drawing.Color.White;
            this.diagramConnector1.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.diagramConnector1.BackgroundId = DevExpress.Diagram.Core.DiagramThemeColorId.Accent1_3;
            this.diagramConnector1.BeginPoint = new DevExpress.Utils.PointFloat(254F, 74F);
            this.diagramConnector1.CanChangeRoute = false;
            this.diagramConnector1.CanDragBeginPoint = false;
            this.diagramConnector1.CanDragEndPoint = false;
            this.diagramConnector1.EndPoint = new DevExpress.Utils.PointFloat(344F, 164F);
            this.diagramConnector1.ForegroundId = DevExpress.Diagram.Core.DiagramThemeColorId.Black;
            this.diagramConnector1.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[] {
            new DevExpress.Utils.PointFloat(344F, 74F)});
            this.diagramConnector1.StrokeId = DevExpress.Diagram.Core.DiagramThemeColorId.Accent1_3;
            // 
            // dockManager1
            // 
            this.dockManager1.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[] {
            this.hideContainerBottom});
            this.dockManager1.Form = this;
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
            // hideContainerBottom
            // 
            this.hideContainerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.hideContainerBottom.Controls.Add(this.dockPanel1);
            this.hideContainerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hideContainerBottom.Location = new System.Drawing.Point(0, 699);
            this.hideContainerBottom.Name = "hideContainerBottom";
            this.hideContainerBottom.Size = new System.Drawing.Size(1038, 20);
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockPanel1.ID = new System.Guid("bdb0971d-3a98-44b8-8490-e1121bdd3360");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 267);
            this.dockPanel1.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockPanel1.SavedIndex = 0;
            this.dockPanel1.Size = new System.Drawing.Size(1038, 267);
            this.dockPanel1.Text = "Prod Grid";
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.detailGrid);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 24);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1030, 239);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // detailGrid
            // 
            this.detailGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailGrid.Location = new System.Drawing.Point(0, 0);
            this.detailGrid.Name = "detailGrid";
            this.detailGrid.OptionsChartDataSource.ProvideDataByColumns = false;
            this.detailGrid.OptionsCustomization.AllowDrag = false;
            this.detailGrid.OptionsCustomization.AllowDragInCustomizationForm = false;
            this.detailGrid.OptionsView.ShowFilterSeparatorBar = false;
            this.detailGrid.Size = new System.Drawing.Size(1030, 239);
            this.detailGrid.TabIndex = 4;
            this.detailGrid.CustomFieldSort += new DevExpress.XtraPivotGrid.PivotGridCustomFieldSortEventHandler(this.detailGrid_CustomFieldSort);
            this.detailGrid.CustomDrawCell += new DevExpress.XtraPivotGrid.PivotCustomDrawCellEventHandler(this.detailGrid_CustomDrawCell);
            // 
            // BOMMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.hideContainerBottom);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "BOMMapView";
            this.Size = new System.Drawing.Size(1038, 719);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProdId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDemandId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diagramOrgChartController1.TemplateDiagram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diagramOrgChartController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.hideContainerBottom.ResumeLayout(false);
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.detailGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoProduct;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoStartDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraBars.BarButtonItem buttonLoad;
        private DevExpress.XtraBars.BarButtonItem buttonExcel;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraBars.BarEditItem editProdId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoProdId;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoEndDate;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraDiagram.DiagramControl diagramControl1;
		private DevExpress.XtraDiagram.DiagramOrgChartController diagramOrgChartController1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraPivotGrid.PivotGridControl detailGrid;
        private DevExpress.XtraBars.BarEditItem editDemandId;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoDemandId;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerBottom;
        private DevExpress.XtraDiagram.DiagramContainer diagramContainer1;
        private DevExpress.XtraDiagram.DiagramShape diagramShape1;
        private DevExpress.XtraDiagram.DiagramShape diagramShape2;
        private DevExpress.XtraDiagram.DiagramConnector diagramConnector1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
    }
}
