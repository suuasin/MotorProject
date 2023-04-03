using DevExpress.Spreadsheet;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using System;
using System.Drawing;
using System.Text;
//using Template.Lcd.Scheduling.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.Outputs;

namespace SmartAPS.UI.Gantts
{
	public class GanttBar : Bar
    {
        public string EqpGroup;
        public string EqpID;
        public string LotID;
        public string OrigLotID;
        public string ProductID;
        public string ProcessID;
        public string StepID;
        
        public DateTime TargetDate;
        public DateTime DispatchInTime;

        public EqpMaster.Eqp EqpInfo;
        public EQP_DISPATCH_LOG DispInfo;

        public Color BackColor;

        public bool IsGhostBar;
        public bool IsProductInBarTitle;
        public bool IsOnTime;

        private GanttMaster Gantt;

        //Eqp 투입을 기준으로 Bar 기록하므로 TkinTime = StateStartTime으로 처리
        public DateTime StartTime
        {
            get { return this.TkinTime; }
        }

        //Eqp 투입을 기준으로 Bar 기록하므로 TkoutTime = StateEndTime으로 처리
        public DateTime EndTime
        {
            get { return this.TkoutTime; }
        }

        public string BarKey
        {
            get
            {
                if (this.IsGhostBar)
                    return this.StepID;

                if (this.IsProductInBarTitle == false)
                    return this.LotID;

                return this.ProductID;
            }
        }

        public GanttBar(
            string eqpGroup,
            string eqpID,
            string lotID,
            string origLotID,
            string productID,
            string processID,
            string stepID,
            DateTime arrivalTime,
            DateTime startTime,
            DateTime endTime,
            DateTime targetDate,
            int inQty,
            EqpState state,
            EqpMaster.Eqp info,
            EQP_DISPATCH_LOG dispInfo,
            GanttMaster eqpGantt,
            bool isOnTime,
            bool isGhostBar = false,
            bool isProductInBarTitle = true)
                : base(startTime, endTime, inQty, 0, state)
        {
            this.EqpGroup = eqpGroup;
            this.EqpID = eqpID;
            this.LotID = lotID;
            this.OrigLotID = origLotID;
            this.ProductID = productID;
            this.ProcessID = processID;
            this.StepID = stepID;
            this.DispatchInTime = arrivalTime;
            this.TargetDate = targetDate;
            this.EqpInfo = info;
            this.DispInfo = dispInfo;
            this.Gantt = eqpGantt;
            this.IsGhostBar = isGhostBar;
            this.IsProductInBarTitle = isProductInBarTitle;
            this.IsOnTime = isOnTime;
        }

        public string GetTitle(bool isProduct)
        {
            if (this.State == EqpState.PM)
            {
                return this.State.ToString();
            }

            if (this.State == EqpState.SETUP)
            {
                return this.State.ToString();
            }

            if (this.IsGhostBar)
            {
                return this.StepID;
            }

            if (isProduct)
            {
                if (this.State != EqpState.BUSY)
                    return string.Format("{0}", this.State);

                return string.Format("{0}/{1}(/{2})", this.ProductID, this.StepID, TIQty.ToString());
            }
            else
            {
                if (this.State != EqpState.BUSY)
                    return string.Format("{0}", this.State);

                return string.Format("{0}-{1}\n({2})", this.LotID, this.StepID, TIQty.ToString());
            }
        }

		public override string ToString()
		{
            StringBuilder sb = new StringBuilder();
   
            sb.AppendLine(string.Format("EqpID : {0}", this.EqpID));
            sb.AppendLine(string.Format("Step : {0}", this.StepID));
            sb.AppendLine(string.Format("InQty : {0}", this.TIQty));
            sb.AppendLine(string.Format("Start : {0}", this.StartTime.ToString("yyyy-MM-dd HH:mm:ss")));
            sb.AppendLine(string.Format("End : {0}", this.EndTime.ToString("yyyy-MM-dd HH:mm:ss")));
            sb.AppendLine(string.Format("Gap : {0}", this.EndTime - this.StartTime));
            sb.AppendLine(string.Format("LotID : {0}", this.OrigLotID));
            sb.AppendLine(string.Format("ProductID : {0}", this.ProductID));
            sb.AppendLine(string.Format("ProcessID : {0}", this.ProcessID));

            return sb.ToString();
        }

		//public override BarDrawEventArgs GetBarDrawEventArgs()
  //      {
  //          Color foreColor = Color.Black;

  //          if (this.State == EqpState.DOWN || this.State == EqpState.PM)
  //              foreColor = Color.White;
  //          else if (this.IsGhostBar)
  //              foreColor = Color.Gray;

  //          var args = new BarDrawEventArgs(null, this);
  //          args.ForeColor = foreColor;
  //          args.FrameColor = Color.White;
  //          args.Text = this.GetTitle(true) + "\r\n\r\n" + this.ToString();

  //          if (this.State == EqpState.SETUP)
  //              args.Background = new BrushInfo(Color.Red);
  //          else if (this.State == EqpState.IDLE || this.State == EqpState.IDLERUN)
  //              args.Background = new BrushInfo(Color.White);
  //          else if (this.State == EqpState.PM)
  //              args.Background = new BrushInfo(Color.Black);
  //          else if (this.State == EqpState.DOWN)
  //              args.Background = new BrushInfo(DevExpress.Spreadsheet.Drawings.ShapeFillPatternType.Percent30, Color.Gray, Color.Black);
  //          else if (this.IsGhostBar)
  //              args.Background = new BrushInfo(DevExpress.Spreadsheet.Drawings.ShapeFillPatternType.Percent30, Color.LightGray, Color.White);

  //          return args;
		//}

		public override void OnShapeAdded(Shape shape)
        {
            if (this.State == EqpState.BUSY || this.State == EqpState.NONE)
            {
                var color = this.Gantt.GetBarColorByProductID(this.ProductID);

                //if (this.WipInitRun == "Y")
                //    shape.Fill.SetPatternFill(Color.Black, color, DevExpress.Spreadsheet.Drawings.ShapeFillPatternType.Percent30);
                //else
                    shape.Fill.SetSolidFill(color);
            }
        }
	}
}
