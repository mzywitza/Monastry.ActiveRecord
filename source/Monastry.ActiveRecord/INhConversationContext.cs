using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface INhConversationContext : IConversationContext
	{
		new INhConversation CurrentConversation { get; }

		new INhScope CurrentScope { get; }
	}
}
