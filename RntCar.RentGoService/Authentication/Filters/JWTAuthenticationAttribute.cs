using RntCar.RentGoService.Authentication.Manager;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace RntCar.RentGoService.Authentication.Filters
{
    public class JWTAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)//OnAuthorization metodu, AuthorizationFilterAttribute sınıfının bir yöntemidir ve HTTP isteklerinin kimlik doğrulama filtresi tarafından ele alınmasını sağlar.
        {
            //Gelen request'i kontrol editoruz. Authorization boş ise direk 401 Unauthorized veriyoruz.
            if (actionContext.Request.Headers.Authorization == null)//if bloğu, gelen HTTP isteğinde "Authorization" başlığının boş olup olmadığını kontrol eder. Eğer boşsa, HTTP 401 "Unauthorized" yanıtı verilir. Bu, kimlik doğrulama başarısız olduğunda geri dönülecek yanıtı belirtir.
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);//CreateResponse() metodu, HTTP yanıtı nesnesinin oluşturulmasını sağlar. Bu yöntem, belirtilen HTTP yanıt kodu ve içerikle birlikte yeni bir HttpResponseMessage nesnesi oluşturur ve geri döndürür.
            }
            //Eğer doluysa geçerliliğini kontrol ediyoruz.
            else//else bloğunda ise, "Authorization" başlığı doluysa, JWT token'in geçerliliğini kontrol etmek için birkaç adım atılır. İlk olarak, JWT token'i alınır ve decode edilir.
            {
                var tokenKey = actionContext.Request.Headers.Authorization.Parameter;//gelen HTTP isteğinin "Authorization" başlığından token'ı alır. 
                var decodeToken = JwtManager.GetPrincipal(tokenKey);//Bu kod, JwtManager.GetPrincipal() metodu kullanılarak JWT token'ın decode edilmesini ve token'ın içeriğinin çözümlenmesini sağlar.  
                if (decodeToken == null)//JWT token'in geçerliliği decodeToken ile kontrol edilir ve eğer geçerli değilse, yine HTTP 401 "Unauthorized" yanıtı verilir.
                {
                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);//CreateResponse() metodu, HTTP yanıtı nesnesinin oluşturulmasını sağlar. Bu yöntem, belirtilen HTTP yanıt kodu ve içerikle birlikte yeni bir HttpResponseMessage nesnesi oluşturur ve geri döndürür.

                }
            }
        }
    }
}