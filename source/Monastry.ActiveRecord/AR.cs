using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace Monastry.ActiveRecord
{
    public class AR
    {
        protected internal static IWindsorContainer Container {get; private set;}

        public static void Install(IWindsorContainer container)
        {
            if (!IsConfigured(container))
                throw new ArgumentException(
                    "The container is not correctly configured. Please make sure that at " +
                    "least the following services are included in installing the container " +
                    "IConversation, IConversationContext, IDao", "container");
            Container = container;
        }

        public static void Install(IActiveRecordInstaller installer)
        {
            AR.Install(installer.GetConfiguredContainer());
        }

        private static bool IsConfigured(IWindsorContainer container)
        {
            return
                (container.Kernel.GetAssignableHandlers(typeof(IDao<>)).Length > 0) &&
                (container.Kernel.GetAssignableHandlers(typeof(IConversation)).Length > 0) &&
                (container.Kernel.GetAssignableHandlers(typeof(IConversationContext)).Length > 0);
        }
    }
}
