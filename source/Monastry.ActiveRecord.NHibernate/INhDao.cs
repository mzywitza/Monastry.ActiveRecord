using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
    public interface INhDao<TEntity> : IDao<TEntity> where TEntity : class
    {
        new INhConversation CurrentConversation { get; }
    }
}
