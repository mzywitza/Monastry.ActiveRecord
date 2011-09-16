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
		protected internal static IWindsorContainer Container {get; private set;}

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
			if (usage.HasValue)	installer.Usage = usage.Value;
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
			var context = Container.Resolve<IConversationContext>();
			var conversation = Container.Resolve<IConversation>();
			context.SetDefaultConversation(conversation);
			Container.Release(context);
		}

		/// <summary>
		/// Ends the default conversation. This should be called after finishing the UoW.
		/// </summary>
		public static void EndDefaultConversation()
		{
			var context = Container.Resolve<IConversationContext>();
			var conversation = context.DefaultConversation;
			context.UnsetDefaultConversation();
			Container.Release(conversation);
			Container.Release(context);
		}

		#endregion

		#region Conversation Factory
		/// <summary>
		/// Creates a new conversation. This can be used to create scopes.
		/// </summary>
		/// <returns>A new conversation</returns>
		public static IConversation StartConversation()
		{
			return Container.Resolve<IConversation>();
		}
		#endregion

		#region Current Conversation support
		/// <summary>
		/// Commits the current conversation.
		/// </summary>
		public static void Commit()
		{
			Container.Resolve<IConversationContext>().CurrentConversation.Commit();
		}

		/// <summary>
		/// Cancels the current conversation.
		/// </summary>
		public static void Cancel()
		{
			Container.Resolve<IConversationContext>().CurrentConversation.Cancel();
		}

		/// <summary>
		/// Restarts the current conversation.
		/// </summary>
		public static void Restart()
		{
			Container.Resolve<IConversationContext>().CurrentConversation.Restart();
		}

		#endregion

		#region Entity Support
		public static void Save<TEntity>(TEntity entity) where TEntity : class
		{
			AR.Dao<TEntity>().Save(entity);
		}

		public static void Add<TEntity>(TEntity entity) where TEntity : class
		{
			AR.Dao<TEntity>().Add(entity);
		}

		public static void Replace<TEntity>(TEntity entity) where TEntity : class
		{
			AR.Dao<TEntity>().Replace(entity);
		}

		public static void Delete<TEntity>(TEntity entity) where TEntity : class
		{
			AR.Dao<TEntity>().Delete(entity);
		}
		
		public static void Forget<TEntity>(TEntity entity) where TEntity : class
		{
			AR.Dao<TEntity>().Forget(entity);
		}
		#endregion

		#region Type API
		public static TEntity Find<TEntity>(object id) where TEntity : class
		{
			return AR.Dao<TEntity>().Find(id);
		}

		public static TEntity Peek<TEntity>(object id) where TEntity : class
		{
			return AR.Dao<TEntity>().Peek(id);
		}

		public static IQueryable<TEntity> Linq<TEntity>() where TEntity : class
		{
			return AR.Dao<TEntity>().Linq();
		}

		#endregion

		#region Container lookups
		public static TQuery Query<TQuery>() where TQuery:class
		{
			return Container.Resolve<TQuery>();
		}

		public static IDao<TEntity> Dao<TEntity>() where TEntity : class
		{
			return Container.Resolve<IDao<TEntity>>();
		}

		public static TService Service<TService>() where TService : class
		{
			return Container.Resolve<TService>();
		}



		#endregion
		
		private static bool IsConfigured(IWindsorContainer container)
		{
			return
				(container.Kernel.GetAssignableHandlers(typeof(IDao<>)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversation)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)).Length > 0);
		}
	}
}
