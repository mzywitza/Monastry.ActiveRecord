using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Monastry.ActiveRecord.Tests.Model
{
	public static class DummyFactory
	{
		public static IWindsorContainer CreateContainer()
		{
			var container = new WindsorContainer();
			container.Register(
				Component.For(typeof(IDao<>)).ImplementedBy(typeof(DummyDao<>)),
				Component.For<IConversation>().ImplementedBy<DummyConversation>(),
				Component.For<IConversationContext>().ImplementedBy<DummyConversationContext>());
			return container;
		}
	}

	#region Dummies
	public class DummyDao<T> : IDao<T> where T : class
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
