using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public class ConversationContext : IConversationContext
	{
		protected IConversation defaultConversation = null;

		public virtual IConversation DefaultConversation { get { return defaultConversation; } }

		public IConversation CurrentConversation
		{
			get
			{
				if (CurrentScope != null)
					return CurrentScope.AssociatedConversation;
				return defaultConversation;
			}
		}

		public IScope CurrentScope
		{
			get
			{
				var currentScope = scopes.Count > 0 ? scopes.Peek() : null;
				if (currentScope != null && !currentScope.IsValid)
					throw new InvalidOperationException(
						"The current scope is invalid. This is most likely an internal error. " +
						"Please make sure that the conversation is not disposed before a scope is released.");
				return currentScope;
			}
		}

		public void RegisterScope(IScope scope)
		{
			if (scope == null) throw new ArgumentNullException("scope");
			if (!scope.IsValid)
				throw new InvalidOperationException(
					"The scope that should be registered is invalid. This is most likely an internal error. " +
					"Please make sure that the conversation is not disposed before a scope is released.");
			if (scope.AssociatedConversation == null)
				throw new InvalidOperationException("The scope's conversation is null. This is an internal error. ");
			if (scopes.Contains(scope))
				throw new InvalidOperationException(
					"This scope is already registered. Scopes cannot be registered twice. This is an internal error. ");
			scopes.Push(scope);
		}

		public void ReleaseScope(IScope scope)
		{
			if (scope == null) throw new ArgumentNullException("scope");
			if (!scopes.Contains(scope))
				throw new InvalidOperationException(
					"This scope has not been registered with the context. An unregistered scope cannot be released. " +
					"This is an internal error.");
			if (!scope.IsValid)
				throw new InvalidOperationException(
					"The scope that should be released is invalid. This is most likely an internal error. " +
					"Please make sure that the conversation is not disposed before a scope is released.");
			if (scope != CurrentScope)
				throw new InvalidOperationException(
					"The scope that should be released is not the innermost scope. Please make sure that " + 
					"scopes are released in reverse nesting order.");
			scopes.Pop();
		}

		private Stack<IScope> scopes = new Stack<IScope>();

		public void SetDefaultConversation(IConversation conversation)
		{
			if (conversation == null) throw new ArgumentNullException("conversation");
			if (defaultConversation != null)
				throw new InvalidOperationException("Another default conversation is already set.");
			defaultConversation = conversation;
		}

		public void UnsetDefaultConversation()
		{
			if (defaultConversation == null)
				throw new InvalidOperationException("No default conversation set. Make sure that SetDefaultConversation() was called before.");
			defaultConversation = null;
		}
	}
}
