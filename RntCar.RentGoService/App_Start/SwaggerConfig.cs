using System.Web.Http;
using WebActivatorEx;
using RntCar.RentGoService;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Reflection;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace RntCar.RentGoService
{
    //Bu kod bir Swagger yapılandırma sınıfıdır. Swagger, RESTful web hizmetlerini tanımlamak, belgelemek ve test etmek için kullanılan bir araçtır. Bu kodda SwaggerConfig sınıfı bulunur ve statik Register() metodu tarafından çağrılır
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;//İlk önce typeof(SwaggerConfig).Assembly kullanılarak, SwaggerConfig sınıfının bulunduğu derleme yani assembly alınır.
            //Daha sonra GlobalConfiguration.Configuration.EnableSwagger() kullanılarak Swagger yapılandırması ayarlanır. 
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "RntCar.RentGoService");//c.SingleApiVersion() kullanılarak, API sürümü belirtilir.


                    //Token için eklendi.
                    c.ApiKey("Authorization")//Token doğrulaması için c.ApiKey() kullanılarak bir "Bearer" token'ı tanımlanır ve açıklaması girilir. Bu token, header'da "Authorization" adı altında gönderilecektir.
                    .Description("Filling bearer token here")
                    .Name("Bearer")
                    .In("header");


                    //Summary için eklendi. //Uyarı mesajlarını kaldırmak için Projenin Proproty'sinde ki Build'in altınada ki warning'e 1591 eklenmelidir. 
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"bin\";//XML belgeleme dosyasının yolu için bir değişken oluşturur.İlk olarak, AppDomain.CurrentDomain.BaseDirectory ile uygulamanın çalıştığı dizin alınır ve bin\ alt dizini eklenir. Bu adım, XML belgeleme dosyasının varsayılan olarak uygulamanın bin klasöründe bulunacağı varsayımına dayanmaktadır.
                    var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";//Daha sonra, Assembly.GetExecutingAssembly().GetName().Name + ".XML" kullanılarak, mevcut yürütülebilir dosyanın adı ve ".XML" uzantısı eklenerek yorumlar dosyasının adı oluşturulur.
                    var commentsFile = Path.Combine(baseDirectory, commentsFileName);//Son olarak, Path.Combine() yöntemi kullanılarak, baseDirectory ve commentsFileName değişkenleri birleştirilerek yorumlar dosyasının tam yolunu elde ederiz. Bu yol, IncludeXmlComments() yöntemi tarafından kullanılır ve Swagger UI'da görüntülenen belgeleri oluşturmak için yorumlar dosyası kullanılır.
                    c.IncludeXmlComments(commentsFile);//Ayrıca c.IncludeXmlComments() metodu kullanılarak, XML belgelemesi dosyası (yorumlar dosyası) okunarak Swagger UI'da görüntülenen dokümantasyona dahil edilir.


                })//Son olarak, GlobalConfiguration.Configuration.EnableSwaggerUi() çağrılır ve burada c.EnableApiKeySupport() kullanılarak, "Bearer" token'ın kullanımı etkinleştirilir.
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }
    }
}
