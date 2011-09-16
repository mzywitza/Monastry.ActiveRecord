using System.Data;
using NHibernate.Connection;

namespace Monastry.ActiveRecord.Testing
{
	class InMemoryConnectionProvider : DriverConnectionProvider
	{
		private static IDbConnection connection;

		public override IDbConnection GetConnection()
		{
			if (connection == null) connection = base.GetConnection();
			return connection;
		}

		public override void CloseConnection(IDbConnection conn)
		{
			//base.CloseConnection(conn);
		}

		public static void CloseDatabase()
		{
			if (connection != null)
			{
				connection.Close();
				connection.Dispose();
				connection = null;
			}
		}
	}
}
