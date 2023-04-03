using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DevExpress.XtraSpreadsheet;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using SmartAPS.UserLibrary.Utils;
using SmartAPS;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Helper;

namespace SmartAPS.UI.LotGantts
{
    public class LotGantt : LotGanttMaster
    {
        public enum ViewMode
        {
            LINE,
            LOT,
            PROD
        }
        
        
        Dictionary<string, GanttInfo> _table;

        public Dictionary<string, GanttInfo> Table
        {
            get { return _table; }
        }

        public LotGantt(SpreadsheetControl grid,
            string targetLineID,
            DateTime planStartTime,
            EqpMaster eqpMgr,
            IExperimentResultItem result
        )
            : base(grid, targetLineID, planStartTime, eqpMgr, result)
        {
            _table = new Dictionary<string, GanttInfo>();
        }

        public override void ClearData()
        {
            base.ClearData();
            _table.Clear();
        }

        public class GanttInfo : GanttItem
        {
            //EqpGantt
            public string LineID;

            public string LotID;
            public string ProductID;
            public int LayerSortSeq;
            public int LayerSortSeqByOption;

            public EqpMaster.Eqp EqpInfo;

            public string EqpGroup 
            {
                get { return this.EqpInfo.EqpGroup; }
            }

            public string EqpID
            {
                get { return this.EqpInfo.EqpID;  }
            }


            public GanttInfo(EqpMaster.Eqp eqpInfo, string lindID, string lotID, string ProductID, int layerStdStepSeq = int.MaxValue)
                : base()
            {
                this.EqpInfo = eqpInfo;
                this.LineID = lindID;
                this.LotID = lotID;
                this.ProductID = ProductID;
                this.LayerSortSeq = layerStdStepSeq;
            }

            public override void AddLinkedNode(Bar bar, LinkedBarNode lnkBarNode)
            {
                base.AddLinkedNode((bar as LotBar).BarKey, lnkBarNode);
            }

            protected override bool CheckConflict(bool isDefault, Bar currentBar, Bar prevBar)
            {
                return isDefault && (currentBar as LotBar).BarKey != (prevBar as LotBar).BarKey;
            }

            public void AddItem(string key, LotBar bar, bool isOnlyToolMode, bool isSplitA = false)
            {
                BarList list;
                if (this.Items.TryGetValue(key, out list) == false)
                {
                    this.Items.Add(key, list = new BarList());
                    list.Add(bar);
                    return;
                }

                foreach (LotBar it in list)
                {
                    if (it.LotID != bar.LotID || it.State != bar.State)
                        continue;

                    if (isOnlyToolMode == false)
                    {
                        if (it.ProductId != bar.ProductId || it.State != bar.State)
                            continue;
                    }

                    if (it.IsShiftSplit || (bar.IsShiftSplit && isSplitA == false)
                    && it.State.Equals(bar.State))
                        return;

                    if (it.Merge(bar))
                    {
                        it.TOQty += bar.TOQty;
                        return;
                    }
                }

                list.Add(bar);
            }

            public Bar GetBarItem(string key, string lotId)
            {
                BarList list;
                if (this.Items.TryGetValue(key, out list) == false)
                    return null;

                return list.FindLast(t => (t as LotBar).LotID == lotId);
            }

        }

        public override void  AddItem(
            EqpMaster.Eqp eqpInfo,
            string lineID, 
            string productId, 
            string processId, 
            string stepId, 
            string lotId, 
            DateTime startTime, 
            DateTime endTime,
			DateTime tkInTime,
			DateTime tkOutTime,
			DateTime nextStateStartTime, 
            int qty, 
            EqpState state, 
            DataRow dispatchingInfo = null)
        {
            GanttInfo info = (GanttInfo)TryGetItem(eqpInfo, lineID, lotId, productId, stepId);
            
            //BUSY
            DoAddItem(info, productId, processId, stepId, lotId,
                startTime, endTime, qty, state, eqpInfo, dispatchingInfo);

			//IDLE RUN
			//DateTime barStartTime = endTime;
			//DateTime barEndTime = tkOutTime;

			//if (barEndTime > nextStateStartTime)
			//	barEndTime = nextStateStartTime;

			//if (barStartTime < barEndTime)
			//{
			//	DoAddItem(info, productId, processId, stepId, lotId,
			//		barStartTime, barEndTime, 0, EqpState.IDLERUN, eqpInfo, dispatchingInfo);
			//}
		}

