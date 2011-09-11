// Copied from Castle.ActiveRecord
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{

	/// <summary>
	/// Determines when the sessions in a conversation flush.
	/// </summary>
	public enum ConversationCommitMode
	{
		/// <summary>
		/// FlushMode is set to automatic, all sessions flush
		/// whenever NHibernate needs it.
		/// </summary>
		Automatic,

		/// <summary>
		/// All information is flushed when the conversation is
		/// disposed and was not canceled
		/// </summary>
		OnClose,

		/// <summary>
		/// Conversation must be flushed explicitly.
		/// </summary>
		Explicit
	}

	/// <summary>
	/// Conversations allow to define broader units of work
	/// than <see cref="SessionScope"/> allows to.
	/// </summary>
	public interface IConversation : IDisposable
	{
		/// <summary>
		/// Cancels all changes made in this session.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Flushes all sessions in this conversation.
		/// </summary>
		void Commit();

		/// <summary>
		/// Resets the conversation, allowing it to be used again
		/// with new sessions after canceling.
		/// <remarks>
		/// This functionality supports serving instances through
		/// IoC where it is not possible to simple create a new
		/// conversation after an error. Restarting the conversation
		/// offers error recovery in such cases.
		/// </remarks>
		/// </summary>
		void Restart();

		/// <summary>
		/// The CommitMode to use. Setting the commit mode via
		/// property allows using IoC-Containers for
		/// providing Conversation objects and configuring
		/// them afterwards.
		/// Setting this property is only supported before
		/// the conversation is actually used.
		/// </summary>
		ConversationCommitMode CommitMode { get; set; }

		/// <summary>
		/// Whether the conversation is canceled
		/// </summary>
		bool IsCanceled { get; }

		/// <summary>
		/// Executes a block of code in the context of the
		/// conversation. This allows to use ActiveRecord
		/// without any scopes by doing all persistence calls
		/// within Execute.
		/// If an exception is caught, the conversation is
		/// automatically canceled and the exception handed
		/// down to the calling code. 
		/// </summary>
		/// <param name="action">The code to execute</param>
		/// <remarks>
		/// This allows to use the interface directly, for example
		/// if it is served through an IoC-Container.
		/// </remarks>
		void Execute(Action action);

		/// <summary>
		/// Executes a block of code. The conversation is canceled
		/// if an exception occurs, but the exception will not be
		/// handed to the calling code.
		/// </summary>
		/// <param name="action">The code to execute</param>
		void ExecuteSilently(Action action);

		/// <summary>
		/// Fired when the conversation is canceled.
		/// </summary>
		event EventHandler<ConversationCanceledEventArgs> Canceled;

		/// <summary>
		/// Registers a scope associated with the conversation.
		/// </summary>
		/// <returns></returns>
		IDisposable Scope();

		/// <summary>
		/// The conversation's context.
		/// </summary>
		IConversationContext Context { get; }
	}
}
