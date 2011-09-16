using System.Linq;
using Castle.Windsor;
using Monastry.ActiveRecord.Testing;
using Monastry.ActiveRecord.Tests.Model;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.TestSupport
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

}
