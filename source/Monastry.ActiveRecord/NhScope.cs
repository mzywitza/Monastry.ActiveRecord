using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public class NhScope : INhScope
	{
		private INhConversation conversation;
		private bool valid = true;

		public NhScope(INhConversation conversation)
		{
			this.conversation = conversation;
		}

		public INhConversation AssociatedConversation
		{
			get { return conversation; }
		}

		public bool IsValid
		{
			get { return valid; }
		}

		public void Invalidate()
		{
			valid = false;
		}

		#region Explicit
		IConversation IScope.AssociatedConversation
		{
			get { return AssociatedConversation; }
		}
		#endregion

	}
}
