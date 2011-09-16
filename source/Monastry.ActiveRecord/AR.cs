using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// Central class for AR.
	/// </summary>
	public class AR
	{
		/// <summary>
		/// This container is used by AR to obtain all internal services.
		/// </summary>
		protected internal static IWindsorContainer Container { get; private set; }

		#region Installation
		/// <summary>
		/// Registers the container for AR internal usage. The container must be
		/// configured to resolve at least the following services:
		/// <list type="bullet">
		/// <item><see cref="IDao<>"/></item>
		/// <item><see cref="IConversation"/></item>
		/// <item><see cref="IConversationContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="container">The container instance to use.</param>
		/// <exception cref="ArgumentException">
		/// <see cref="ArgumentException"/> will be thrown when not all
		/// necessary services are registered.
		/// </exception>
		public static void Install(IWindsorContainer container)
		{
			if (!IsConfigured(container))
				throw new ArgumentException(
					"The container is not correctly configured. Please make sure that at " +
					"least the following services are included in installing the container " +
					"IConversation, IConversationContext, IDao", "container");
			Container = container;
		}

		/// <summary>
		/// Uses the container provided by the installer to configure AR.
		/// </summary>
		/// <param name="installer">The installer to use.</param>
		public static void Install(IActiveRecordInstaller installer, Usage? usage = null, ConversationCommitMode? commitMode = null)
		{
			if (usage.HasValue) installer.Usage = usage.Value;
			if (commitMode.HasValue) installer.CommitMode = commitMode.Value;
			AR.Install(installer.GetConfiguredContainer());
		}
		#endregion

		#region Implicit conversation management
		/// <summary>
		/// Starts a default conversation that is used when no scopes are in effect.
		/// </summary>
		public static void StartDefaultConversation()
		{
			WithContext(cc => cc.SetDefaultConversation(Container.Resolve<IConversation>()));
		}

		/// <summary>
		/// Ends the default conversation. This should be called after finishing the UoW.
		/// </summary>
		public static void EndDefaultConversation()
		{
			WithContext(cc =>
				{
					var conv = cc.DefaultConversation;
					cc.UnsetDefaultConversation();
					Container.Release(conv);
				});
		}

		#endregion

		#region Conversation Factory
		/// <summary>
		/// Creates a new conversation. This can be used to create scopes. The
		/// conversation must not be disposed, but released by calling 
		/// <see cref="AR.EndConversation(IConversation)"/>
		/// </summary>
		/// <returns>A new conversation</returns>
		public static IConversation StartConversation()
		{
			return Container.Resolve<IConversation>();
		}

		/// <summary>
		/// Ends the conversation.
		/// </summary>
		/// <param name="conversation">The conversation to end.</param>
		public static void EndConversation(IConversation conversation)
		{
			if (conversation == null) throw new ArgumentNullException("conversation");
			Container.Release(conversation);
		}
		#endregion

		#region Current Conversation support
		/// <summary>
		/// Commits the current conversation.
		/// </summary>
		public static void Commit()
		{
			WithContext(cc=>cc.CurrentConversation.Commit());
		}

		/// <summary>
		/// Cancels the current conversation.
		/// </summary>
		public static void Cancel()
		{
			WithContext(cc => cc.CurrentConversation.Cancel());
		}

		/// <summary>
		/// Restarts the current conversation.
		/// </summary>
		public static void Restart()
		{
			WithContext(cc => cc.CurrentConversation.Restart());
		}

		#endregion

		#region Entity Support
		public static void Save<TEntity>(TEntity entity) where TEntity : class
		{
			WithDao<TEntity>(d=>d.Save(entity));
		}

		public static void Add<TEntity>(TEntity entity) where TEntity : class
		{
			WithDao<TEntity>(d => d.Add(entity));
		}

		public static void Replace<TEntity>(TEntity entity) where TEntity : class
		{
			WithDao<TEntity>(d => d.Replace(entity));
		}

		public static void Delete<TEntity>(TEntity entity) where TEntity : class
		{
			WithDao<TEntity>(d => d.Delete(entity));
		}

		public static void Forget<TEntity>(TEntity entity) where TEntity : class
		{
			WithDao<TEntity>(d => d.Forget(entity));
		}
		#endregion

		#region Type API
		public static TEntity Find<TEntity>(object id) where TEntity : class
		{
			return ReturnWithDao<TEntity, TEntity>(d => d.Find(id));
		}

		public static TEntity Peek<TEntity>(object id) where TEntity : class
		{
			return ReturnWithDao<TEntity,TEntity>(d=>d.Peek(id));
		}

		public static IQueryable<TEntity> Linq<TEntity>() where TEntity : class
		{
			return ReturnWithDao<IQueryable<TEntity>,TEntity>(d => d.Linq());
		}

		#endregion

		#region Query support
		public static TResult Query<TResult, TQuery>(Action<TQuery> configureQuery = null) where TQuery : class, IQuery<TResult>
		{
			var query = Container.Resolve<TQuery>();
			if (configureQuery != null)
				configureQuery.Invoke(query);
			TResult result = query.Execute();
			Container.Release(query);
			return result;
		}

		#endregion

		private static bool IsConfigured(IWindsorContainer container)
		{
			return
				(container.Kernel.GetAssignableHandlers(typeof(IDao<>)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversation)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)).Length > 0);
		}

		private static void WithContext(Action<IConversationContext> action)
		{
			var context = Container.Resolve<IConversationContext>();
			action.Invoke(context);
			Container.Release(context);
		}

		private static void WithDao<TEntity>(Action<IDao<TEntity>> action) where TEntity : class
		{
			var dao = Container.Resolve<IDao<TEntity>>();
			action.Invoke(dao);
			Container.Release(dao);
		}

		private static TResult ReturnWithDao<TResult, TEntity>(Func<IDao<TEntity>,TResult> func) where TEntity : class
		{
			var dao = Container.Resolve<IDao<TEntity>>();
			TResult result = func.Invoke(dao);
			Container.Release(dao);
			return result;
		}
	}
}
