using NHibernate.Cfg;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// NHibernate specific <see cref="IActiveRecordInstaller"/>.
	/// </summary>
	public interface INhActiveRecordInstaller : IActiveRecordInstaller
	{
		/// <summary>
		/// Obtains the NHibernate configuration. The object must be fully 
		/// configured because it will be used to create the <see cref="ISessionFactory"/>
		/// after the call.
		/// </summary>
		/// <returns>The NHibernate configuration to use.</returns>
		Configuration GetNhConfiguration();
	}
}
