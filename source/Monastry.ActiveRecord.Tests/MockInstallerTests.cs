using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Monastry.ActiveRecord.Testing.Mocks;
using Rhino.Mocks;
using Monastry.ActiveRecord.Extensions;

namespace Monastry.ActiveRecord.Tests.Mocking
{
	[TestFixture]
	public class MockInstallerTests
	{

		[Test]
		public void InstallerConfiguresTheContainer()
		{
			var mock = new MockInstaller();
			var container = mock.GetConfiguredContainer();

			Assert.That(container.Kernel.GetAssignableHandlers(typeof(IDao<>)), Is.Not.Empty);
			Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversation)), Is.Not.Empty);
			Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)), Is.Not.Empty);
		}

		[Test]
		public void StrictMockingIsAvailable()
		{
			var mock = new MockInstaller(true);
			var container = mock.GetConfiguredContainer();

			var dao = container.Resolve<IDao<Software>>();
			var ex = Assert.Throws<InvalidOperationException>(() => dao.Save(new Software()));
			Assert.That(ex.Message.ToLower(), Contains.Substring("unexpected"));
			Assert.That(ex.Message.ToLower(), Contains.Substring("strict"));
		}

		[Test]
		public void AdditionalSetupIsCalled()
		{
			var wasCalled = false;
			var mock = new MockInstaller();

			mock.AdditionalConfig = c => { wasCalled = true; };
			mock.GetConfiguredContainer();

			Assert.That(wasCalled);
		}

		[Test]
		public void CustomDaosCanBeAdded()
		{
			var guid = Guid.NewGuid();
			var software = new Software{Id=guid, Name="FooBar"};

			var mock = new MockInstaller(true);
			var dao = MockRepository.GenerateMock<IDao<Software>>();
			dao.Expect(d => d.Find(guid)).Return(software);
			mock.RegisterDaoDouble(dao);
			var container = mock.GetConfiguredContainer();
			
			Assert.That(container.Resolve<IDao<Software>>().Find(guid), Is.SameAs(software));

			var ex = Assert.Throws<InvalidOperationException>(
				() => container.Resolve<IDao<Installation>>().Find(guid));
			Assert.That(ex.Message.ToLower(), Contains.Substring("unexpected"));
			Assert.That(ex.Message.ToLower(), Contains.Substring("strict"));
		}

		[Test]
		public void CustomConversationCanBeSet()
		{
			var mock = new MockInstaller(true);
			var conv = MockRepository.GenerateMock<IConversation>();
			conv.Expect(c => c.Commit());

			mock.RegisterConversationDouble(conv);

			var container = mock.GetConfiguredContainer();

			container.Resolve<IConversation>().Commit();

			conv.VerifyAllExpectations();
		}

		[Test]
		public void FullTest()
		{
			// Arrange
			var guid = Guid.NewGuid();
			var software = new Software { Id = Guid.Empty, Name = "FooBar" };

			var mock = new MockInstaller(true);
			
			var dao = MockRepository.GenerateMock<IDao<Software>>();
			dao.Expect(d => d.Save(software)).WhenCalled(i=>{software.Id = guid;});
			dao.Expect(d => d.Find(guid)).Return(software);
			mock.RegisterDaoDouble(dao);

			var conv = MockRepository.GenerateMock<IConversation>();
			conv.Expect(c => c.Execute(Arg<Action>.Is.Anything)).WhenCalled(i => ((Action)i.Arguments[0]).Invoke());
			conv.Expect(c => c.Commit());
			mock.RegisterConversationDouble(conv);

			AR.Install(mock);
			// Act
			using (var conversation = AR.StartConversation())	
			{
				conv.Execute(() => software.Save());
				conv.Commit();
				Assert.That(AR.Find<Software>(guid), Is.SameAs(software));
			}

			//Assert
			dao.VerifyAllExpectations();
			conv.VerifyAllExpectations();
		}
	}

	public class Software : IActiveRecordObject
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class Installation : IActiveRecordObject
	{
		public Guid Id { get; set; }
		public string Computer { get; set; }
		public Software Software { get; set; }
	}
}
