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
    public partial class ToolControl
    {
        public bool IS_NEED_TOOL_SETTINGS0(Mozart.SeePlan.Simulation.AoEquipment eqp, ILot lot, ref bool handled, bool prevReturnValue)
        {
            SmartAPSLot tlot = lot as SmartAPSLot;
            bool useTOOL = ConfigHelper.GetConfig<bool>(Constants.USE_TOOL);

            if (useTOOL)
            {
                var tools = InputMart.Instance.TOOL_ARRANGEView.FindRows(tlot.LineID, tlot.CurrentStepID, tlot.CurrentProductID);

                if (tools.Count() == 0)
                    return false;

                return true;
            }

            return false;

           
        }

        public IToolData GET_TOOL_DATA0(AoEquipment eqp, ILot lot, ref bool handled, IToolData prevReturnValue)
        {
            //사용 가능한 tool 유효성 검증 
            SmartAPSTool tool = new SmartAPSTool();
            tool.Tools = new List<string>();
            SmartAPSLot tlot = lot as SmartAPSLot;
            var tools = InputMart.Instance.TOOL_ARRANGEView.FindRows(tlot.LineID, tlot.CurrentStepID, tlot.CurrentProductID);

            if (tools.Count() == 0)
                return null;
            var pool = eqp.Factory.GetResourcePool("Tools");
            if (pool == null)
                return null;
            foreach (var t in tools)
            {
                var sres = pool.GetResource(t.TOOL_ID, eqp);
                if (sres == null)
                    continue;
                if (sres.IsAvailable(eqp))
                    tool.Tools.Add(t.TOOL_ID);
                //break;
            }
            return tool;
        }

        public IEnumerable<ToolItem> BUILD_TOOL_ITEMS0(ToolSettings tool, ref bool handled, IEnumerable<ToolItem> prevReturnValue)
        {
            return new List<ToolItem>() { new ToolItem("Tools", 1) };
        }

        public object SELECT_TOOL0(ToolSettings tool, ToolItem item, ILot lot, AoEquipment aeqp, ToolItem last, bool canAlt, ref bool handled, object prevReturnValue)
        {
            //ToolItem 이 실제 사용해야하는 SecondResource 의 Key 를 반환
            var pool = aeqp.Factory.GetResourcePool(item.ResourceType);
            SmartAPSTool tools = tool.Data as SmartAPSTool;
            if (tools == null || pool == null || tools.Tools.Count() <= 0)
                return null;

            string resourceKey = string.Empty;
            foreach (var tt in tools.Tools)
            {
                if (last != null && last.ResourceKey.ToString() == tt)
                {
                    resourceKey = tt;
                    break;
                }
            }
            return string.IsNullOrEmpty(resourceKey.ToString()) ? tools.Tools.First() : resourceKey;
        }
    }
}