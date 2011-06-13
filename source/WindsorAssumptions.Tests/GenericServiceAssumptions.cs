using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace WindsorAssumptions.Tests
{
    [TestFixture]
    public class GenericServiceAssumptions
    {
        private IWindsorContainer container =null;

        [SetUp]
        public void CreateContainer()
        {
            container = new WindsorContainer().Install(new FooInstaller());
        }

        [Test]
        public void CanOverrideService()
        {
            Assert.That(container.Resolve<IFooService<A>>(), Is.TypeOf<FooService<A>>());
            Assert.That(container.Resolve<IFooService<B>>(), Is.TypeOf<BarService<B>>());
        }

        [Test]
        public void OverriddenServiceIsSingleton()
        {
            var b1 = container.Resolve<IFooService<B>>();
            var b2 = container.Resolve<IFooService<B>>();

            Assert.That(b1, Is.SameAs(b2));
        }

        [Test]
        public void CanBeOverriddenAfterResolve()
        {
            var c1 = container.Resolve<IFooService<C>>();
            Assert.That(c1, Is.TypeOf<FooService<C>>());

            container.Register(Component.For<IFooService<C>>().Instance(new BarService<C> { NameValue = "C" }));

            var c2 = container.Resolve<IFooService<C>>();
            Assert.That(c2, Is.TypeOf<BarService<C>>());
        }

    }

    public class FooInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(IFooService<>))
                    .ImplementedBy(typeof(FooService<>))
                    .LifeStyle.Singleton,
                Component.For<IFooService<B>>()
                    .Instance(new BarService<B> {NameValue = "B"})
                );
        }
    }

    interface IFooService<T>
    {
        string Name(T t);
    }

    class FooService<T> : IFooService<T>
    {
        public string Name(T t)
        {
            return "Generic:"+typeof(T).Name;
        }
    }

    class BarService<T> : IFooService<T>
    {
        public string NameValue;

        public string Name(T t)
        {
            return NameValue;
        }
    }

    class A { }
    class B { }
    class C { }

}
