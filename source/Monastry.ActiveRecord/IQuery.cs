using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// Defines a base interface for querying. 
	/// <para>
	/// To use querying, define an interface for it that inherits from this
	/// interface. Then create a Query class based on an ORM providers Query
	/// base class.
	/// </para>
	/// </summary>
	/// <typeparam name="TResult">The result type of the query.</typeparam>
	public interface IQuery<TResult>
	{
		/// <summary>
		/// The conversation that will be used for querying.
		/// </summary>
		IConversation CurrentConversation {get;}

		/// <summary>
		/// Executes the specified query.
		/// </summary>
		/// <returns>The query's result.</returns>
		TResult Execute(); 
	}
}
