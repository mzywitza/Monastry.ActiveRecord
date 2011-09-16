using Castle.Windsor;
using Monastry.ActiveRecord.Tests.Tools;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests
{
    [TestFixture]
    public class NhActiveRecordInstallerBaseTests
    {
        [Test]
        public void BaseConfiguresTheContainer()
        {
            var mock = CreateMock();
            var container = mock.GetConfiguredContainer();

            Assert.That(container.Kernel.GetAssignableHandlers(typeof(IDao<>)), Is.Not.Empty);
            Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversation)), Is.Not.Empty);
            Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)), Is.Not.Empty);
            Assert.That(container.Kernel.GetAssignableHandlers(typeof(ISessionFactory)), Is.Not.Empty);
        }

        [Test]
        public void BaseGetsNhConfigurationFromAbstractMethod()
        {
            var mock = CreateMock();
            var container = mock.GetConfiguredContainer();
            Assert.That(mock.GetNhConfigurationCalled, Is.True);
        }

        [Test]
        public void BaseOnlyConstructsTheContainerOnce()
        {
            var mock = CreateMock();
            var container1 = mock.GetConfiguredContainer();
            Assert.That(mock.GetNhConfigurationCalled);
            Assert.That(mock.AddCustomConfigurationCalled);
            mock.AddCustomConfigurationCalled = false;
            mock.GetNhConfigurationCalled = false;
            var container2 = mock.GetConfiguredContainer();
            Assert.That(mock.GetNhConfigurationCalled, Is.False);
            Assert.That(mock.GetNhConfigurationCalled, Is.False);
        }

        [Test]
        public void BaseCallsAddCustomConfiguration()
        {
            var mock = CreateMock();
            mock.GetConfiguredContainer();
            Assert.That(mock.AddCustomConfigurationCalled == true);
        }

        private static InstallerMock CreateMock()
        {
            // Since Configuration is not mockable, we use the in-memory Configuration
            // from Testing.
            Configuration cfg = new Configuration().DataBaseIntegration(
                db =>
                {
                    db.Dialect<SQLiteDialect>();
                    db.ConnectionProvider<InMemoryConnectionProvider>();
                    db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                });
            return new InstallerMock
                {
                    ConfigurationToUse = cfg,
                    AddCustomConfigurationCalled = false,
                    GetNhConfigurationCalled = false
                };
        }
    }

    class InstallerMock : NhActiveRecordInstallerBase
    {
        public bool GetNhConfigurationCalled { get; set; }
        public bool AddCustomConfigurationCalled { get; set; }
        public Configuration ConfigurationToUse { get; set; }

        public override Configuration GetNhConfiguration()
        {
            GetNhConfigurationCalled = true;
            return ConfigurationToUse;
        }

        public override void AddCustomConfiguration(IWindsorContainer container)
        {
            AddCustomConfigurationCalled = true;
        }
    }
}
