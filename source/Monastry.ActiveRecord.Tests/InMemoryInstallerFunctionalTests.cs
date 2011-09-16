using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Monastry.ActiveRecord.Testing;
using NHibernate.Mapping.ByCode;
using Castle.Windsor;

namespace Monastry.ActiveRecord.Tests.IMM
{
	[TestFixture]
	public class InMemoryInstallerFunctionalTests
	{
		private IWindsorContainer container;

		[TestFixtureSetUp]
		public void CreateMapping()
		{
			InMemoryInstaller.Mapping = configuration =>
				{
					var mapper = new ModelMapper();
					mapper.Class<Software>(m => { m.Id(e => e.Id); m.Property(e => e.Name); });
					configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
				};
		}

		[SetUp]
		public void InitializeAR()
		{
			InMemoryInstaller.AdditionalSetup = c => container = c;
			AR.Install(new InMemoryInstaller());
		}

		[Test]
		public void SmokeTest()
		{
			// resolving those is only needed when AR is not used...
			var dao = container.Resolve<IDao<Software>>();
			var c = container.Resolve<IConversation>();
			var cc = container.Resolve<IConversationContext>();
			cc.SetDefaultConversation(c);
		
			Assert.That(dao, Is.Not.Null);
			var software = new Software { Name = "ActiveRecord" };

			dao.Save(software);

			c.Commit();

			var loaded = dao.Linq().Where(s => s.Name == "ActiveRecord").First();
			Assert.That(loaded.Id, Is.EqualTo(software.Id));

			dao.Delete(software);

			c.Commit();

			Assert.That(dao.Linq().Count(), Is.EqualTo(0));
		}
	}

	public class Software
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
