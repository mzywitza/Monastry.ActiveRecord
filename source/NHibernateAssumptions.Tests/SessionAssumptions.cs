using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Monastry.ActiveRecord.Testing;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using FluentNHibernate.Testing;
using NHibernate.Linq;
using NHibernate;

namespace NHibernateAssumptions.Tests
{
    [TestFixture]
    public class SessionAssumptions : NUnitInMemoryTest
    {
        protected override void Mapping(MappingConfiguration config)
        {
            config.FluentMappings.Add<SoftwareMapping>();
        }

        [Test]
        public void PersistenceSpecification()
        {
            new PersistenceSpecification<Software>(sessionFactory.OpenSession())
                .CheckProperty(e => e.Name, "FooBar")
                .VerifyTheMappings();
        }

        [Test]
        public void DirtyEntitiesCanBeCommittedLater()
        {
            var software = new Software { Name = "Foo" };
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(software);
                    tx.Commit();
                }
            }

            software = null;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    software = session.Query<Software>().First();
                }
                Assert.That(software, Is.Not.Null);
                Assert.That(software.Name, Is.EqualTo("Foo"));

                software.Name = "Bar";

                Assert.That(software.Name, Is.EqualTo("Bar"));

                using (var tx = session.BeginTransaction())
                    tx.Commit();

            }

            software = null;
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
                software = session.Query<Software>().First();

            Assert.That(software.Name, Is.EqualTo("Bar"));

        }

        [Test]
        public void DirtyEntitiesAreNotFlushedBeforeCommit()
        {
            var software = new Software { Name = "Foo" };
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(software);
                    tx.Commit();
                }
            }

            software = null;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    software = session.Query<Software>().First();
                }
                Assert.That(software, Is.Not.Null);
                Assert.That(software.Name, Is.EqualTo("Foo"));

                software.Name = "Bar";

                Assert.That(software.Name, Is.EqualTo("Bar"));

                var all = session.Query<Software>().ToList();

                //using (var tx = session.BeginTransaction())
                //    tx.Commit();

            }

            software = null;
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
                software = session.Query<Software>().First();

            Assert.That(software.Name, Is.EqualTo("Foo"));

        }

        [Test]
        public void DifferentSessionsHoldDifferentObjectsAndNoAutomaticFlushOccurs()
        {
            var software = new Software { Name = "Foo" };
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(software);
                    tx.Commit();
                }
            }

            software = null;

            ISession session1 = null, session2 = null;
            try
            {
                session1 = sessionFactory.OpenSession();
                session2 = sessionFactory.OpenSession();

                Software software1, software2;
                using (session1.BeginTransaction())
                    software1 = session1.Query<Software>().First();
                using (session2.BeginTransaction())
                    software2 = session2.Query<Software>().First();

                Assert.That(software1.Id,Is.EqualTo(software2.Id));
                Assert.That(software1, Is.Not.SameAs(software2));

                software2.Name = "Bar";

                using (session2.BeginTransaction())
                    Assert.That(session2.Query<Software>().Where(s=>s.Name == "Bar").Count(), Is.EqualTo(1)); // flush must occur

                using (session1.BeginTransaction())
                    session1.Refresh(software1);

                Assert.That(software1.Name, Is.EqualTo("Foo"));
            }
            finally
            {
                if (session1 != null) session1.Dispose();
                if (session2 != null) session2.Dispose();
            }

        }
    }

    public class Software
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }

    public class SoftwareMapping : ClassMap<Software>
    {
        public SoftwareMapping()
        {
            Id(e => e.Id).GeneratedBy.GuidComb();
            Map(e => e.Name).Not.Nullable().Unique();
        }
    }
}
