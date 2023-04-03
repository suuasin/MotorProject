using System;

namespace SmartAPS.UserLibrary.Helper
{
    public class ShiftTimeHelper
    {
        private static bool IsInit = false;

        private static Mozart.SeePlan.FactoryTimeInfo SeePlanInfo;

        private static Mozart.Studio.TaskModel.UserLibrary.FactoryTimeInfo UserLibInfo;

        public static void Initialize()
        {
            //if (ShiftTimeHelper.IsInit)
            //    return;


            SetFactoryConfiguration_SeePlan();
            SetFactoryConfiguration_UserLibrary();

            //ShiftTimeHelper.IsInit = true;
        }

        public static void SetFactoryStartTime(TimeSpan timeOffset)
        {
            UserLibInfo.StartOffset = SeePlanInfo.StartOffset = timeOffset;

            var seePlanConfig = Mozart.SeePlan.FactoryConfiguration.Current;
            seePlanConfig.TimeInfo = SeePlanInfo;

            var userLibConfig = Mozart.Studio.TaskModel.UserLibrary.FactoryConfiguration.Current;
            userLibConfig.TimeInfo = UserLibInfo;
        }

        private static void SetFactoryConfiguration_SeePlan()
        {
            try
            {
                var curr = Mozart.SeePlan.FactoryConfiguration.Current;

                Mozart.SeePlan.FactoryTimeInfo info = new Mozart.SeePlan.FactoryTimeInfo();

                info.Default = false;
                info.Name = null;
                info.ShiftNames = new string[] { "A" };
                info.StartOffset = TimeSpan.FromHours(0);
                info.ShiftHours = 24;
                info.StartOfWeek = DayOfWeek.Monday;

                curr.TimeInfo = SeePlanInfo = info;
            }
            catch { }
        }

        private static void SetFactoryConfiguration_UserLibrary()
        {
            try
            {
				var curr = Mozart.Studio.TaskModel.UserLibrary.FactoryConfiguration.Current;

				Mozart.Studio.TaskModel.UserLibrary.FactoryTimeInfo info = new Mozart.Studio.TaskModel.UserLibrary.FactoryTimeInfo();

                info.Default = false;
                info.Name = null;
                info.ShiftNames = new string[] { "A" };
                info.StartOffset = TimeSpan.FromHours(0);
                info.ShiftHours = 24;
                info.StartOfWeek = DayOfWeek.Monday;

                curr.TimeInfo = UserLibInfo = info;
			}
            catch { }
        }
    }
}
