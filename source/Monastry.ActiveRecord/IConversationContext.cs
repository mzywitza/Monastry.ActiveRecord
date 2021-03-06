﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface IConversationContext
	{
		IConversation CurrentConversation { get; }
		IConversation DefaultConversation { get; }
		void SetDefaultConversation(IConversation conversation);
		void UnsetDefaultConversation();

		IScope CurrentScope { get; }
		void RegisterScope(IScope scope);
		void ReleaseScope(IScope scope);
	}
}
