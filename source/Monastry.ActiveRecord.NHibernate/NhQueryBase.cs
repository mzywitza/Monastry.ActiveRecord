using System;
using NHibernate;

namespace Monastry.ActiveRecord
{
	/// <summary>
	/// Base class for queries using NHibernate.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public abstract class NhQueryBase<TResult> : INhQuery<TResult>
	{
		public INhConversationContext ConversationContext { get; set; }

		public INhConversation CurrentConversation
		{
			get { return ConversationContext.CurrentConversation; }
		}

		IConversation IQuery<TResult>.CurrentConversation
		{
			get { return (INhConversation)CurrentConversation; }
		}

		public TResult Execute()
		{
			if (ConversationContext == null)
				throw new InvalidOperationException("ConversationContext is not set. This means that the query is not properly registered with ActiveRecord.");
			ISession session = null;
			CurrentConversation.Execute(s => session = s);
			return Execute(session);
		}

		/// <summary>
		/// This method should be implemented to do the actual querying.
		/// </summary>
		/// <param name="session">The current conversation's session.</param>
		/// <returns>The query result.</returns>
		public abstract TResult Execute(ISession session);
	}
}
