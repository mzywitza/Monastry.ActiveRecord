using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// Redefines <see cref="IQuery"/> for usage with NHibernate ORM.
	/// </summary>
	/// <typeparam name="TResult">The query's result type</typeparam>
	public interface INhQuery<TResult> : IQuery<TResult>
	{
		/// <summary>
		/// The NHibernate conversation used for querying.
		/// </summary>
		new INhConversation CurrentConversation { get; }
	}
}
