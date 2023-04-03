using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Mozart.Common;
using Mozart.Collections;
using Mozart.Extensions;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.Persists;
using Mozart.SeePlan;
using Mozart.Simulation.Engine;
using Mozart.Reflection;
using SmartAPS.Logic;
using Mozart.SeePlan.Simulation;
using Mozart.SeePlan.DataModel;

namespace SmartAPS
{
    [FeatureBind()]
    public static partial class SAPSUtils
    {
        public static bool IsEffectiveTime(DateTime targetTime, DateTime startTime, DateTime endTime)
        {
            var dt = targetTime.SplitDate();
            if (dt >= startTime && dt < endTime)
                return true;

            return false;
        }
        public static Type GetValueType(string valueType)
        {
            var vType = valueType.ToUpper();

            if (vType == "STRING")
                return typeof(string);
            else if (vType == "INT")
                return typeof(int);
            else if (vType == "DOUBLE" || vType == "PERCENT" || vType == "NUMBER" || vType == "DECIMAL")
                return typeof(double);
            else if (vType == "DATETIME")
                return typeof(DateTime);
            else if (vType == "BOOL")
                return typeof(bool);
            else if (
                    vType == "WEEK" ||
                    vType == "DAY" ||
                    vType == "SHIFT" ||
                    vType == "HOUR" ||
                    vType == "MINUTE" ||
                    vType == "SEC"
                )
                return typeof(Time);
            else
                return typeof(string);
        }
        public static dynamic GetTValue(dynamic value, Type type)
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch
            {
                return null;
            }

        }
        public static void AddSort<T>(List<T> list, T item, Comparison<T> compare)
        {
            var index = list.BinarySearch(item, compare);
            if (index < 0)
                index = ~index;

            list.Insert(index, item);
        }

        public static CalendarValue GetCalendarValue(List<CalendarValue> list, DateTime now)
        {
            CalendarValue cv = list.Find(it => it.IsEffectiveTime((DateTime)now));
            return cv; 
        }

        public static bool BoolYN(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return false;

            if (val.Equals("Y") == false)
                return false;

            return true;
        }

        public static void ResisterType()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetAttributes<FEBaseClassAttribute>();
                if (attr == null)
                    continue;

                foreach (var it in attr)
                {
                    if (it.Root == "SAPS" && it.IsTypeBinding)
                    {
                        Mozart.Task.Execution.TypeRegistry.Register(type, type, null);
                    }
                }
            }
        }

        public static string CreateKey(params string[] args)
        {
            string sValue = null;
            foreach (string str in args)
            {
                if (sValue == null) sValue = str;
                else sValue += "@" + str;
            }

            return sValue;
        }

        public static string GetFirmPlanKey(SmartAPSLot lot)
        {
            if (lot.CurrentStep == null)
                return string.Empty;

            return CreateKey(lot.LotID, lot.CurrentStep.StepID);
        }

        public static string GetFirmPlanKey(SmartAPSLot lot, string stepId)
        {
            var step = lot.Steps.Where(r => r.StepID == stepId).FirstOrDefault();
            if (step == null)
                return string.Empty;
            
            return CreateKey(lot.LotID, stepId);
        }


        public static void SetBeforePMSchedule(SmartAPSEqp eqp, DateTime startTime, DateTime beforeDate, SmartAPS.Inputs.FACTORY_BREAK fb)
        {
            if (startTime > beforeDate)
            {
                PMSchedule newPm = CreateHelper.CreatePMSchedule(startTime, fb.PERIOD - (startTime - beforeDate).TotalSeconds);
                if (SimInit.Instance.OnUsePMSchedule(newPm, eqp))

                    eqp.BreakList.Add(newPm);
            }
        }

        public static float SetWeight(float maxValue, float targetValue, float section, string type, bool useNegativeNumber = true)
        {
            float subSection = section;
            float weight = 0f;

            if (type == Constants.DESC)
            {
                if (0 > targetValue)
                    weight = 1;
                else if (targetValue >= maxValue)
                    weight = 0;
                else
                {
                    while (subSection > 0)
                    {
                        if (maxValue * subSection / section > targetValue && targetValue >= maxValue * (subSection - 1) / section)
                        {
                            weight = useNegativeNumber ? (section - subSection + 1) / (section + 1) : (section - subSection + 1) / section;
                            break;
                        }
                        subSection--;
                    }
                }

            }
            else if (type == Constants.ASC)
            {
                if (targetValue > maxValue)
                    weight = 1f;
                else if (targetValue <= 0)
                    weight = 0f;
                else
                {
                    while (subSection > 0)
                    {
                        if (maxValue * subSection / section > targetValue && targetValue >= maxValue * (subSection - 1) / section)
                        {
                            weight = useNegativeNumber ? subSection / (section + 1) : (subSection - 1) / section;
                            break;
                        }
                        subSection--;
                    }
                }
            }

            return weight;
        }

        public static string ValidationCriteria(string criteria)
        {
            string[] criteriaArray;
            string retStr = null;

            if (criteria == null)
                criteriaArray = new string[] { Constants.AbbrHOUR, "1", Constants.AbbrSEC, "1" };
            else
            {
                criteriaArray = criteria.Split('/');
                if (criteriaArray.Length != 4)
                {
                    Array.Clear(criteriaArray, 0, criteriaArray.Length);
                    criteriaArray = new string[] { Constants.AbbrHOUR, "1", Constants.AbbrSEC, "1" };
                }
                else
                {
                    for (int i = 0; i < criteriaArray.Length; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (!float.TryParse(criteriaArray[i], out float optionValue))
                                criteriaArray[i] = "1";
                            else if (optionValue <= 0)
                                criteriaArray[i] = "1";
                        }
                        else
                        {
                            if (!Enum.IsDefined(typeof(TimeOptionType), criteriaArray[i].ToUpper()))
                            {
                                criteriaArray[i] = i == 0 ? Constants.AbbrHOUR : Constants.AbbrSEC;
                            }
                        }
                    }
                }

            }
            retStr = string.Join("/", criteriaArray);
            return retStr;
        }

        public static float CalcOptionValue(string type, float value)
        {
            float optionValue = Constants.HourValue;

            switch (type.ToUpper())
            {
                case Constants.AbbrDAY:
                    optionValue = Constants.DayValue * value;
                    break;
                case Constants.AbbrHOUR:
                    optionValue = Constants.HourValue * value;
                    break;
                case Constants.AbbrMIN:
                    optionValue = Constants.MinValue * value;
                    break;
                case Constants.AbbrSEC:
                    optionValue = value;
                    break;
                default:
                    optionValue = Constants.HourValue;
                    break;
            }

            return optionValue;
        }

        public static double ConvertPeriodUnit(double period ,string unit)
        {
            switch (unit)
            {
                case "MIN":
                    period *= 60;
                    break;
                case "HOUR":
                    period *= 60 * 60;
                    break;
                case "DAY":
                    period *= 60 * 60 * 24;
                    break;
            }
            return period;
        }
    }
}
