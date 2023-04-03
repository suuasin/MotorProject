using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.Inputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SmartAPS.UI.Gantts
{
	public class EqpMaster
    {
        //Dictionary<EqpID, Eqp>
        public Dictionary<string, Eqp> EqpAll { get; set; }

        //DoubleDictionary<lineID, EqpID, Eqp>      
        //private DoubleDictionary<string, string, Eqp> Eqps { get; set; }
        private Dictionary<string, Eqp> Eqps { get; set; }

        //Dictionary<AreaID+EqpGroup, int>
        private Dictionary<string, int> DspGroupSeqs { get; set; }

        private Dictionary<string, int> EqpModelSeqs { get; set; }

        private Dictionary<string, int> ProductSeqs { get; set; }

        IExperimentResultItem Result { get; set; }

        public EqpMaster(IExperimentResultItem result)
        {
            this.Eqps = new Dictionary<string, Eqp>();
			this.EqpAll = new Dictionary<string, Eqp>();
            this.DspGroupSeqs = new Dictionary<string, int>();
            this.EqpModelSeqs = new Dictionary<string, int>();
            this.Result = result;
        }

        internal void LoadEqp()
        {
            var table = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);

            if (table != null || table.Count() == 0)
            {
                foreach (EQUIPMENT srow in table)
                {
                    Eqp eqp = new Eqp(srow);                                        
                    this.AddEqps(eqp);
                }
            }

            SetDspEqpGroupSeq();
            SetEqpModelSeq();
        }

        internal void SetDspEqpGroupSeq()
        {
            var table = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(this.Result);
            if (table == null || table.Count() == 0)
                return;

            foreach (var eqp in this.EqpAll.Values)
            {
                string dspEqpGroup = eqp.DspEqpGroup;
                if (string.IsNullOrEmpty(dspEqpGroup))
                    continue;
                
                //string areaID = GetAreaID(eqp.ShopID);

                var finds = table.OrderBy(t => t.STEP_SEQ);
                //.Where(t => t.AREA_ID == areaID && t.DSP_EQP_GROUP_ID == dspEqpGroup).OrderBy(t => t.STEP_SEQ);
                var find = finds.FirstOrDefault();

                if (find == null)
                   continue;

                eqp.DspEqpGroupSeq = Convert.ToInt32(find.STEP_SEQ);

                //string key = MyHelper.STRING.CreateKey(areaID, eqp.EqpGroup);
                this.DspGroupSeqs[eqp.EqpGroup] = Convert.ToInt32(find.STEP_SEQ);
            }
        }

        internal void SetEqpModelSeq()
        {
            var table = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(this.Result);
            if (table == null || table.Count() == 0)
                return;

            foreach (var eqp in this.EqpAll.Values)
            {
                string eqpModel = eqp.EqpModel;
                if (string.IsNullOrEmpty(eqpModel))
                    continue;

                var finds = table.OrderBy(t => t.STEP_SEQ);
                var find = finds.FirstOrDefault();

                if (find == null)
                    continue;

                eqp.EqpModelSeq = Convert.ToInt32(find.STEP_SEQ);

                this.EqpModelSeqs[eqp.EqpModel] = Convert.ToInt32(find.STEP_SEQ);
            }
        }

        internal void AddEqps(Eqp eqp)
		{
            //this.Eqps.TryAdd(eqp.ShopID, eqp.EqpID, eqp);
            this.Eqps.Add(eqp.EqpID, eqp);

			if (this.EqpAll.ContainsKey(eqp.EqpID) == false)
				this.EqpAll.Add(eqp.EqpID, eqp);
		}

		internal Eqp FindEqp(string eqpId)
        {
            Eqp eqp;
            if (this.EqpAll.TryGetValue(eqpId, out eqp))
                return eqp;

            return null;
        }

        //internal Eqp FindEqp(string lineid, string eqpId)
        //{
        //    Eqp eqp;
        //    if (this.Eqps.TryGetValue(lineid, eqpId, out eqp) == false)
        //        return null;

        //    return eqp;
        //}

        internal Dictionary<string, Eqp> GetEqpsByLine(string selectedLineID = null)
        {
            Dictionary<string, Eqp> dic;
            if (string.IsNullOrEmpty(selectedLineID) || selectedLineID == "ALL")
                return this.EqpAll;

            //if (this.Eqps.TryGetValue(selectedShopID, out dic))
            //    return dic;
            return this.Eqps;
        }

        public int GetDspGroupSeq(string lineID, string eqpGroupID)
        {
            string key = MyHelper.STRING.CreateKey(lineID, eqpGroupID);
            if (string.IsNullOrEmpty(key) == false)
            {
                int seq;
                if (this.DspGroupSeqs.TryGetValue(key, out seq))
                    return seq;
            }

            return 999;
        }

        public int GetEqpModelSeq(string lineID, string eqpModel)
        {
            string key = MyHelper.STRING.CreateKey(lineID, eqpModel);
            if (string.IsNullOrEmpty(key) == false)
            {
                int seq;
                if (this.EqpModelSeqs.TryGetValue(key, out seq))
                    return seq;
            }

            return 999;
        }

        public int GetEqpSeq(string eqpID)
        {
            var eqp = FindEqp(eqpID);
            if (eqp != null)
                return eqp.GetEqpSeq();

            return 999;
        }

        public class Eqp
        {
            public string LineID { get; set; }
            public string EqpGroup { get; set; }
            public string DspEqpGroup { get; set; }
            public string EqpID { get; set; }
            public string EqpType { get; set; }
            public string EqpModel { get; set; }
            //public string MaxBatchSize { get; set; }
            public string MinBatchSize { get; set; }
            //public int ViewSeq { get; set; }

            public int DspEqpGroupSeq { get; set; }

            public int EqpModelSeq { get; set; }

            internal class Schema
            {
                public static string LINE_ID = "LINE_ID";
                public static string EQP_ID = "EQP_ID";
                public static string EQP_GROUP_ID = "EQP_GROUP_ID";
                public static string EQP_MODEL = "EQP_MODEL";
                public static string DSP_EQP_GROUP_ID = "DSP_EQP_GROUP_ID";
                public static string SIM_TYPE = "SIM_TYPE";
                //public static string MAX_BATCH_SIZE = "MAX_BATCH_SIZE";
                //public static string MIN_BATCH_SIZE = "MIN_BATCH_SIZE";
                //public static string VIEW_SEQ = "VIEW_SEQ";
            }

            public bool IsLotBatch
            {
                get
                {
                    if (string.Equals(this.EqpType, "Lot_Batch", StringComparison.CurrentCultureIgnoreCase) ||
                        string.Equals(this.EqpType, "Batch_Inline", StringComparison.CurrentCultureIgnoreCase) ||
                        string.Equals(this.EqpType, "SCRIBE", StringComparison.CurrentCultureIgnoreCase))
                        return true;

                    return false;
                }
            }

            public Eqp(string lineID, string eqpID)
            {
                this.LineID = lineID;
                this.EqpID = eqpID;
                this.EqpGroup = string.Empty;
                
                this.DspEqpGroupSeq = 999;
                this.EqpModelSeq = 999;
                //this.ViewSeq = 999;
            }

            public Eqp(EQUIPMENT srow)
            {
                ChartEqp eqp = new ChartEqp(srow);

                this.LineID = eqp.LineID;
                this.EqpID = eqp.EqpID;
                this.EqpGroup = eqp.EqpGroup;
                this.EqpModel = eqp.EqpModel;
                this.DspEqpGroup = eqp.DspEqpGroup;
                this.EqpType = eqp.SimType;
                //this.MaxBatchSize = eqp.MaxBatchSize.ToString();
                //this.MinBatchSize = eqp.MinBatchSize.ToString();

                this.DspEqpGroupSeq = 999;
                this.EqpModelSeq = 999;
                //this.ViewSeq = eqp.ViewSeq;
            }

            public int GetEqpSeq()
            {                
                int groupSeq = this.DspEqpGroupSeq * 1000;
                //int viewSeq = this.ViewSeq;

                return groupSeq;
                //return groupSeq + viewSeq;
            }

            public static Eqp CreateDummy(string lineID, string eqpID)
            {
                return new Eqp(lineID, eqpID);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();                
                sb.AppendFormat("{0} : {1}\r\n", Schema.EQP_ID, this.EqpID);
                sb.AppendFormat("{0} : {1}\r\n", Schema.EQP_GROUP_ID, this.EqpGroup);
                sb.AppendFormat("{0} : {1}\r\n", Schema.SIM_TYPE, this.EqpType);
                //sb.AppendFormat("{0} : {1}\r\n", EqpGanttChartData.Eqp.Schema.MIN_BATCH_SIZE, string.IsNullOrEmpty(this.MinBatchSize) ? "-" : this.MinBatchSize);
                //sb.AppendFormat("{0} : {1}\r\n", EqpGanttChartData.Eqp.Schema.MAX_BATCH_SIZE, string.IsNullOrEmpty(this.MaxBatchSize) ? "-" : this.MaxBatchSize);

                return sb.ToString();
            }
        }

        #region Eqp
        public class ChartEqp
        {
            public string LineID;
            public string EqpID;
            public string EqpGroup;
            public string EqpModel;
            public string DspEqpGroup;
            public string SimType;
            //public int MaxBatchSize;
            //public int MinBatchSize;
            //public int ViewSeq;

            public ChartEqp(EQUIPMENT row)
            {
                LineID = string.Empty;
                EqpID = string.Empty;
                EqpGroup = string.Empty;
                EqpModel = string.Empty;
                DspEqpGroup = string.Empty;
                SimType = string.Empty;
                //MaxBatchSize = 0;
                //MinBatchSize = 0;
                //ViewSeq = 999;

                ParseRow(row);
            }

            private void ParseRow(EQUIPMENT row)
            {
                LineID = row.LINE_ID;
                EqpID = row.EQP_ID;
                EqpGroup = row.EQP_GROUP;
                EqpModel = row.EQP_MODEL;
                DspEqpGroup = row.EQP_GROUP;
                SimType = row.SIM_TYPE;
                //MaxBatchSize = Convert.ToInt32(row.MAX_BATCH_SIZE);
                //MinBatchSize = Convert.ToInt32(row.MIN_BATCH_SIZE);
                //ViewSeq = row.VIEW_SEQ;
            }

            internal class Schema
            {
                public static string LINE_ID = "LINE_ID";
                public static string EQP_ID = "EQP_ID";
                public static string EQP_GROUP_ID = "EQP_GROUP_ID";
                public static string DSP_EQP_GROUP_ID = "DSP_EQP_GROUP_ID";
                public static string SIM_TYPE = "SIM_TYPE";
                //public static string MAX_BATCH_SIZE = "MAX_BATCH_SIZE";
                //public static string MIN_BATCH_SIZE = "MIN_BATCH_SIZE";
                //public static string VIEW_SEQ = "VIEW_SEQ";
            }
        }

        #endregion 
    }
}
