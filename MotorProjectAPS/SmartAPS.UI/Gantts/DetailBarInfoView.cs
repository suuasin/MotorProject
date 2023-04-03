using Mozart.Studio.TaskModel.UserInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.UI.Analysis;

namespace SmartAPS.UI.Gantts
{
	public partial class DetailBarInfoView : MyXtraGridTemplate
    {
        private GanttBar _bar;
        private List<PRESET_INFO_HIS> _wpList;
        private DateTime _startTime;
        private int _shifts;

        private string TargetVersionNo { get; set; }

        public DetailBarInfoView()
        {
            InitializeComponent();
        }

        public DetailBarInfoView(IExperimentResultItem result, List<PRESET_INFO_HIS> presetList, DateTime startTime, int shifts)            
        {
            InitializeComponent();

            _wpList = presetList;
            this._startTime = startTime;
            this._shifts = shifts;
            this.Result = result;
        }

        public void SetBarInfo(GanttBar bar)
        {
            this._bar = bar;

            StringBuilder sb = new StringBuilder();

            if (bar != null)
            {
                //sb.AppendLine("+ -----------------------------------------");     
                //string stateInfo = MyHelper.STRING.IsEmptyID(bar.StateInfo) ? "" : string.Format(" [{0}]", bar.StateInfo);

                //sb.AppendLine(string.Format("State : {0}{1}", bar.State.ToString(), stateInfo));
                sb.AppendLine(string.Format("EqpID : {0}", bar.EqpID));
                sb.AppendLine(string.Format("StepID : {0}", bar.StepID));

                sb.AppendLine(string.Format("InQty : {0}", bar.TIQty));
                sb.AppendLine(string.Format("Start : {0}", bar.StartTime.ToString("yyyy-MM-dd HH:mm:ss")));
                sb.AppendLine(string.Format("End : {0}", bar.EndTime.ToString("yyyy-MM-dd HH:mm:ss")));
                sb.AppendLine(string.Format("Gap : {0}", bar.EndTime - bar.StartTime));

                sb.AppendLine(string.Format("LotID : {0}", bar.OrigLotID));
                sb.AppendLine(string.Format("ProductID : {0}", bar.ProductID));

                sb.AppendLine(string.Format("ProcessID : {0}", bar.ProcessID));
            }

            memoEdit1.Text = sb.ToString();

            btnDispBar.Enabled = bar != null && bar.DispInfo != null;
            btnDispEqp.Enabled = bar != null;
        }

        private void DetailBarInfoDialog_Shown(object sender, EventArgs e)
        {
            memoEdit1.SelectionStart = memoEdit1.Text.Length;
        }

        private void btnDispBar_Click(object sender, EventArgs e)
        {
            OpenDispatchInfoPopup();
		}

        public void OpenDispatchInfoPopup()
        {
            if (_bar == null)
                return;

            var info = _bar.DispInfo;
            if (info == null)
                return;

            try
            {
                DispatchingInfoViewPopup form = new DispatchingInfoViewPopup(this.Result, info, _bar.EqpInfo, _wpList);

                form.Show();
                SetFormParentCenter(form);
            }
            catch { }
        }

        private void btnDispEqp_Click(object sender, EventArgs e)
        {
			if (_bar == null)
				return;

			try
			{
				DispatchingAnalysisView control = new DispatchingAnalysisView(this.Result, this._startTime, this._shifts);

				PopUpForm form = new PopUpForm(control);
				form.Show();
				SetFormParentCenter(form);

				control.Query(_bar.EqpGroup, _bar.EqpID);
			}

			catch { }
		}

        private void SetFormParentCenter(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            if (this.ParentForm != null && form.StartPosition == FormStartPosition.CenterParent)
            {
                var x = Location.X + (this.ParentForm.Width - form.Width) / 2;
                var y = Location.Y + (this.ParentForm.Height - form.Height) / 2;
                form.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));            
            }    
        }
    }
}
