using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Extensions
{
	public static class EntityExtensions
	{
		public static void Save<T>(this T entity) where T: class, IActiveRecordObject
		{
			AR.Save(entity);
		}

		public static void Add<T>(this T entity) where T : class, IActiveRecordObject
		{
			AR.Add(entity);
		}

		public static void Replace<T>(this T entity) where T : class, IActiveRecordObject
		{
			AR.Replace(entity);
		}

		public static void Delete<T>(this T entity) where T : class, IActiveRecordObject
		{
			AR.Delete(entity);
		}

		public static void Forget<T>(this T entity) where T : class, IActiveRecordObject
		{
			AR.Forget(entity);
		}
	}
}
