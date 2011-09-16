namespace Monastry.ActiveRecord
{
	public interface INhScope : IScope
	{
		new INhConversation AssociatedConversation { get; }
	}
}
