using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.SeePlan; 

namespace SmartAPS
{
    public class CalendarValue
    {
        #region Variables

        private Type type;
        private dynamic value;

        #endregion

        #region Properties

        public DateTime EffectiveStartTime { get; private set; }
        public DateTime EffectiveEndTime { get; private set; }

        public dynamic Value
        {
            get { return SAPSUtils.GetTValue(this.value, this.type); }
        }

        #endregion

        #region Constructors

        public CalendarValue(
            DateTime startTime,
            DateTime endTime,
            Type type,
            dynamic value)
        {
            this.type = type;
            this.value = value;

            this.EffectiveStartTime = startTime;
            this.EffectiveEndTime = endTime;
        }

        #endregion

        #region Methods

        public bool IsEffectiveTime(DateTime targetTime)
        {
            return SAPSUtils.IsEffectiveTime(targetTime, this.EffectiveStartTime, this.EffectiveEndTime);
        }

        public override string ToString()
        {
            return string.Concat(EffectiveStartTime.DbToDateString(), "~", EffectiveEndTime.DbToDateString(), " : ", Value.ToString());
        }

        public int Compare(CalendarValue x, CalendarValue y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            int cmp = 0;
            if (cmp == 0)
                cmp = x.EffectiveStartTime.CompareTo(y.EffectiveStartTime);

            return cmp;
        }


        #endregion

    }
}
