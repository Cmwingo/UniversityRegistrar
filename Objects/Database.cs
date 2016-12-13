using System.Data;
using System.Data.SqlClient;

namespace UniversityRegistrar
{
  public class DB
  {
    public static SqlConnection Connection()
    {
      SqlConnection conn = new SqlConnection(DBConfiguration.ConnectionString);
      return conn;
    }
  }
}
