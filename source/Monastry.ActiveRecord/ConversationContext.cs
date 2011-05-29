using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public class ConversationContext : IConversationContext
	{
		protected IConversation defaultConversation = null;

		public IConversation CurrentConversation
		{
			get
			{
				// TODO: Check for scope
				return defaultConversation;
			}
		}

		public IScope CurrentScope
		{
			get { throw new NotImplementedException(); }
		}

		public void RegisterScope(IScope scope)
		{
			throw new NotImplementedException();
		}

		public void ReleaseScope(IScope scope)
		{
			throw new NotImplementedException();
		}


		public void SetDefaultConversation(IConversation conversation)
		{
			if (conversation == null) throw new ArgumentNullException("conversation");
			if (defaultConversation != null)
				throw new InvalidOperationException("Another default conversation is already set.");
			defaultConversation = conversation;
		}

		public void EndDefaultConversation()
		{
			if (defaultConversation == null)
				throw new InvalidOperationException("No default conversation set. Make sure that SetDefaultConversation() was called before.");
			defaultConversation.Dispose();
			defaultConversation = null;
		}
	}
}
