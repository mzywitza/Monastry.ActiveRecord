using System;

namespace Monastry.ActiveRecord.Testing
{
	public class ScopeDouble : IScope
	{
		private IConversation conversation;
		private bool valid = true;

		public ScopeDouble(IConversation conversation)
		{
			this.conversation = conversation;
		}

		public IConversation AssociatedConversation
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

	}
}
