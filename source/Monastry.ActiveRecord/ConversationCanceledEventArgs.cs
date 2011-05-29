// Taken from Castle.ActiveRecord
using System;

namespace Monastry.ActiveRecord
{

	/// <summary>
	/// Event arguments for the Canceled-event.
	/// </summary>
	public class ConversationCanceledEventArgs : EventArgs
	{
		/// <summary>
		/// Was the conversation canceld by a call to
		/// <see cref="IConversation.Cancel"/>?
		/// </summary>
		public bool CanceledByUser { get; private set; }

		/// <summary>
		/// Holds the exception caused the cancellation, if any.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="canceledByUser">Whether the conversation was user-canceled.</param>
		/// <param name="exception">The exception causing the conversation to cancel.</param>
		public ConversationCanceledEventArgs(bool canceledByUser, Exception exception)
		{
			CanceledByUser = canceledByUser;
			Exception = exception;
		}
	}
}