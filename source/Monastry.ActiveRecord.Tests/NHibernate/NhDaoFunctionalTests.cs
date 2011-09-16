using System;
using System.Linq;
using Monastry.ActiveRecord.Tests.Model;
using Monastry.ActiveRecord.Tests.Tools;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.NHibernate
{
	[TestFixture]
	public class NhDaoFunctionalTests : NUnitInMemoryTest
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

			mapper.Class<AssignedSoftware>(map =>
			{
				map.Id(s => s.Key, o => o.Generator(Generators.Assigned));
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

		private INhConversation conv;
		private INhConversationContext context;
		private INhDao<Software> dao;

		public override void Setup()
		{
			base.Setup();
			context = new NhConversationContext();
			conv = new NhConversation(sessionFactory, context);
			context.SetDefaultConversation(conv);
			dao = new NhDao<Software>(context);
		}

		public override void Teardown()
		{
			if (conv != null)
			{
				context.UnsetDefaultConversation();
				conv.Dispose();
				conv = null;
			}
			base.Teardown();
		}


		[Test]
		public void CanSaveSoftware()
		{
			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);

			conv.Commit();
			int count = 0;
			conv.Execute(s => count = s.Query<Software>().Where(ar => ar.Name == "ActiveRecord").Count());
			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void CanFindSavedSoftware()
		{
			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);
			var id = sw.Id;

			var sw2 = dao.Find(id);

			Assert.That(sw2.Id, Is.EqualTo(sw.Id));
			Assert.That(sw2.Name, Is.EqualTo(sw.Name));
		}

		[Test]
		public void CanPeekSavedSoftware()
		{
			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);
			var id = sw.Id;

			var sw2 = dao.Peek(id);

			Assert.That(sw2.Id, Is.EqualTo(sw.Id));
			Assert.That(sw2.Name, Is.EqualTo(sw.Name));
		}

		[Test]
		public void CanQuerySavedSoftware()
		{
			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);

			var query = from soft in dao.Linq()
						where soft.Name == "ActiveRecord"
						select soft;
			Assert.That(query.Count(), Is.EqualTo(1));

			var sw2 = query.First();
			Assert.That(sw2.Id, Is.EqualTo(sw.Id));
			Assert.That(sw2.Name, Is.EqualTo(sw.Name));
		}

		[Test]
		public void CanSaveAssignedSoftware()
		{
			var sw = new AssignedSoftware { Key = "AR", Name = "ActiveRecord" };

			INhDao<AssignedSoftware> aDao = new NhDao<AssignedSoftware>(context);
			aDao.Add(sw);

			conv.Commit();
			int count = 0;
			conv.Execute(s => count = s.Query<AssignedSoftware>().Where(ar => ar.Key == "AR").Count());
			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void CanSaveAndUpdateAssignedSoftware()
		{
			var sw = new AssignedSoftware { Key = "AR", Name = "ActiveRecord" };

			INhDao<AssignedSoftware> aDao = new NhDao<AssignedSoftware>(context);
			aDao.Add(sw);
			conv.Commit();

			conv.Restart();
			sw.Name = "ActiveRecord vNext";
			aDao.Replace(sw);
			conv.Commit();

			int count = 0;
			conv.Execute(s => count = s.Query<AssignedSoftware>().Where(ar => ar.Name.Contains("vNext")).Count());
			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void CanDeleteSoftware()
		{
			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);
			conv.Commit();

			dao.Delete(sw);
			conv.Commit();

			int count = 0;
			conv.Execute(s => count = s.Query<Software>().Count());
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void CanAttachDetachSoftware()
		{
			Guid id;
			int count = 0;

			var sw = new Software { Name = "ActiveRecord" };
			dao.Save(sw);
			id = sw.Id;
			conv.Commit();

			conv.Execute(s => count = s.Query<Software>().Where(e => e.Name == "ActiveRecord").Count());
			Assert.That(count, Is.EqualTo(1));

			var sw2 = dao.Find(id);
			sw2.Name = "ActiveRecord vNext";
			dao.Forget(sw2);
			conv.Commit();

			conv.Execute(s => count = s.Query<Software>().Where(e => e.Name == "ActiveRecord").Count());
			Assert.That(count, Is.EqualTo(1));

			dao.Save(sw2);
			conv.Commit();

			conv.Execute(s => count = s.Query<Software>().Where(e => e.Name == "ActiveRecord vNext").Count());
			Assert.That(count, Is.EqualTo(1));
		}
	}
}
