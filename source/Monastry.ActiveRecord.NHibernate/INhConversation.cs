using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Monastry.ActiveRecord
{
	public interface INhConversation : IConversation
	{
		void Execute(Action<ISession> action);

        new INhConversationContext Context { get; }
	}
}