        private void DoAddItem(
            GanttInfo info,
            string productId,
            string processId,      
            string stepId,
            string lotId,
            DateTime startTime,
            DateTime endTime,
            int qty,                        
            EqpState state,
            EqpMaster.Eqp eqpInfo,
            DataRow dispatchingInfo)
        {
            string lineID = info.LineID;
            string eqpId = info.EqpID;

            LotBar currentBar = new LotBar(
                lineID,
                eqpId,
                productId,
                processId,
                stepId,
                lotId,
                startTime,
                endTime,
                qty,
                state,
                eqpInfo,
                this,
                dispatchingInfo,
                false
                );

            var barKey = state != EqpState.DOWN ? currentBar.BarKey : "DOWN";

            if (barKey != string.Empty)
                info.AddItem(barKey, currentBar, this.IsOnlyLineMode);
        }

        public object TryGetItem(EqpMaster.Eqp eqpInfo, string lineId, string lotID, string productID, string stepID = "-")
        {
            string eqpId = eqpInfo.EqpID;

            string key = string.Empty;
            if (this.IsOnlyLineMode)
                key = MyHelper.STRING.CreateKey(lineId, lotID);
            else
                key = MyHelper.STRING.CreateKey(productID, lotID);
            
            GanttInfo info;
            if (_table.TryGetValue(key, out info) == false)
            {
                info = new GanttInfo(eqpInfo, lineId, lotID, productID, 0);
                _table.Add(key, info);
            }

            return info;
       }
        
        public void Expand(bool isDefault)
        {
            foreach (GanttInfo info in _table.Values)
            {
                if (IsVisibleItem(info.ProductID) == false)
                    continue;

                info.Expand(isDefault);
                info.LinkBar(this,isDefault);
            }
        }

        public override void AddVisibleItem(string productID, string prodCode, string pattern)
        {
            if (string.IsNullOrEmpty(pattern) == false)
            {
                if (MyHelper.STRING.Like(prodCode, pattern))
                    base.AddVisibleItem(productID);
            }
            else
            {
                base.AddVisibleItem(productID);
            }
        }

        public enum SortOptions
        {
            LAYER,
            PRODUCT_ID,
            STEP_SEQ,
            LOT_ID
        }

        public class CompareGanttInfo : IComparer<GanttInfo>
        {
            private SortOptions[] sortList;

            public CompareGanttInfo(params SortOptions[] sortList)
            {
                this.sortList = sortList;
            }

            public int Compare(GanttInfo x, GanttInfo y)
            {
                int iCompare = 0;
                foreach (var sort in sortList)
                {
                    if (iCompare == 0)
                        iCompare = Compare(x, y, sort);
                }

                return iCompare;
            }
            
            private int Compare(GanttInfo x, GanttInfo y, SortOptions sort)
            {
                //"설비그룹" , "EQP ID" , "설비명" , "STEP ID" , "PROD CODE"                
                if (sort == SortOptions.PRODUCT_ID)
                {
                    bool empty_x = string.IsNullOrEmpty(x.ProductID) || x.ProductID == "-";
                    bool empty_y = string.IsNullOrEmpty(y.ProductID) || y.ProductID == "-";

                    int cmp = empty_x.CompareTo(empty_y);

                    if (cmp == 0)
                        cmp = x.ProductID.CompareTo(y.ProductID);

                    return cmp;
                }
                else if (sort == SortOptions.STEP_SEQ)
                {

                }
                else if (sort == SortOptions.LOT_ID)
                {
                    return x.LotID.CompareTo(y.LotID);
                }
                else if (sort == SortOptions.LAYER)
                {
                    int xSeq = x.LayerSortSeq;
                    int ySeq = y.LayerSortSeq;

                    return xSeq.CompareTo(ySeq);
                }

                return 0;
            }
        }
    }
}
