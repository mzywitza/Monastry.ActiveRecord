using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Monastry.ActiveRecord.Extensions;
using Monastry.ActiveRecord.Tests.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.Core
{
	[TestFixture]
	public class ArApiTests
	{
		// StartDefaultConversation - Set implicit conversation
		// EndDefaultConversation - Unset implicit conversation
		// StartConversation - New IConversation for Gui-Mode
		// Current Conversation support
		// - Commit
		// - Cancel
		// - Restart
		// Entity API
		// - Save/Add/Replace
		// - Delete/Forget
		// Type API
		// - Find/Peek
		// - Linq
		// Service Lookup
		// - Query
		// - Dao
		// - Service

		private MockContainer mocks;

		[SetUp]
		public void Install()
		{
			mocks = new MockContainer();
			AR.Install(mocks.Container);
		}

		[TearDown]
		public void Verify()
		{
			mocks.Verify();
			mocks = null;
		}

		// StartDefaultConversation - Set implicit conversation
		[Test]
		public void StartDefaultConversation()
		{
			mocks.Context.Expect(cc => cc.SetDefaultConversation(Arg<IConversation>.Is.Same(mocks.Conversation)));
			AR.StartDefaultConversation();
		}

		// EndDefaultConversation - Unset implicit conversation
		[Test]
		public void EndDefaultConversation()
		{
			mocks.Context.Expect(cc => cc.DefaultConversation).Return(mocks.Conversation);
			mocks.Context.Expect(cc => cc.UnsetDefaultConversation());
			AR.EndDefaultConversation();
		}

		// StartConversation - New IConversation for Gui-Mode
		[Test]
		public void StartConversation()
		{
			var c = AR.StartConversation();
			Assert.That(c, Is.SameAs(mocks.Conversation));
		}

		// Current Conversation support - Commit
		[Test]
		public void Commit()
		{
			mocks.Context.Expect(cc => cc.CurrentConversation).Return(mocks.Conversation);
			mocks.Conversation.Expect(c => c.Commit());
			AR.Commit();
		}

		// Current Conversation support - Cancel
		[Test]
		public void Cancel()
		{
			mocks.Context.Expect(cc => cc.CurrentConversation).Return(mocks.Conversation);
			mocks.Conversation.Expect(c => c.Cancel());
			AR.Cancel();
		}

		// Current Conversation support - Restart
		[Test]
		public void Restart()
		{
			mocks.Context.Expect(cc => cc.CurrentConversation).Return(mocks.Conversation);
			mocks.Conversation.Expect(c => c.Restart());
			AR.Restart();
		}

		// Service Lookup - Query
		[Test]
		public void Query()
		{
			mocks.Container.Register(Component.For<IMockQuery>().ImplementedBy<MockQuery>());
			var q = AR.Query<IMockQuery>();
			Assert.That(q, Is.AssignableTo<IMockQuery>());
		}

		// Service Lookup - Dao
		[Test]
		public void Dao()
		{
			var d = AR.Dao<Software>();
			Assert.That(d, Is.SameAs(mocks.Dao));
		}

		// Service Lookup - Service
		[Test]
		public void Service()
		{
			mocks.Container.Register(Component.For<IMockQuery>().ImplementedBy<MockQuery>());
			var q = AR.Service<IMockQuery>();
			Assert.That(q, Is.AssignableTo<IMockQuery>());
		}

		// Entity API - Save
		[Test]
		public void Save()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Save(software));
			AR.Save(software);
		}

		// Entity API - Add
		[Test]
		public void Add()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Add(software));
			AR.Add(software);
		}

		// Entity API - Replace
		[Test]
		public void Replace()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Replace(software));
			AR.Replace(software);
		}

		// Entity API - Delete
		[Test]
		public void Delete()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Delete(software));
			AR.Delete(software);
		}

		// Entity API - Forget
		[Test]
		public void Forget()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Forget(software));
			AR.Forget(software);
		}

		// Type API - Find
		[Test]
		public void Find()
		{
			var id = Guid.NewGuid();
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Find(id)).Return(software);
			Assert.That(AR.Find<Software>(id), Is.SameAs(software));
		}

		// Type API - Peek
		[Test]
		public void Peek()
		{
			var id = Guid.NewGuid();
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Peek(id)).Return(software);
			Assert.That(AR.Peek<Software>(id), Is.SameAs(software));
		}

		// Type API - Linq
		[Test]
		public void Linq()
		{
			var software = new[] { new Software { Name = "ActiveRecord" } }.AsQueryable();
			mocks.Dao.Expect(d => d.Linq()).Return(software);
			Assert.That(AR.Linq<Software>(), Is.SameAs(software));
		}

		// Extensions API-Save
		[Test]
		public void ExtSave()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Save(software));
			software.Save();
		}

		// Extensions API - Add
		[Test]
		public void ExtAdd()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Add(software));
			software.Add();
		}

		// Extensions API - Replace
		[Test]
		public void ExtReplace()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Replace(software));
			software.Replace();
		}

		// Extensions API - Delete
		[Test]
		public void ExtDelete()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Delete(software));
			software.Delete();
		}

		// Extensions API - Forget
		[Test]
		public void ExtForget()
		{
			var software = new Software { Name = "ActiveRecord" };
			mocks.Dao.Expect(d => d.Forget(software));
			software.Forget();
		}
	}

	class MockContainer
	{
		public IConversation Conversation;
		public IConversationContext Context;
		public IDao<Software> Dao;
		public IWindsorContainer Container;

		public MockContainer()
		{
			Container = new WindsorContainer();
			Dao = MockRepository.GenerateStrictMock<IDao<Software>>();
			Conversation = MockRepository.GenerateStrictMock<IConversation>();
			Context = MockRepository.GenerateStrictMock<IConversationContext>();
			Container.Register(
				Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
				Component.For<IDao<Software>>().Instance(Dao),
				Component.For<IConversation>().Instance(Conversation),
				Component.For<IConversationContext>().Instance(Context));
		}

		public void Verify()
		{
			Conversation.VerifyAllExpectations();
			Context.VerifyAllExpectations();
			Dao.VerifyAllExpectations();
		}
	}
}
