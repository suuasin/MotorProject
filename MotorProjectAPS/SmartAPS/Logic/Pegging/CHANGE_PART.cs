using Mozart.SeePlan.Pegging;
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

namespace SmartAPS.Logic.Pegging
{
    [FeatureBind()]
    public partial class CHANGE_PART
    {
        public List<object> GET_PART_CHANGE_INFOS0(Mozart.SeePlan.Pegging.PegPart pegPart, bool isRun, ref bool handled, List<object> prevReturnValue)
        {
            SmartAPSPegPart pp = pegPart as SmartAPSPegPart;

            string currentProdID = pp.Product.ProductID;
            string currentStepID = (pp.CurrentStage.Tag as SmartAPSStep).StepID;

            string position = pegPart.CurrentStage.Properties.Get(Constants.Position).ToString();

            var infos = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (
                p.TO_PRODUCT_ID == currentProdID &&
                p.STEP_ID == currentStepID &&
                p.CHANGE_TYPE == position)
                );

            if(position == Constants.WAIT)
            {
                if(infos.Count() > 0)
                {
                    WriteHelper.WriteSmartAPSProductRoute(infos.FirstOrDefault(), pegPart as SmartAPSPegPart);
                }
            }

            return infos.Count() == 0 ? null : infos.ToList<object>();
        }

        public PegPart APPLY_PART_CHANGE_INFO0(PegPart pegPart, object partChangeInfo, bool isRun, ref bool handled, PegPart prevReturnValue)
        {
            PRODUCT_ROUTE route = (PRODUCT_ROUTE)partChangeInfo;

            string prodID = route.FROM_PRODUCT_ID;
            string toProd = (pegPart as SmartAPSPegPart).Product.ProductID;

            SmartAPSPegPart pps = pegPart as SmartAPSPegPart;

            //Wait일 경우 ToProduct로 Pegging??
            //확인해볼 것
            SmartAPSProduct changeProd = InputMart.Instance.SmartAPSProductView.FindRows(prodID).FirstOrDefault();

            if (changeProd != null)
            {
                SmartAPSProcess changeProc = changeProd.Process as SmartAPSProcess;
                SmartAPSStep changeStep = (SmartAPSStep)changeProc.FindStep(route.STEP_ID);

                bool stepChangeFlag = false;

                if (changeStep == null)
                {
                    changeStep = changeProc.LastStep as SmartAPSStep;
                    stepChangeFlag = true;
                }

                var stageName = route.CHANGE_TYPE == Constants.IN ? Constants.PostWaitPeg : Constants.PostRunPeg;
                PegStage changeStage = pegPart.CurrentStage.Model.GetStage(changeStep, stageName);

                SmartAPSPegPart pp = (SmartAPSPegPart)pegPart;
                pp.Product = changeProd;
                pp.CurrentStage = changeStage;
                pp.CurrentStep = changeStep;
                pp.IsPartStepChanged = stepChangeFlag;

                if (route.IN_QTY > 0 && route.OUT_QTY > 0)
                {
                    var rate = route.IN_QTY / (double)route.OUT_QTY;

                    if (rate > 0)
                    {
                        foreach (PegTarget pt in pp.PegTargetList)
                        {
                            pt.Qty *= rate;
                        }
                    }
                }

                return pp;
            }
            string item = $"{nameof(APPLY_PART_CHANGE_INFO0)} " +
                $"{nameof(route.CHANGE_TYPE)} = {route.CHANGE_TYPE}, " +
                $"{nameof(route.TO_PRODUCT_ID)} = {route.TO_PRODUCT_ID}, " +
                $"{nameof(route.FROM_PRODUCT_ID)} = {route.FROM_PRODUCT_ID}";

            ErrorHelper.WriteError(Constants.PE0001, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_PEGGING", "Product data is missing",
                item, null, prodID);

            return pegPart;
        }
    }
}