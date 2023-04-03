using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;

using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using SmartAPS.UserLibrary.Utils;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Helper;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Drawings;

namespace SmartAPS.UI.LotGantts
{
    public class LotBar : Bar
    {
        public string LineID;
        public string LotID;
        public string ProductId;
        public string ProcessId;
        public string EqpId;
        public string StepId;

        public EqpMaster.Eqp EqpInfo;
        public DataRow DispatchingInfo;        
        public Color BackColor;

        public bool IsGhostBar;

        private LotGantt Gantt;

        public string BarKey
        {
            get
            {             
                return IsGhostBar == true ? this.LotID : MyHelper.STRING.CreateKey(this.ProductId, this.LotID);
            }
        }

        public LotBar(
        string lineID,
        string eqpId,
        string productId,
        string processId,
        string stepId,
        string lotId,
        DateTime startTime,
        DateTime endTime,        
        int qty,                
        EqpState state,
        EqpMaster.Eqp info,
        LotGantt lotGantt,
        DataRow dispatchingInfo = null,
        bool isGhostBar = false)
            : base(startTime, endTime, qty, qty, state)
        {
            this.LineID = lineID;
            this.EqpId = eqpId;
            this.ProductId = productId;
            this.ProcessId = processId;
            this.StepId = stepId;
            this.LotID = lotId;
            this.EqpInfo = info;
            this.DispatchingInfo = dispatchingInfo;
            this.IsGhostBar = isGhostBar;
            this.Gantt = lotGantt;
        }
        
        public string GetTitle(bool isOnlyLotMode)
        {
            if (this.State == EqpState.PM)
                return this.State.ToString();
            else if (this.State == EqpState.SETUP)
                return "MOVE";

            if (this.IsGhostBar)
            {
                return this.StepId;
            }

            if (isOnlyLotMode)
            {
                if (this.State != EqpState.BUSY)
                    return string.Format("{0}", this.State);

                return string.Format("{0}/{1}/{2}({3})", this.EqpId, this.ProductId, this.StepId, TIQty.ToString());
            }
            else
            {
                if (this.State != EqpState.BUSY)
                    return string.Format("{0}", this.State);

                return string.Format("{0}-{1}\n({2})", this.ProductId, this.StepId, TIQty.ToString());
            }
        }

		public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("LotID :    \t {0}", this.LotID));
            sb.AppendLine(string.Format("EqpID :    \t {0}", this.EqpId));
            sb.AppendLine(string.Format("ProductID :    \t {0}", this.ProductId));
            sb.AppendLine(string.Format("StepID :        \t {0}", this.StepId));

            string state = this.State == EqpState.SETUP ? "MOVE" : this.State.ToString();
            sb.AppendLine(string.Format("State :      \t {0}", state));
            sb.AppendLine(string.Format("Qty :        \t {0}", this.TIQty));
            sb.AppendLine(string.Format("Step :       \t {0}", this.StepId));

            sb.AppendLine();
            sb.AppendLine(string.Format("{0} -> {1}", this.TkinTime.ToString("yyyy-MM-dd HH:mm:ss"), this.TkoutTime.ToString("yyyy-MM-dd HH:mm:ss")));
            sb.AppendLine(string.Format("Gap :           \t {0}", this.TkoutTime - this.TkinTime));

            return sb.ToString();
        }

		public override BarDrawEventArgs GetBarDrawEventArgs()
        {
            var args = new BarDrawEventArgs(null, this);
            args.FrameColor = Color.White;

            args.ForeColor = this.IsGhostBar && this.State != EqpState.PM ? Color.Gray
                : this.State == EqpState.DOWN ? Color.White : Color.Black;

            if (this.State == EqpState.PM)
                args.ForeColor = Color.White;

            return args;
        }

		public override void OnShapeAdded(Shape shape)
        {
            var shapeText = shape.ShapeText.Characters();
            shapeText.Text = this.GetTitle(true) + "\r\n\r\n" + this.ToString();

            if (this.State == EqpState.SETUP)
                shape.Fill.SetSolidFill(Color.Red);
            else if (this.State == EqpState.PM)
                shape.Fill.SetPatternFill(Color.Black, Color.OrangeRed, ShapeFillPatternType.Divot);
            else if (this.State == EqpState.DOWN)
                shape.Fill.SetPatternFill(Color.Gray, Color.Black, ShapeFillPatternType.Percent30);
            else if (this.IsGhostBar)
                shape.Fill.SetPatternFill(Color.LightGray, Color.White, ShapeFillPatternType.Percent30);
            else
            {
                var color = this.Gantt.ColorGen.GetColor(this.LotID);

                if (this.State == EqpState.IDLERUN)
                    shape.Fill.SetPatternFill(Color.Black, color, ShapeFillPatternType.Percent30);
                else
                    shape.Fill.SetSolidFill(color);
            }
        }
	}
}
