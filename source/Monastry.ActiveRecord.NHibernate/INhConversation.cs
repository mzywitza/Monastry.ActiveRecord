using System;
using NHibernate;

namespace Monastry.ActiveRecord
{
	public interface INhConversation : IConversation
	{
		void Execute(Action<ISession> action);

		new INhConversationContext Context { get; }
	}
}
