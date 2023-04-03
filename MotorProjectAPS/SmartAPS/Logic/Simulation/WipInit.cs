using Mozart.SeePlan.Simulation;
using SmartAPS.Persists;
using SmartAPS.Outputs;
using SmartAPS.Inputs;
using SmartAPS.DataModel;
using Mozart.Task.Execution;
using Mozart.Extensions;
using Mozart.Collections;
using Mozart.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class WipInit
    {
        public IList<Mozart.SeePlan.Simulation.IHandlingBatch> GET_WIPS0(ref bool handled, IList<Mozart.SeePlan.Simulation.IHandlingBatch> prevReturnValue)
        {
            List<IHandlingBatch> result = new List<IHandlingBatch>();

            Dictionary<string, SmartAPSLotBatch> batches = new Dictionary<string, SmartAPSLotBatch>();

            foreach (SmartAPSWipInfo wip in InputMart.Instance.SmartAPSWipInfo.Rows)
            {
                SmartAPSEqp initEqp = wip.InitialEqp as SmartAPSEqp;

                if (initEqp != null && (initEqp.SimType == SimEqpType.LotBatch || initEqp.SimType == SimEqpType.BatchInline))
                {
                    string batchKey = initEqp.ResID + wip.LastTrackInTime.DbToString();

                    if (batches.TryGetValue(batchKey, out SmartAPSLotBatch batch) == false)
                    {
                        batch = new SmartAPSLotBatch();
                        batch.BatchID = batchKey;
                        result.Add(batch);
                        batches.Add(batchKey, batch);
                    }
                    SmartAPSLot lot = SimInit.Instance.OnCreateLot(wip, true);
                    if (lot != null)
                    {
                        lot.ReleaseTime = wip.LineInTime;

                        if (SimHelper.MergePartLot(lot))
                            continue;

                        batch.Contents.Add(lot);
                    }
                }
                else
                {
                    SmartAPSLot lot = SimInit.Instance.OnCreateLot(wip, true);
                    if (lot != null)
                    {
                        lot.ReleaseTime = wip.LineInTime;

                        if (SimHelper.MergePartLot(lot))
                            continue;

                        result.Add(lot);
                    }
                }
            }

            foreach (SmartAPSLotBatch b in batches.Values)
            {
                result.Add(b);
            }

            return result;
        }

        public string GET_LOADING_EQUIPMENT0(Mozart.SeePlan.Simulation.IHandlingBatch hb, ref bool handled, string prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            if (lot.WipInfo.InitialEqp != null)
                return lot.WipInfo.InitialEqp.EqpID;

            return string.Empty;
        }

        public bool CHECK_TRACK_OUT0(AoFactory factory, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            // run중인 lot에대해 statetime에서 현재시간이 얼마나 지났는지 확인, track out 시켜줌
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            DateTime now = factory.NowDT;

            if (lot.WipInfo.InitialEqp != null && lot.StateTime < now)
            {
                var aeqp = factory.GetEquipment(lot.WipInfo.InitialEqp.EqpID);

                if (aeqp == null)
                    return false;

                var stateTime = SimInit.Instance.OnGetStateTime(aeqp, hb);
                var info = TimeHelper.GetProcessTime(hb, aeqp);
                var unitQty = SimHelper.GetUnitQty(hb, aeqp);

                DateTime endTime = SimHelper.MinDateTime;

                if (SimHelper.IsFrimPlan(hb))
                {
                    endTime = SimHelper.GetFirmPlanEndTime(hb);
                }
                else
                {
                    endTime = stateTime.AddSeconds(info.FlowTime.TotalSeconds * unitQty);
                }

                if (endTime <= now)
                    return true;
            }

            return false;
        }

        public DateTime FIX_START_TIME_SAPS(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, DateTime prevReturnValue)
        {
            // site project에서 override한 method를 가져다 씀
            return SimInit.Instance.OnGetStateTime(aeqp, hb);
        }
    }
}