using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Monastry.ActiveRecord
{
    public class NhDao<TEntity> : INhDao<TEntity> where TEntity:class
    {
        protected readonly INhConversationContext ConversationContext;

        public NhDao(INhConversationContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            ConversationContext = context;
        }

        public INhConversation CurrentConversation
        {
            get { return ConversationContext.CurrentConversation; }
        }

        IConversation IDao<TEntity>.CurrentConversation
        {
            get { return CurrentConversation; }
        }


        public void Save(TEntity entity)
        {
            CurrentConversation.Execute(s => s.SaveOrUpdate(entity));
        }

        public void Add(TEntity entity)
        {
            CurrentConversation.Execute(s => s.Save(entity));
        }

        public void Replace(TEntity entity)
        {
            CurrentConversation.Execute(s => s.Update(entity));
        }

        public TEntity Find(object id)
        {
            TEntity entity = null;
            CurrentConversation.Execute(s => entity = s.Get<TEntity>(id));
            return entity;
        }


        public TEntity Peek(object id)
        {
            TEntity entity = null;
            CurrentConversation.Execute(s => entity = s.Load<TEntity>(id));
            return entity;
        }


        public IQueryable<TEntity> Linq()
        {
            IQueryable<TEntity> linq = null;
            CurrentConversation.Execute(s => linq = s.Query<TEntity>());
            return linq;
        }

        public void Delete(TEntity entity)
        {
            CurrentConversation.Execute(s => s.Delete(entity));
        }

        public void Forget(TEntity entity)
        {
            CurrentConversation.Execute(s => s.Evict(entity));
        }

    }
}
