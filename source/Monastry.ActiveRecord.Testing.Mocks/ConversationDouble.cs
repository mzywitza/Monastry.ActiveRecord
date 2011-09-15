using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Testing.Mocks
{
	public class ConversationDouble : IConversation
	{
		private bool strict;
		private IConversation userDouble;

		public ConversationDouble(bool useStrictMocking, IConversationContext context, IConversation userSpecifiedDouble)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			strict = useStrictMocking;
			userDouble = userSpecifiedDouble;
		}

		public event EventHandler<ConversationCanceledEventArgs> Canceled;

		private ConversationCommitMode commitMode = ConversationCommitMode.Automatic;
		private bool sessionStarted = false;
		private bool conversationCanceled = false;
		private IConversationContext context;
		private List<IScope> scopes = new List<IScope>();

		public ConversationCommitMode CommitMode
		{
			get
			{
				return commitMode;
			}
			set
			{
				if (sessionStarted)
					throw new NotSupportedException("CommitMode cannot be set after the conversation started");
				commitMode = value;
			}
		}

		public IConversationContext Context { get { return context; } }

		public void Cancel()
		{
			if (userDouble != null)
			{
				userDouble.Cancel(); 
				return;
			}
			CheckStrictness();
			CancelConversation(true, null);
		}

		public void Commit()
		{
			if (userDouble != null)
			{
				userDouble.Commit();
				return;
			}
			CheckStrictness();
			CheckCanceledState();
		}

		public void Restart()
		{
			if (userDouble != null)
			{
				userDouble.Restart();
				return;
			}
			CheckStrictness();
			EndSession();
			conversationCanceled = false;
		}


		public bool IsCanceled
		{
			get { return conversationCanceled; }
		}

		public void Execute(Action action)
		{
			if (userDouble != null)
			{
				userDouble.Execute(action);
				return;
			}
			CheckStrictness();
			ExecuteInternal(action, true);
		}

		public void ExecuteSilently(Action action)
		{
			if (userDouble != null)
			{
				userDouble.ExecuteSilently(action);
				return;
			}
			CheckStrictness();
			ExecuteInternal(action, false);
		}

		private void ExecuteInternal(Action action, bool rethrow)
		{
			CheckCanceledState();
			try
			{
				using (Scope())
				{
					action.Invoke();
				}
			}
			catch (Exception e)
			{
				CancelConversation(false, e);
				if (rethrow) throw;
			}
		}
		
		public IDisposable Scope()
		{
			var scope = new ScopeDouble(this);
			context.RegisterScope(scope);
			scopes.Add(scope);
			scope.Disposed += ScopeDisposed;
			return scope;
		}

		private void ScopeDisposed(object sender, EventArgs args)
		{
			var scope = sender as ScopeDouble;
			if (scope != null)
			{
				context.ReleaseScope(scope);
				scopes.Remove(scope);
			}
		}

		public void Dispose()
		{
			foreach (var scope in scopes)
				scope.Invalidate();
			EndSession();
		}

		private void CancelConversation(bool canceledByUser, Exception exception)
		{
			conversationCanceled = true;
			if (Canceled == null) return;
			Canceled(this, new ConversationCanceledEventArgs(canceledByUser, exception));
		}

		private void CheckCanceledState()
		{
			if (IsCanceled)
				throw new InvalidOperationException("This conversation was already canceled.");
		}

		private void EndSession()
		{
			if (!IsCanceled && CommitMode == ConversationCommitMode.OnClose)
				Commit();
		}

		private void CheckStrictness()
		{
			if (strict) throw new InvalidOperationException("Unexpected method call on strict test double.");
		}

	}
}
