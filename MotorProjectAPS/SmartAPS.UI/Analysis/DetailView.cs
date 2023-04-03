using DevExpress.XtraGrid.Columns;
using Mozart.Studio.TaskModel.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartAPS.UI.Analysis
{
    public partial class DetailView : Form
    {
        IExperimentResultItem _result { get; set; }
        List<string> _productIDs;
        DateTime _filterStartDate;
        DateTime _filterEndDate;
        DateTime _startDate;
        DateTime _endDate;

        TargetMoveAnalysisView dpControl;
        RTFView rtfView;


        public DetailView()
        {
            InitializeComponent();
            AddControls();
        }

        public DetailView(IExperimentResultItem result, List<string> productID, DateTime filterStartDate, DateTime filterEndDate, DateTime startDate, DateTime endDate)
        {
            this._result = result;
            this._productIDs = productID;
            this._filterStartDate = filterStartDate;
            this._filterEndDate = filterEndDate;
            this._startDate = startDate;
            this._endDate = endDate;
            InitializeComponent();

            AddControls();
        }

        private void AddControls()
        {
            dpControl = new TargetMoveAnalysisView(this._result);            
            this.splitContainerControl1.Panel1.Controls.Add(dpControl);
            dpControl.BringToFront();
            dpControl.Dock = DockStyle.Fill;
            this.label1.Text = "TargetMove Analysis";

            rtfView = new RTFView(this._result);
            this.splitContainerControl1.Panel2.Controls.Add(rtfView);
            rtfView.BringToFront();
            rtfView.Dock = DockStyle.Fill;
            this.label2.Text = "RTF View";

            this.panel1.BackColor = Color.FromArgb(235, 236, 239);
            this.panel2.BackColor = Color.FromArgb(235, 236, 239);
        }

        public void Query()
        {
            TimeSpan span = this._endDate - this._startDate;
            this.dpControl.Query(this._productIDs, this._startDate, span.Days, this.checkBox1.Checked);
            this.rtfView.Query(this._productIDs, this._filterStartDate, this._filterEndDate);
            ApplyFilterDate();            
        }

        private void ApplyFilterDate()
        {
            string fromDay = GetDayString(this._filterStartDate);
            string toDay = GetDayString(this._filterEndDate);
            string fromDate = this._filterStartDate.ToString("yyyyMMdd");
            string toDate = this._filterEndDate.ToString("yyyyMMdd");

            var field = this.dpControl.GetColumnField();

            List<string> list = new List<string>();
            for(DateTime date = this._filterStartDate; date <= this._filterEndDate; date = date.AddDays(1))
                list.Add(GetDayString(date));

            field.FilterValues.ValuesIncluded = list.ToArray();

            string demandFilterString = String.Format("[DUE_DATE] Between('{0}','{1}')", fromDate, toDate);
            string planFilterString = String.Format("[LAST_OUT_DATE] Between('{0}','{1}')", fromDate, toDate);
            string filterString = string.Format("{0} or {1}", demandFilterString, planFilterString);

            this.rtfView.GetGridControl().ActiveFilterString = filterString;
        }

        private string GetDayString(DateTime date)
        {
            //return string.Format("{0}/{1:00}", date.Month, date.Day);
            return date.ToString("yyyyMMdd");
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            TimeSpan span = this._endDate - this._startDate;
            this.dpControl.Query(this._productIDs, this._startDate, span.Days, this.checkBox1.Checked);
        }
    }
}
