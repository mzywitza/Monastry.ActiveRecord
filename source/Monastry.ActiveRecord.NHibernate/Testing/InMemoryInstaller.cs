using System;
using Castle.Windsor;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace Monastry.ActiveRecord.Testing
{
	public class InMemoryInstaller : NhActiveRecordInstallerBase
	{
		private static Configuration nhConfiguration = null;
		private static Action<Configuration> mapping = null;
		private static Action<IWindsorContainer> additionalSetup = null;

		public static Action<Configuration> Mapping
		{
			get { return mapping; }
			set
			{
				mapping = value;
				nhConfiguration = null;
			}
		}

		public static Action<IWindsorContainer> AdditionalSetup
		{
			get { return additionalSetup; }
			set { additionalSetup = value; }
		}

		public override Configuration GetNhConfiguration()
		{
			if (nhConfiguration == null)
				nhConfiguration = CreateConfiguration();
			return nhConfiguration;
		}

		public override void AddCustomConfiguration(IWindsorContainer container)
		{
			if (additionalSetup != null)
				additionalSetup.Invoke(container);
		}

		private static Configuration CreateConfiguration()
		{
			if (mapping == null)
				throw new InvalidOperationException("Mapping is not set. A Configuration for in-memory-databases cannot be created without mapping delegate.");
			var configuration = new Configuration();
			configuration.DataBaseIntegration(
				db =>
				{
					db.Dialect<SQLiteDialect>();
					db.ConnectionProvider<InMemoryConnectionProvider>();
					db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
				});
			mapping.Invoke(configuration);
			return configuration;
		}

		public override IWindsorContainer GetConfiguredContainer()
		{
			InMemoryConnectionProvider.CloseDatabase();
			var container = base.GetConfiguredContainer();
			new SchemaExport(nhConfiguration).Create(false, true);
			return container;
		}
	}
}
