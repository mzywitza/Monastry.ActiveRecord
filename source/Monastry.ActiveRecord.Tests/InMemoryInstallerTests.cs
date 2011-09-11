using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Monastry.ActiveRecord.Testing.NHibernate;
using Castle.Windsor;

namespace Monastry.ActiveRecord.Tests
{
	[TestFixture]
	public class InMemoryInstallerTests
	{
		[Test]
		public void InstallerCallsMappingAction()
		{
			bool called = false;
			InMemoryInstaller.Mapping = c => called = true;
			new InMemoryInstaller().GetConfiguredContainer();

			Assert.That(called, Is.True);
		}

		[Test]
		public void InstallerCallsAdditionalSetupAction()
		{
			IWindsorContainer container = null; 
			InMemoryInstaller.Mapping = c => { };
			InMemoryInstaller.AdditionalSetup = c => container = c;
			var container2 = new InMemoryInstaller().GetConfiguredContainer();

			Assert.That(container, Is.SameAs(container2));
		}

		[Test]
		public void InstallerThrowsWhenNoMapperIsAvailable()
		{
			InMemoryInstaller.Mapping = null;

			var e = Assert.Throws<InvalidOperationException>(() => new InMemoryInstaller().GetConfiguredContainer());
			Assert.That(e.Message, Contains.Substring("Mapping"));
		}
	}
}
