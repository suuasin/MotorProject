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
using Mozart.SeePlan.Simulation;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class InputBatchInit
    {
        public IEnumerable<Mozart.SeePlan.Simulation.ILot>  INSTANCING0(ref bool handled, IEnumerable<Mozart.SeePlan.Simulation.ILot> prevReturnValue)
        {
            List<SmartAPSLot> inputLots = new List<SmartAPSLot>();

            if (!SimInit.Instance.OnGetBlockInTargetLots())
            {
                string batchOption = SimInit.Instance.OnGetReleaseLotOption();
                int releaseTimeBuffer = SimInit.Instance.OnGetReleaseTimeBuffer();
                List<int> specificTime = SimInit.Instance.OnGetSpecificReleaseTime();
                DateTime defaultReleaseTime = new DateTime();

                foreach (var target in InputMart.Instance.SmartAPSInTarget.Rows.OrderBy(r => r.TargetDate))
                {
                    int inQty = target.TargetQty;
                    var prod = target.Product;
                    int lotSize = SimInit.Instance.OnGetLotSize(prod);

                    if (lotSize > 0)
                    {
                        double term = SimInit.Instance.OnGetReleaseTerm(Convert.ToInt32(Math.Ceiling(inQty / (double)lotSize))); //옵션에 따라 투입 term을 정함
                        int cnt = 0;
                        while (inQty > 0)
                        {
                            int lotQty = lotSize;
                            if (inQty < lotSize)
                                lotQty = inQty;

                            SmartAPSWipInfo w = CreateHelper.CreateWipInfo(target.Product, lotQty, target.DemandID);

                            if (w == null)
                            {
                                inQty -= lotQty;
                                cnt++;
                                continue;
                            }

                            SmartAPSLot lot = SimInit.Instance.OnCreateLot(w, false);

                            lot.ReleaseTime = (target.TargetDate < AoFactory.Current.NowDT) ? AoFactory.Current.NowDT : target.TargetDate; // 투입예약 (wipInit에서 값 넣으면 동작 x 여기서 setting해줘야 동작함)

                            defaultReleaseTime = SimHelper.SetReleaseTime(batchOption, releaseTimeBuffer, specificTime, defaultReleaseTime, target);
                            if (defaultReleaseTime != null)
                                lot.ReleaseTime = defaultReleaseTime;

                            inQty -= lotQty;
                            cnt++;

                            if (SimHelper.MergePartLot(lot))
                                continue;

                            inputLots.Add(lot);

                            WriteHelper.WriteReleaseHistory(lot, target);
                        }
                    }
                    else
                    {
                        SmartAPSWipInfo w = CreateHelper.CreateWipInfo(target.Product, inQty, target.DemandID);

                        if (w == null)
                            continue;

                        SmartAPSLot lot = SimInit.Instance.OnCreateLot(w, false);

                        lot.ReleaseTime = (target.TargetDate < AoFactory.Current.NowDT) ? AoFactory.Current.NowDT : target.TargetDate; // 투입예약 (wipInit에서 값 넣으면 동작 x 여기서 setting해줘야 동작함)

                        defaultReleaseTime = SimHelper.SetReleaseTime(batchOption, releaseTimeBuffer, specificTime, defaultReleaseTime, target);
                        if (defaultReleaseTime != null)
                            lot.ReleaseTime = defaultReleaseTime;

                        if (SimHelper.MergePartLot(lot))
                            continue;

                        inputLots.Add(lot);

                        WriteHelper.WriteReleaseHistory(lot, target);
                    }
                }
                return inputLots;
            }
            else
                return inputLots;
        }

        public IEnumerable<Mozart.SeePlan.Simulation.ILot> INSTANCING1(ref bool handled, IEnumerable<Mozart.SeePlan.Simulation.ILot> prevReturnValue)
        {
            return prevReturnValue;
        }
    }
}