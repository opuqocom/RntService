using Dapper;
using RntCar.ClassLibrary.Estepe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.DAL
{
    public class DAL : IDB
    {
        IDbConnection connection;
        public IDbConnection ConnectionGet()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Close();
                    connection.Open();
                }
            }
            else
            {
                string assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(assemblyPath);
                var sqlConnectionString = cfg.ConnectionStrings.ConnectionStrings["EstepeConnection"].ConnectionString;
                sqlConnectionString = sqlConnectionString.Replace("uid", "entegre");
                sqlConnectionString = sqlConnectionString.Replace("pass", "Entegre135"); 
                connection = new SqlConnection(sqlConnectionString);
                connection.Open();
            }
            return connection;
        }
        public  IEnumerable<T> Cmd<T>(string query)
        {
            try
            { 
                var result = ConnectionGet().Query<T>(query);
                return result;
            } 
            finally
            {
                connection.Close();
            }
        }
        public int CmdExec(string query)
        {
            try
            {
                int result = ConnectionGet().Execute(query, null, null, 600000000);

                return result;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
