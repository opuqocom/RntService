using RntCar.RentGoService.Authentication.Manager;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace RntCar.RentGoService.Authentication.Filters
{
    public class JWTAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //Gelen request'i kontrol editoruz. Authorization boş ise direk 401 Unauthorized veriyoruz.
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            //Eğer doluysa geçerliliğini kontrol ediyoruz.
            else
            {
                var tokenKey = actionContext.Request.Headers.Authorization.Parameter;
                var decodeToken = JwtManager.GetPrincipal(tokenKey);
                if (decodeToken == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);

                }
            }
        }
    }
}