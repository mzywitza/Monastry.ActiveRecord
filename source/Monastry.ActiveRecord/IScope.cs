using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
    /// <summary>
    /// Adds the possibility to set the current conversation using scopes.
    /// </summary>
    /// <remarks>
    /// This interface will only be used as part of the framework. The end-
    /// user doesnot need to use this interface or one of the classes
    /// implementing it.
    /// </remarks>
    /// <example>
    /// This example shows how a conversation creates a scope:
    /// <code>
    /// var conv = GetConversation(); // helper method
    /// using (conv.Scope())
    /// {
    ///     // All operations here use the conversation conv.
    /// }
    /// </code>
    /// </example> 
	public interface IScope : IDisposable
	{
        /// <summary>
        /// The conversation object associated with this scope.
        /// </summary>
        /// <remarks>
        /// When a scope is used, all operations will be executed using the
        /// associated conversation.
        /// </remarks>
		IConversation AssociatedConversation { get; }

        /// <summary>
        /// Checks whether a scope is valid.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A scope can become invalid when its <see cref="AssociatedConversation"/>
        /// is disposed during the lifetime of the scope.
        /// </para>
        /// <para>
        /// Whenever an invalid scope is detected, the framework will throw as soon as
        /// possible to guarantee a determinable state at all times.
        /// </para>
        /// </remarks>
		bool IsValid { get; }

        /// <summary>
        /// Marks a scope as invalid.
        /// </summary>
        /// <seealso cref="IsValid"/>
		void Invalidate();

        /// <summary>
        /// Called when the scope ends. Used to allow creating Conversations to release
        /// the scope.
        /// </summary>
        event EventHandler Disposed;
	}
}
