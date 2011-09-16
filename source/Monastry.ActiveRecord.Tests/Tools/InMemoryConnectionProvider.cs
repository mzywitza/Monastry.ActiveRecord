using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Connection;
using System.Data;

namespace Monastry.ActiveRecord.Tests.Tools
{
    public class InMemoryConnectionProvider : DriverConnectionProvider
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
