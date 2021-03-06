﻿using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Monastry.ActiveRecord.Extensions;
using NHibernate;
using NHibernate.Cfg;

namespace Monastry.ActiveRecord
{
	public abstract class NhActiveRecordInstallerBase : INhActiveRecordInstaller
	{
		protected IWindsorContainer Container;

		public virtual IWindsorContainer GetConfiguredContainer()
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
					.LifeStyle.Transient
					.DependsOn(Property.ForKey("CommitMode").Eq(CommitMode)),
				Component.For<IConversationContext>()
					.Forward<INhConversationContext>()
					.ImplementedBy<NhConversationContext>()
					.WithUsage(Usage),
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

		private Usage usage = Usage.Simple;
		private ConversationCommitMode commitMode = ConversationCommitMode.Automatic;

		public virtual Usage Usage
		{
			get { return usage; }
			set { usage = value; }
		}


		public virtual ConversationCommitMode CommitMode
		{
			get { return commitMode; }
			set { commitMode = value; }
		}
	}
}
