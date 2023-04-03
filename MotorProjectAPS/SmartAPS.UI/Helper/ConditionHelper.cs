using Mozart.Studio.TaskModel.Projects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SmartAPS.UI.Properties;
using SmartAPS.Outputs;
using EngInputs = SmartAPS.Inputs;
using SmartAPS.Inputs;

namespace SmartAPS.UI.Helper
{
	enum SHOP
    {
        ARRAY = 0,
        CF,
        CELL,
        NONE
	}

	enum AREA
	{
		TFT = 0,
		CF,
		CELL,
		NONE
	}

	public class ConditionHelper
	{

		//public static List<object> GetShopId_Output(IExperimentResultItem resultItem, string areaId)
		//{
		//	if (resultItem == null)
		//		return null;

		//	List<SHOP> shopIdList = new List<SHOP>();
		//	List<object> resultList = new List<object>();

		//	var dt = MyHelper.DATASVC.GetEntityData<EngInputs.StdStep>(resultItem);
		//	foreach (var row in dt.Distinct())
		//	{
		//		if (!MyHelper.STRING.Equals(row.AREA_ID, areaId))
		//			continue;

		//		string shopId = row.SHOP_ID;
		//		var value = MyHelper.ENUM.ToEnum(shopId, SHOP.NONE);

		//		if (value != SHOP.NONE && !shopIdList.Contains(value))
		//			shopIdList.Add(value);

		//		if (string.IsNullOrEmpty(shopId) == false && resultList.Contains(shopId) == false)
		//			resultList.Add(shopId);
		//	}

		//	if (shopIdList.Count > 0)
		//	{
		//		shopIdList.Sort();
		//		resultList = shopIdList.ConvertAll<object>(item => item.ToString());
		//	}
		//	else if (resultList.Count > 0)
		//		resultList.Sort();

		//	return resultList;
		//}

		//public static List<object> GetProductionType(IExperimentResultItem resultItem)
		//{
		//	if (resultItem == null)
		//		return null;

		//	List<object> list = new List<object>();

		//	var table = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(resultItem);
		//	if (table != null)
		//	{

		//		foreach (var item in table)
		//		{
		//			string value = item.PRODUCTION_TYPE;

		//			if (value != null && list.Contains(value) == false)
		//				list.Add(value);
		//		}
		//	}

		//	list.Sort();
		//	return list;
		//}

		public static List<object> GetLineId_Line(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> list = new List<object>();

			var table = MyHelper.DATASVC.GetEntityData<LINE_INFO>(resultItem);
			if (table != null)
			{
				foreach (var item in table.Distinct())
				{
					string value = item.LINE_ID;

					if (!String.IsNullOrEmpty(value) && !list.Contains(value))
						list.Add(value);
				}
			}

			return list;
		}

		public static List<object> GetProductId_Demand(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> list = new List<object>();

			var table = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(resultItem);
			if (table != null)
			{
				foreach (var item in table.Distinct())
				{
					string value = item.PRODUCT_ID;

					if (!String.IsNullOrEmpty(value) && !list.Contains(value))
						list.Add(value);
				}
			}

			return list;
		}

		public static List<object> GetProductId_Wip(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> list = new List<object>();

			var table = MyHelper.DATASVC.GetEntityData<WIP>(resultItem);
			if (table != null)
			{
				foreach (var item in table.Distinct())
				{
					string value = item.PRODUCT_ID;

					if (!String.IsNullOrEmpty(value) && !list.Contains(value))
						list.Add(value);
				}
			}

			return list;
		}

		public static List<object> GetStepId_StdStep(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> stepIdList = new List<object>();

			var dt = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(resultItem);
			foreach (var row in dt)
			{
				string value = row.STD_STEP_ID;

				if (!String.IsNullOrEmpty(value) && stepIdList.Contains(value) == false)
					stepIdList.Add(value);
			}

			return stepIdList;
		}

		public static List<object> GetEqpModel_Equipment(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> eqpModelList = new List<object>();

			var dt = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(resultItem);
			foreach (var row in dt)
			{
				string value = row.EQP_MODEL;

				if (!String.IsNullOrEmpty(value) && eqpModelList.Contains(value) == false)
					eqpModelList.Add(value);
			}

			return eqpModelList;
		}

		//public static List<object> GetMask_Output(IExperimentResultItem resultItem, string shopId = null)
		//{
		//	if (resultItem == null)
		//		return null;

		//	List<object> maskList = new List<object>();

		//	var dt = MyHelper.DATASVC.GetEntityData<EngInputs.Tool>(resultItem);
		//	foreach (var row in dt.Distinct())
		//	{
		//		if (String.IsNullOrEmpty(row.TOOL_ID))
		//			continue;
		//		if (!String.IsNullOrEmpty(row.TOOL_TYPE) && row.TOOL_TYPE != "MASK")
		//			continue;

		//		if (string.IsNullOrEmpty(shopId) || shopId == row.SHOP_ID)
		//		{
		//			string value = row.TOOL_ID;

		//			if (!String.IsNullOrEmpty(value) && maskList.Contains(value) == false)
		//				maskList.Add(value);
		//		}
		//	}

		//	return maskList;
		//}

		public static List<object> GetEqpId_Equipment(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> eqpList = new List<object>();

			var dt = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(resultItem);
			foreach (var row in dt.Distinct())
			{
				string value = row.EQP_ID;

				if (!String.IsNullOrEmpty(value) && eqpList.Contains(value) == false)
					eqpList.Add(value);
			}

			return eqpList;
		}

		public static List<object> GetEqpGroup_Equipment(IExperimentResultItem resultItem)
		{
			if (resultItem == null)
				return null;

			List<object> eqpGrpList = new List<object>();

			var dt = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(resultItem);
			foreach (var row in dt.Distinct())
			{
				string value = row.EQP_GROUP;

				if (!String.IsNullOrEmpty(value) && eqpGrpList.Contains(value) == false)
					eqpGrpList.Add(value);
			}

			return eqpGrpList;
		}

		//public static List<object> GetDspEqpGroup_Output(IExperimentResultItem resultItem)
		//{
		//	if (resultItem == null)
		//		return null;

		//	List<object> eqpGrpList = new List<object>();

		//	var dt = MyHelper.DATASVC.GetEntityData<EngInputs.Eqp>(resultItem);
		//	foreach (var row in dt.Distinct())
		//	{
		//		string value = row.DSP_EQP_GROUP_ID;

		//		if (!String.IsNullOrEmpty(value) && eqpGrpList.Contains(value) == false)
		//			eqpGrpList.Add(value);
		//	}

		//	return eqpGrpList;
		//}
	}
}
