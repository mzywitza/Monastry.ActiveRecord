﻿using System.Linq;
using Castle.Windsor;
using Monastry.ActiveRecord.Extensions;
using Monastry.ActiveRecord.Tests.Model;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.Core
{
	[TestFixture]
	public class QueryInstallationTests
	{
		private IWindsorContainer container;

		[SetUp]
		public void CreateContainer()
		{
			container = new WindsorContainer();
		}

		[Test]
		public void GenericInstallationMethodWorks()
		{
			container.InstallQuery<MockQuery>();
			Assert.That(QueryIsInstalled<IMockQuery>());
		}

		[Test]
		public void ParamsInstallationMethodWorks()
		{
			container.InstallQueries(typeof(MockQuery));
			Assert.That(QueryIsInstalled<IMockQuery>());
		}

		[Test]
		public void EnumerationInstallationMethodWorks()
		{
			(new[] { typeof(MockQuery) }).AsEnumerable().InstallInto(container);
			Assert.That(QueryIsInstalled<IMockQuery>());
		}

		[Test]
		public void InstallAllQueriesWorks()
		{
			container.InstallAllQueries();
			Assert.That(QueryIsInstalled<IMockQuery>());
		}

		[Test]
		public void InstallAllQueriesWorksWithAssembly()
		{
			container.InstallAllQueries(typeof(MockQuery).Assembly);
			Assert.That(QueryIsInstalled<IMockQuery>());
		}

		private bool QueryIsInstalled<TQuery>()
		{
			return container.Kernel.GetAssignableHandlers(typeof(TQuery)).Length > 0;
		}
	}
}
