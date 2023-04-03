using Mozart.Data;
using System;
using System.Data;
using SmartAPS.UI.Utils;
using Mozart.Extensions;

namespace SmartAPS.UI.Gantts
{
	public partial class EqpGanttLoadQtyView : MyXtraGridTemplate
    {
        private string TargetEqp;
        private DataTable TargetDB;
        private string TargetLayer;

        public EqpGanttLoadQtyView()
        {
            InitializeComponent();
        }

        public EqpGanttLoadQtyView(DataTable dt, string targetEqpId, string targetLayer)
        {
            InitializeComponent();
            this.TargetEqp = targetEqpId;
            this.TargetDB = dt;
            this.TargetLayer = targetLayer;
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();
            RunQuery();
        }
        
        protected override void Query()
        {
            BindData();
        }
        private void BindData()
        {
            if (TargetLayer.IsNullOrEmpty())
                TargetLayer = "ALL";
            this.barStaticItem1.Caption = String.Format("EQP_ID : {0}, LAYER_ID : {1}", TargetEqp, TargetLayer);
            gridControl1.DataSource = TargetDB;
            gridView1.PopulateColumns();
            gridView1.BestFitColumns();
            
        }   
        private void date_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            e.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        }
    }
}