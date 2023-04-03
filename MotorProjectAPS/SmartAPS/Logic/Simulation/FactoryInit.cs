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
    public partial class FactoryInit
    {
        public IEnumerable<Mozart.SeePlan.DataModel.WeightPreset> GET_WEIGHT_PRESETS0(Mozart.SeePlan.Simulation.AoFactory factory, ref bool handled, IEnumerable<Mozart.SeePlan.DataModel.WeightPreset> prevReturnValue)
        {
            return InputMart.Instance.SmartAPSWeightPreset.Rows;
        }

        public IList<SecondResourcePool> GET_SECOND_RESOURCE_POOLS0(AoFactory factory, ref bool handled, IList<SecondResourcePool> prevReturnValue)
        {
            List<SecondResourcePool> pools = new List<SecondResourcePool>();

            // Mold resource를 관리하기 위한 SecondResourcePool 객체 생성   
            SecondResourcePool pool = new SecondResourcePool(factory, "Tools");

            foreach (TOOL t in InputMart.Instance.TOOL.Rows)
            {
                SecondResource res = new SecondResource(t.TOOL_ID, t);
                res.Capacity = 1;   // Capacity 가 1인 Resource
                res.Uses = 0;       // 기본상태는 미사용중으로 설정
                res.Pool = pool;
                pool.Add(res);
            }

            pools.Add(pool);
            return pools;
        }

        public void INITIALIZE_WIP_GROUP0(AoFactory factory, IWipManager wipManager, ref bool handled)
        {
            factory.WipManager.AddGroup("GROUP", "CurrentProductID", "CurrentProcessID", "CurrentStepID");
        }
    }
}