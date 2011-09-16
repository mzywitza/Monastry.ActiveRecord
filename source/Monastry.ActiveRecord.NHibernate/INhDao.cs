namespace Monastry.ActiveRecord
{
	public interface INhDao<TEntity> : IDao<TEntity> where TEntity : class
	{
		new INhConversation CurrentConversation { get; }
	}
}
