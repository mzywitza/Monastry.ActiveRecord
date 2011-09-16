using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Monastry.ActiveRecord.Testing;
using Monastry.ActiveRecord.Extensions;
using NHibernate.Mapping.ByCode;
using Monastry.ActiveRecord.Tests.Model;

namespace Monastry.ActiveRecord.Tests.Integration
{
	[TestFixture]
	public class NhIntegrationTest
	{
		private IActiveRecordInstaller installer;
		
		[TestFixtureSetUp]
		public void CreateMapping()
		{
			InMemoryInstaller.Mapping = configuration =>
				{
					var mapper = new ModelMapper();
					mapper.Class<Software>(m => { m.Id(e => e.Id, im=>im.Generator(Generators.GuidComb)); m.Property(e => e.Name); });
					configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
				};
			installer = new InMemoryInstaller();
		}

		[SetUp]
		public void InitializeAR()
		{
			
			AR.Install(installer);
		}
 
		[Test]
		public void SimpleSmokeTest()
		{
			AR.StartDefaultConversation();

			var software = new Software { Name = "FooBar" };
			software.Save();

			AR.Commit();

			var loaded = AR.Linq<Software>().Where(s => s.Name == "FooBar").First();

			// Will be from 1st level cache, hence same object
			Assert.That(loaded, Is.SameAs(software));
			
			AR.EndDefaultConversation();
		}

		[Test]
		public void SmokeTestOnDifferentSessions()
		{
			AR.StartDefaultConversation();

			var software = new Software { Name = "FooBar" };
			software.Save();

			Assert.That(software.Id, Is.Not.EqualTo(Guid.Empty));

			AR.Commit();
			AR.Restart();

			var loaded = AR.Linq<Software>().Where(s => s.Name == "FooBar").First();

			// Will not be from 1st level cache, hence different object
			Assert.That(loaded, Is.Not.SameAs(software));
			Assert.That(loaded.Id, Is.EqualTo(software.Id));

			AR.EndDefaultConversation();
		}

	}
}
