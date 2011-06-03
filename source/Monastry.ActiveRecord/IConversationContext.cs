using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface IConversationContext<TConversation, TScope> 
        where TConversation : class, IConversation
        where TScope : class, IScope<TConversation>
	{
		TConversation CurrentConversation { get; }
		void SetDefaultConversation(TConversation conversation);
		void EndDefaultConversation();

		TScope CurrentScope { get; }
		void RegisterScope(TScope scope);
		void ReleaseScope(TScope scope);
	}
}
