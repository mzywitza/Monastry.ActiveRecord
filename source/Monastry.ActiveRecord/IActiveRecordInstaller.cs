using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// This interface defines installer classes for AR. ORM providers
	/// should extend and implement it for their own configuration needs.
	/// </summary>
	public interface IActiveRecordInstaller
	{
		/// <summary>
		/// This method is called by AR when <see cref="AR.Install(IActiveRecordInstaller)"/>
		/// is called to obtain the container after all configuration has been done.
		/// </summary>
		/// <returns>The completely configured container.</returns>
		IWindsorContainer GetConfiguredContainer();

		/// <summary>
		/// This is a hook for end user to add additional services to the container.
		/// <para>
		/// ORM providers must call this hook after their initial configuration is finished.
		/// </para>
		/// </summary>
		/// <param name="container">The container instance to configure</param>
		void AddCustomConfiguration(IWindsorContainer container);

		/// <summary>
		/// The intended usage.
		/// </summary>
		Usage Usage { get; set; }
	}
}
