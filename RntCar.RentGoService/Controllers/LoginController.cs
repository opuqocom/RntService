using Newtonsoft.Json.Linq;
using RntCar.ClassLibrary.Estepe.Login;
using RntCar.RentGoService.Authentication.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RntCar.RentGoService.Controllers
{
    //Bu kod, bir LoginController sınıfını ve bu sınıfın içindeki bir Login() adlı bir HttpPost yöntemini içerir. Bu yöntem, bir kullanıcı adı ve şifre alır ve doğru kimlik bilgileri girilirse, bir Bearer token oluşturur ve bu token'ı JSON olarak döndürür.
    public class LoginController : ApiController
    {
        /// <summary>
        /// Bearer Token Generate
        /// </summary>
        ///  
        [Route("api/Auth/Login")]//Yöntemin URL'sini belirlemiş olduk(/api/Auth/Login) 
        [HttpPost]
        public IHttpActionResult Login(User user)
        {
            //Parametre olarak bir User nesnesi alır ve girilen kullanıcı adı ve şifreyi doğrular.    Not: Bu kodda JwtManager bir üçüncü taraf kütüphanesi olabilir ve token oluşturma işlemini gerçekleştirmek için kullanılabilir.

            string result, token;

            //Eğer girilen bilgiler doğru ise, JwtManager.GenerateToken() yöntemi kullanılarak bir Bearer token oluşturulur.
            if (user.UserName == "turasist" && user.Password == "Dv2@G22Qpj5@2")
            {
                token = JwtManager.GenerateToken(user.UserName, 30);
                //Bu yönteme, kullanıcının adı ve bir son kullanma tarihi (30 dakika sonrası) verilir.
                result = token;//Token oluşturulduktan sonra, bu token bir değişkene atanır 
            }
            else
            {
                //Eğer kullanıcı adı veya şifre yanlış ise, bir hata mesajı "Kullanıcı adı veya şifre geçersiz." oluşturulur 
                result = "Kullanıcı adı veya şifre geçersiz.";
            }

            return Json(result);//her iki durumdada( if yada else ), değişkene atanmış olan token jsona dönüştürülürüp, döndürülür.
        }
    }
}
