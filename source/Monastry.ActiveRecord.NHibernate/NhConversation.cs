using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Monastry.ActiveRecord
{
	public class NhConversation : INhConversation
	{
		public event EventHandler<ConversationCanceledEventArgs> Canceled;

		private ConversationCommitMode commitMode = ConversationCommitMode.Automatic;
		private ISessionFactory sessionFactory;
		private ISession session = null;
		private bool sessionStarted = false;
		private bool conversationCanceled = false;
        private INhConversationContext context;

		public NhConversation(ISessionFactory sessionFactory, INhConversationContext context)
		{
			if (sessionFactory == null)
				throw new ArgumentNullException("sessionFactory");
			this.sessionFactory = sessionFactory;
            if (context == null)
                throw new ArgumentNullException("context");
            this.context = context;
        }

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

        public INhConversationContext Context { get { return context; } }

		public void Cancel()
		{
			CancelConversation(true, null);
		}

		public void Commit()
		{
			CheckCanceledState();
			if (session != null)
			{
				using (var transaction = session.BeginTransaction())
					transaction.Commit();
			}
		}

		public void Restart()
		{
			EndSession();
			conversationCanceled = false;
		}


		public bool IsCanceled
		{
			get { return conversationCanceled; }
		}

		public void Execute(Action action)
		{
            ExecuteInternal(action, true);
		}

		public void ExecuteSilently(Action action)
		{
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
            var scope = new NhScope(this);
            context.RegisterScope(scope);
            scope.Disposed += (s, e) => { context.ReleaseScope(scope); };
            return scope;
		}

		public void Dispose()
		{
			EndSession();
		}

		public void Execute(Action<ISession> action)
		{
			CheckCanceledState();
			StartSessionIfNecessary();
			try
			{
				using (var transaction = session.BeginTransaction())
				{
					action(session);
					if (commitMode == ConversationCommitMode.Automatic)
						transaction.Commit();
				}
			}
			catch (Exception ex)
			{
				CancelConversation(false, ex);
				throw;
			}
		}

		private void StartSessionIfNecessary()
		{
			if (session != null) return;
			session = sessionFactory.OpenSession();
			sessionStarted = true;
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
			if (session != null)
			{
				session.Dispose();
				session = null;
			}
        }

        #region Explicit
        IConversationContext IConversation.Context { get { return Context; } } 
        #endregion
    }
}
