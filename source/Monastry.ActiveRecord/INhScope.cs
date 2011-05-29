using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface INhScope : IScope
	{
		new INhConversation AssociatedConversation { get; }
	}
}
