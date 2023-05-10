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
            else//Bu else bloğu, ilk kez bir bağlantı yapılırken çalışacak olan kod bloğudur.
            {
                string assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;//İlk olarak, kod, yürütülen derlemenin kod tabanının yolu hakkında bilgi içeren bir Uri oluşturur. 
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(assemblyPath);//Daha sonra ConfigurationManager sınıfı kullanarak yapılandırma ayarlarını yükler  
                var sqlConnectionString = cfg.ConnectionStrings.ConnectionStrings["EstepeConnection"].ConnectionString;//EstepeConnection adlı bir bağlantı dizesini alır.

                //Sonraki satırlar, sqlConnectionString adlı bağlantı dizesinin kullanıcı adı ve parolasını ayarlar. uid ve pass parametreleri, bu bağlantı dizesindeki kullanıcı adı ve parola bilgisini temsil ederler. Bu satırlar, bu parametreleri gerçek kullanıcı adı ve parola bilgileriyle değiştirirler.
                sqlConnectionString = sqlConnectionString.Replace("uid", "entegre");
                sqlConnectionString = sqlConnectionString.Replace("pass", "Entegre135");
                //Son olarak, SqlConnection sınıfından bir bağlantı oluşturulur ve bu bağlantı açılır. Oluşturulan ve açılan bağlantı sonunda geri döndürülür.
                connection = new SqlConnection(sqlConnectionString);
                connection.Open();
            }
            return connection;
        }
        public IEnumerable<T> Cmd<T>(string query)//Cmd" metodu, belirtilen sorguyu veritabanında çalıştırır ve belirtilen tipin nesnelerini döndürür.
        {
            try
            {
                var result = ConnectionGet().Query<T>(query);
                return result;
            }
            finally
            {
                connection.Close();//"finally" bloğu, bağlantının kapatılması için kullanılmaktadır.
            }
        }
        public int CmdExec(string query)//CmdExec metodu belirtilen sorguyu veritabanında çalıştırmak ve etkilenen satır sayısını döndürmektedir.
        {
            try
            {
                int result = ConnectionGet().Execute(query, null, null, 600000000);

                return result;
            }
            finally
            {
                connection.Close();//"finally" bloğu, bağlantının kapatılması için kullanılmaktadır.
            }
        }
    }
}
