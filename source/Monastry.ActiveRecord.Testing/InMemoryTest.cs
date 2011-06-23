using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

namespace Monastry.ActiveRecord.Testing
{
    public abstract class InMemoryTest
    {
        public ISessionFactory sessionFactory;
        public Configuration configuration;

        public virtual void Setup()
        {
            if (configuration == null) CreateInMemoryDatabase();
            PrepareInMemoryDatabase();
        }

        public virtual void Teardown()
        {
            ResetInMemoryDatabase();
        }

        protected virtual void CreateInMemoryDatabase()
        {
            configuration = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard
                    .InMemory()
                //.ShowSql()
                    .Provider<InMemoryConnectionProvider>())
                //.Mappings(x => x.FluentMappings.AddFromAssemblyOf<Monastry.Heraldry.Core.Data.ThisAssembly>())
                .Mappings(Mapping)
                .BuildConfiguration();
            sessionFactory = configuration.BuildSessionFactory();
        }

        protected abstract void Mapping(MappingConfiguration config);

        protected virtual void PrepareInMemoryDatabase()
        {
            new SchemaExport(configuration).Create(false, true);
        }


        protected virtual void ResetInMemoryDatabase()
        {
            InMemoryConnectionProvider.CloseDatabase();
        }

        protected virtual void ResetConfiguration()
        {
            ResetInMemoryDatabase();
            configuration = null;
        }

    }
}
