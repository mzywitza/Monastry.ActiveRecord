using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Monastry.ActiveRecord;
using NHibernate;
using Rhino.Mocks;

namespace WindsorAssumptions.Tests
{
    [TestFixture]
    public class WindsorAssumptions
    {
        [Test]
        public void InstallerWorks()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(new NhInstaller());
        }

        [Test]
        public void ConversationClassesAreInstalled()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(new NhInstaller());

            Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)), Is.Not.Empty);
            Assert.That(container.Kernel.GetAssignableHandlers(typeof(IConversation)), Is.Not.Empty);
        }

        [Test]
        public void NhConversationInterfacesAreForwared()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(new NhInstaller());

            Assert.That(container.Kernel.GetAssignableHandlers(typeof(INhConversationContext)), Is.Not.Empty);
            Assert.That(container.Kernel.GetAssignableHandlers(typeof(INhConversation)), Is.Not.Empty);
        }

        [Test]
        public void ConversationIsCorrectlyImplemented()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(new NhInstaller());

            var conv = container.Resolve<IConversation>();
            Assert.That(conv, Is.InstanceOf<NhConversation>());
            Assert.That(conv.Context, Is.InstanceOf<NhConversationContext>());
        }

        [Test]
        public void ConversationContextIsShared()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(new NhInstaller());

            var conv1 = container.Resolve<IConversation>();
            var conv2 = container.Resolve<IConversation>();

            Assert.That(conv1, Is.Not.SameAs(conv2));
            Assert.That(conv1.Context, Is.SameAs(conv2.Context));
        }
    }

    class NhInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container,IConfigurationStore store)
        {
            container.Register(
                Component.For<IConversation>()
                    .Forward<INhConversation>()
                    .ImplementedBy<NhConversation>()
                    .LifeStyle.Transient,
                Component.For<IConversationContext>()
                    .Forward<INhConversationContext>()
                    .ImplementedBy<NhConversationContext>()
                    .LifeStyle.PerThread,
                Component.For<ISessionFactory>()
                    .Instance(MockRepository.GenerateStub<ISessionFactory>())
                    .LifeStyle.Singleton
            );
        }
    }
}
