using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RntCar.RentGoService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        //Register adlı metot, yapılandırmayı gerçekleştirmek için kullanılmaktadır. Bu metot, config parametresi aracılığıyla gelen yapılandırma ayarlarını uygulamaktadır.
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            //Bu kodda, önce config.MapHttpAttributeRoutes() metodu çağrılarak, Web API yönlendirmeleri için kullanılacak olan URL rotaları tanımlanmaktadır. 
            //Daha sonra config.Routes.MapHttpRoute() metodu ile varsayılan bir URL şablonu belirtilmektedir. Bu şablon, api/{controller}/{action}/{id} olarak tanımlanmıştır ve bu şablon ile belirtilen parametreler, Web API çağrılarından gelen URL'lerle eşleştirilecektir. 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }//Son olarak, RouteParameter.Optional parametresi, URL şablonundaki id parametresinin varsayılan değerinin belirtilmesini sağlamaktadır.

            );

        }
    }
}
