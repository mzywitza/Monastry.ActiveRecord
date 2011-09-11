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
		public static void Install(IActiveRecordInstaller installer)
		{
			AR.Install(installer.GetConfiguredContainer());
		}

		/// <summary>
		/// Uses the container provided by the installer to configure AR.
		/// </summary>
		/// <param name="installer">The installer to use.</param>
		public static void Install(IActiveRecordInstaller installer, Usage usage)
		{
			installer.Usage = usage;
			AR.Install(installer.GetConfiguredContainer());
		}

		private static bool IsConfigured(IWindsorContainer container)
		{
			return
				(container.Kernel.GetAssignableHandlers(typeof(IDao<>)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversation)).Length > 0) &&
				(container.Kernel.GetAssignableHandlers(typeof(IConversationContext)).Length > 0);
		}
	}
}
