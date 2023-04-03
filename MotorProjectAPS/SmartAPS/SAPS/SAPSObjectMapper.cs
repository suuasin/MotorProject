using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;

namespace SmartAPS
{
    public class SAPSObjectMapper : IModelController
    {
        public static SAPSObjectMapper Instance
        {
            get
            {
#if BUILD_PKG
                CheckInstance();
#endif

                return ServiceLocator.Resolve<SAPSObjectMapper>();
            }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(SAPSObjectMapper); }
        }
        #endregion

        private static void CheckInstance()
        {
            var baseType = typeof(SAPSObjectMapper);
            if (ServiceLocator.IsRegistered(baseType))
                return;

            foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assy.GetTypes())
                {
                    if (type.IsSubclassOf(baseType))
                    {
                        var instance = (IModelController)Activator.CreateInstance(type);
                        ServiceLocator.RegisterController(instance);
                        return;
                    }
                }
            }

        }

        internal static T Create<T>() where T : new()
        {
            if (IsRegistered(typeof(T)))
                return TypeRegistry.Create<T>();
            else
                return new T();
        }
        internal static T Create<T>(params object[] args) where T : class
        {
            var t = TypeRegistry.Resolve(typeof(T)) ?? typeof(T);
            var instance = Activator.CreateInstance(t, args);
            return (T)instance;
        }
        internal static bool IsRegistered(Type type)
        {
            return TypeRegistry.Resolve(type) != null;
        }

        internal static ReplenishEvents CreateReplenishEvents()
        {
            var re = SAPSObjectMapper.Create<ReplenishEvents>();

            return re;
        }

        internal static MatPlan CreateMatPlan()
        {
            var mp = SAPSObjectMapper.Create<MatPlan>();

            return mp;
        }
    }

}
