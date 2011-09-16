using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Monastry.ActiveRecord.Tests.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.Core
{
	[TestFixture]
	public class InstallationTests
	{
		[Test]
		public void InstallationSucceedsForCorrectContainer()
		{
			AR.Install(DummyFactory.CreateContainer());
		}

		[Test]
		public void InstallationFailsWithoutDao()
		{
			var container = new WindsorContainer();
			container.Register(
				Component.For<IConversation>().ImplementedBy<DummyConversation>(),
				Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
			var ex = Assert.Throws<ArgumentException>(() => AR.Install(container));
			Assert.That(ex.Message, Contains.Substring("IDao"));
		}

		[Test]
		public void InstallationFailsWithoutConversation()
		{
			var container = new WindsorContainer();
			container.Register(
				Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
				Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
			var ex = Assert.Throws<ArgumentException>(() => AR.Install(container));
			Assert.That(ex.Message, Contains.Substring(typeof(IConversation).Name));
		}

		[Test]
		public void InstallationFailsWithoutConversationContext()
		{
			var container = new WindsorContainer();
			container.Register(
				Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
				Component.For<IConversation>().ImplementedBy<DummyConversation>());
			var ex = Assert.Throws<ArgumentException>(() => AR.Install(container));
			Assert.That(ex.Message, Contains.Substring(typeof(IConversationContext).Name));
		}

		[Test]
		public void InstallationSetsUsageOnInstaller()
		{
			var container = DummyFactory.CreateContainer();
			var installer = MockRepository.GenerateStrictMock<IActiveRecordInstaller>();
			installer.Expect(i => i.Usage).SetPropertyWithArgument(Usage.Web);
			installer.Expect(i => i.GetConfiguredContainer()).Return(container);
			AR.Install(installer, usage: Usage.Web);
			installer.VerifyAllExpectations();
		}

		[Test]
		public void InstallationUsesCommitMode()
		{
			var container = DummyFactory.CreateContainer();
			var installer = MockRepository.GenerateStrictMock<IActiveRecordInstaller>();
			installer.Expect(i => i.CommitMode).SetPropertyWithArgument(ConversationCommitMode.Explicit);
			installer.Expect(i => i.GetConfiguredContainer()).Return(container);
			AR.Install(installer, commitMode: ConversationCommitMode.Explicit);
			installer.VerifyAllExpectations();
		}
	}

}
