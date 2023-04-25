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
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "RntCar.RentGoService");
                         

                        //Token için eklendi.
                        c.ApiKey("Authorization")
                        .Description("Filling bearer token here")
                        .Name("Bearer")
                        .In("header");


                        //Summary için eklendi. //Uyarı mesajlarını kaldırmak için Projenin Proproty'sinde ki Build'in altınada ki warning'e 1591 eklenmelidir. 
                        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                        var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                        var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                        c.IncludeXmlComments(commentsFile);


                    })
                .EnableSwaggerUi(c =>
                    {
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
