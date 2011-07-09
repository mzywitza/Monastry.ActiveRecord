using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using NHibernate;
using NHibernate.Cfg;

namespace Monastry.ActiveRecord
{
    public abstract class NhActiveRecordInstallerBase : INhActiveRecordInstaller
    {
        protected IWindsorContainer Container;

        public IWindsorContainer GetConfiguredContainer()
        {
            if (Container == null)
                Container = CreateContainer();
            return Container;
        }

        private IWindsorContainer CreateContainer()
        {
            var c = new WindsorContainer();
            c.Register(
                Component.For<IConversation>()
                    .Forward<INhConversation>()
                    .ImplementedBy<NhConversation>()
                    .LifeStyle.Transient,
                Component.For<IConversationContext>()
                    .Forward<INhConversationContext>()
                    .ImplementedBy<NhConversationContext>()
                    .LifeStyle.PerThread,
                Component.For<ISessionFactory>()
                    .Instance(GetNhConfiguration().BuildSessionFactory())
                    .LifeStyle.Singleton,
                Component.For(typeof(IDao<>))
                    .Forward(typeof(INhDao<>))
                    .ImplementedBy(typeof(NhDao<>))
                    .LifeStyle.Transient);
            AddCustomConfiguration(c);
            return c;
        }


        public abstract Configuration GetNhConfiguration();


        public virtual void AddCustomConfiguration(IWindsorContainer container)
        {
        }
    }
}
