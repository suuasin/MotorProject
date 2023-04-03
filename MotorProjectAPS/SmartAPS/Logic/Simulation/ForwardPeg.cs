using Mozart.SeePlan.DataModel;
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

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class ForwardPeg
    {
        public IEnumerable<Tuple<Mozart.SeePlan.DataModel.Step, object>> GET_STEP_PLAN_KEYS0(Mozart.SeePlan.Simulation.ILot lot, ref bool handled, IEnumerable<Tuple<Mozart.SeePlan.DataModel.Step, object>> prevReturnValue)
        {
            bool useForward = ConfigHelper.GetConfig<bool>(Constants.USE_FORWARDPEG);

            if (useForward)
            {
                List<Tuple<Mozart.SeePlan.DataModel.Step, object>> keys = new List<Tuple<Mozart.SeePlan.DataModel.Step, object>>();
                SmartAPSLot slot = lot as SmartAPSLot;

                bool hardPegOpt = OptionHelper.UseHardPegging();

                if (hardPegOpt)
                {
                    var key = new Tuple<Mozart.SeePlan.DataModel.Step, object>(slot.CurrentStep, (slot.WipInfo as SmartAPSWipInfo).DemandID);
                    keys.Add(key);
                }
                else
                {
                    var key = new Tuple<Mozart.SeePlan.DataModel.Step, object>(slot.CurrentStep, slot.Product);
                    keys.Add(key);
                }
                return keys;
            }
            else
                return null;
        }

        public double GET_FORWARD_PEGGING_QTY0(ILot lot, ref bool handled, double prevReturnValue)
        {
            return lot.UnitQty;
        }

        public int COMPARE_STEP_TARGET0(Mozart.SeePlan.DataModel.StepTarget x, StepTarget y, ref bool handled, int prevReturnValue)
        {
            //포워드패깅의 우선순위
            return x.DueDate.CompareTo(y.DueDate);
        }
    }
}