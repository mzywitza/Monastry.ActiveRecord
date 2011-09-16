using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace Monastry.ActiveRecord.Tests.Tools
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
            configuration = new Configuration();
            configuration.DataBaseIntegration(
                db=>{
                    db.Dialect<SQLiteDialect>();
                    db.ConnectionProvider<InMemoryConnectionProvider>();
                    db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                });
            Mapping(configuration);
            sessionFactory = configuration.BuildSessionFactory();
        }

        protected abstract void Mapping(Configuration config);

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
