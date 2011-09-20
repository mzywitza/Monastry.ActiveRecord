using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace Monastry.ActiveRecord.Extensions
{
	public static class InstallerExtensions
	{
		public static IWindsorContainer InstallQuery<TQuery>(this IWindsorContainer container) where TQuery : class
		{
			container.Register(Component.For<TQuery>());
			return container;
		}

		public static IWindsorContainer InstallQueries(this IWindsorContainer container, params Type[] queryTypes)
		{
			container.Register(Component.For(queryTypes));
			return container;
		}

		public static IEnumerable<Type> InstallInto(this IEnumerable<Type> queryTypes, IWindsorContainer container)
		{
			container.Register(Component.For(queryTypes));
			return queryTypes;
		}

		public static IWindsorContainer InstallAllQueries(this IWindsorContainer container)
		{
			return container.InstallAllQueries(Assembly.GetCallingAssembly());
		}

		public static IWindsorContainer InstallAllQueries(this IWindsorContainer container, Assembly assembly)
		{
			container.Register(
				Classes
					.FromAssembly(assembly)
					.BasedOn(typeof(IQuery<>))
					.WithServiceAllInterfaces()
					);
			return container;
		}

		public static ComponentRegistration<IConversationContext> WithUsage(this ComponentRegistration<IConversationContext> registration, Usage usage)
		{
			switch (usage)
			{
				case Usage.Web: return registration.LifeStyle.PerWebRequest;
				case Usage.Service: return registration.LifeStyle.PerThread;
				default: return registration.LifeStyle.Singleton;
			}
		}
	}
}
