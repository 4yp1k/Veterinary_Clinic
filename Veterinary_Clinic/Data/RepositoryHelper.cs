using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace Veterinary_Clinic.Data
{
    public static class RepositoryHelper
    {
        private static string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
        
    }
}
