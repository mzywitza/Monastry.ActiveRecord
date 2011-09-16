using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Monastry.ActiveRecord.Testing
{
	public class MockInstaller : IActiveRecordInstaller
	{
		protected IWindsorContainer Container;
		protected bool StrictMocking;
		protected IConversation UserConversation;

		public MockInstaller()
			: this(false)
		{
		}

		public MockInstaller(bool useStrictMocks)
		{
			Usage = Usage.Simple;
			CommitMode = ConversationCommitMode.Automatic;
			StrictMocking = useStrictMocks;
			Container = new WindsorContainer();
		}

		public IWindsorContainer GetConfiguredContainer()
		{
			Container.Register(
				Component.For(typeof(IDao<>))
					.ImplementedBy(typeof(DaoDouble<>))
					.LifeStyle.Transient
					.DependsOn(Property.ForKey<bool>().Eq(StrictMocking)),
				Component.For<IConversationContext>()
					.ImplementedBy<ConversationContext>(),
				Component.For<IConversation>()
					.ImplementedBy<ConversationDouble>()
					.LifeStyle.Transient
					.DependsOn(Property.ForKey<bool>().Eq(StrictMocking))
					.DependsOn(Property.ForKey<IConversation>().Eq(UserConversation))
				);

			AddCustomConfiguration(Container);
			if (AdditionalConfig != null) AdditionalConfig.Invoke(Container);

			return Container;
		}

		public virtual void AddCustomConfiguration(IWindsorContainer container)
		{
		}

		public Usage Usage { get; set; }
		public ConversationCommitMode CommitMode { get; set; }
		public virtual Action<IWindsorContainer> AdditionalConfig { get; set; }

		public virtual void RegisterDaoDouble<T>(IDao<T> dao) where T : class
		{
			Container.Register(Component.For<IDao<T>>().Instance(dao));
		}

		public virtual void RegisterConversationDouble(IConversation conversation)
		{
			UserConversation = conversation;
		}
	}
}
