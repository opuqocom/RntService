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
    public class LoginController : ApiController
    {
        /// <summary>
        /// Bearer Token Generate
        /// </summary>
        ///  
        [Route("api/Auth/Login")]
        [HttpPost]
        public IHttpActionResult Login(User user)
        {
            string result, token;
            if (user.UserName == "turasist" && user.Password == "Dv2@G22Qpj5@2")
            {
                token = JwtManager.GenerateToken(user.UserName, 30);
                result = token;
            }
            else
            {
                result = "Kullanıcı adı veya şifre geçersiz.";
            }

            return Json(result);
        }
    }
}
