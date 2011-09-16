using System;
using System.Linq;
using Monastry.ActiveRecord.Tests.Tools;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.Conversation
{
	[TestFixture]
	public class NhConversionFunctionalTests : NUnitInMemoryTest
	{
		protected override void Mapping(Configuration config)
		{
			var mapper = new ModelMapper();
			mapper.Class<Software>(map =>
			{
				map.Id(s => s.Id, o => o.Generator(Generators.GuidComb));
				map.Property(s => s.Name, o =>
				{
					o.NotNullable(true);
					o.Unique(true);
				});
			});
			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			config.AddMapping(mapping);
			config.DataBaseIntegration(db => db.LogSqlInConsole = true);
		}

		private INhConversation conv = null;
		private INhConversationContext context = new NhConversationContext();

		public override void Setup()
		{
			base.Setup();
			conv = new NhConversation(sessionFactory, context);
		}

		public override void Teardown()
		{
			if (conv != null)
			{
				conv.Dispose();
				conv = null;
			}
			base.Teardown();
		}

		[Test]
		public void CanCallIntoSession()
		{
			ISession session = null;
			conv.Execute(s => session = s);
			Assert.That(session, Is.Not.Null);
		}

		[Test]
		public void SessionIsWrappedInTransaction()
		{
			conv.Execute(session =>
			{
				Assert.That(session, Is.Not.Null);
				Assert.That(session.Transaction.IsActive);
			});
		}

		[Test]
		public void SessionIsFlushedAutomatically()
		{
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().First().Name, Is.EqualTo("Foo")));
		}

		[Test]
		public void SessionIsNotFlushedWhenCommitModeIsExplicit()
		{
			conv.CommitMode = ConversationCommitMode.Explicit;
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().Count(), Is.EqualTo(0)));
		}

		[Test]
		public void SessionIsFlushedWhenCommitModeIsOnClose()
		{
			conv.CommitMode = ConversationCommitMode.OnClose;
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().Count(), Is.EqualTo(1)));
		}

		[Test]
		public void SessionIsFlushedWhenCommitted()
		{
			conv.CommitMode = ConversationCommitMode.Explicit;
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Commit();
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().Count(), Is.EqualTo(1)));
		}

		[Test]
		public void SessionIsFlushedWhenCommittedInOnCloseMode()
		{
			conv.CommitMode = ConversationCommitMode.OnClose;
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Commit();
			conv.Cancel();
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().Count(), Is.EqualTo(1)));
		}

		[Test]
		public void SessionIsNotFlushedWhenCanceledInOnCloseMode()
		{
			conv.CommitMode = ConversationCommitMode.OnClose;
			conv.Execute(s => s.Save(new Software { Name = "Foo" }));
			conv.Cancel();
			conv.Restart(); // Open new session
			conv.Execute(s => Assert.That(s.Query<Software>().Count(), Is.EqualTo(0)));
		}

	}

	public class Software
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

}
