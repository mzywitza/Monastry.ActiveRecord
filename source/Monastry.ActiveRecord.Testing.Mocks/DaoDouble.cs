using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Testing.Mocks
{
	public class DaoDouble<T> : IDao<T> where T : class
	{
		private bool strict;

		public DaoDouble(bool useStrictMocking)
		{
			strict = useStrictMocking;
		}

		public IConversation CurrentConversation
		{
			get { throw new InvalidOperationException("This method must never be called on a test double. It is intended to be used internally."); }
		}

		public void Save(T entity)
		{
			CheckStrictness();
		}

		public void Add(T entity)
		{
			CheckStrictness();
		}

		public void Replace(T entity)
		{
			CheckStrictness();
		}

		public T Find(object id)
		{
			CheckStrictness();
			return null;
		}

		public T Peek(object id)
		{
			CheckStrictness();
			return null;
		}

		public IQueryable<T> Linq()
		{
			CheckStrictness();
			return new List<T>().AsQueryable();
		}

		public void Delete(T entity)
		{
			CheckStrictness();
		}

		public void Forget(T entity)
		{
			CheckStrictness();
		}

		private void CheckStrictness()
		{
			if (strict) throw new InvalidOperationException("Unexpected call on strict test double.");
		}
	}
}
