using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface IScope
	{
		IConversation AssociatedConversation { get; }

		bool IsValid { get; }
		void Invalidate();
	}
}
