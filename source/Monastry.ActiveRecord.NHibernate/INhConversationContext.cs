
namespace Monastry.ActiveRecord
{
	public interface INhConversationContext : IConversationContext
	{
		new INhConversation CurrentConversation { get; }

		new INhScope CurrentScope { get; }
	}
}
