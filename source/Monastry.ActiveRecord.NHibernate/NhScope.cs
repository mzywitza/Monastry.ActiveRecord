using System;

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

		public void Dispose()
		{
			if (Disposed != null)
				Disposed(this, new EventArgs());
		}

		public event EventHandler Disposed;

		#region Explicit
		IConversation IScope.AssociatedConversation
		{
			get { return AssociatedConversation; }
		}
		#endregion
	}
}
