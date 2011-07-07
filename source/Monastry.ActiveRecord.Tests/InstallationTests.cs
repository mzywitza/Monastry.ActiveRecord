using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace Monastry.ActiveRecord.Tests
{
    [TestFixture]
    public class InstallationTests
    {
        [Test]
        public void InstallationSucceedsForCorrectContainer()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
                Component.For<IConversation>().ImplementedBy<DummyConversation>(),
                Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
            AR.Install(container);
        }

        [Test]
        public void InstallationFailsWithoutDao()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IConversation>().ImplementedBy<DummyConversation>(),
                Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
            var ex = Assert.Throws<ArgumentException>(()=>AR.Install(container));
            Assert.That(ex.Message, Contains.Substring("IDao"));
        }

        [Test]
        public void InstallationFailsWithoutConversation()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
                Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
            var ex = Assert.Throws<ArgumentException>(() => AR.Install(container));
            Assert.That(ex.Message, Contains.Substring(typeof(IConversation).Name));
        }

        [Test]
        public void InstallationFailsWithoutConversationContext()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
                Component.For<IConversation>().ImplementedBy<DummyConversation>());
            var ex = Assert.Throws<ArgumentException>(() => AR.Install(container));
            Assert.That(ex.Message, Contains.Substring(typeof(IConversationContext).Name));
        }
    }

    #region Dummies
    public class DummyDao<T> : IDao<T> where T: class
    {

        public IConversation CurrentConversation
        {
            get { throw new NotImplementedException(); }
        }

        public void Save(T entity)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Replace(T entity)
        {
            throw new NotImplementedException();
        }

        public T Find(object id)
        {
            throw new NotImplementedException();
        }

        public T Peek(object id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Linq()
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Forget(T entity)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyConversation : IConversation
    {
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public ConversationCommitMode CommitMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCanceled
        {
            get { throw new NotImplementedException(); }
        }

        public void Execute(Action action)
        {
            throw new NotImplementedException();
        }

        public void ExecuteSilently(Action action)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ConversationCanceledEventArgs> Canceled;

        public IDisposable Scope()
        {
            throw new NotImplementedException();
        }

        public IConversationContext Context
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class DummyConversationContext : IConversationContext
    {

        public IConversation CurrentConversation
        {
            get { throw new NotImplementedException(); }
        }

        public void SetDefaultConversation(IConversation conversation)
        {
            throw new NotImplementedException();
        }

        public void EndDefaultConversation()
        {
            throw new NotImplementedException();
        }

        public IScope CurrentScope
        {
            get { throw new NotImplementedException(); }
        }

        public void RegisterScope(IScope scope)
        {
            throw new NotImplementedException();
        }

        public void ReleaseScope(IScope scope)
        {
            throw new NotImplementedException();
        }
    }
    #endregion  
}
