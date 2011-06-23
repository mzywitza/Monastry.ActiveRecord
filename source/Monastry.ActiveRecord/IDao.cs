using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
    public interface IDao<TEntity> where TEntity : class
    {
        IConversation CurrentConversation { get; }

        void Save(TEntity entity);
        void Add(TEntity entity);
        void Replace(TEntity entity);

        TEntity Find(object id);
        TEntity Peek(object id);
        IQueryable<TEntity> Linq();

        void Delete(TEntity entity);
        void Forget(TEntity entity);
    }
}
