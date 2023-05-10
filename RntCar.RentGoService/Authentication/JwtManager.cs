using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RntCar.RentGoService.Authentication.Manager
{
    public class JwtManager
    {
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";
        //Bu kod, kullanıcının oturum açtıktan sonra JWT (JSON Web Token) oluşturmasını sağlar. 
        public static string GenerateToken(string username, int expireMinutes = 240)
        {
            var symmetricKey = Convert.FromBase64String(Secret);//bu kod simetrik bir anahtarı byte dizisi olarak elde etmek için kullanılır. Bu anahtar, Secret adlı bir string değişkeninde saklanmıştır.
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {//SecurityTokenDescriptor nesnesinin Subject özelliği, JWT'nin özniteliklerini belirler. Bu örnekte, JWT'ye ClaimTypes.Name ve ClaimTypes.Role öznitelikleri eklenir
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username), //Oturum açan kullanıcı adını ya da maili bu aşamada ekleniyor
                            new Claim(ClaimTypes.Role, "Role") // İstenirse rol/roller eklenebilir.
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),//JWT'nin geçerlilik süresi (Expires) belirlenir. Bu örnekte, JWT, oturum açma işleminden expireMinutes dakika sonra geçerli olmayacak şekilde ayarlanır.

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                //SigningCredentials özelliği, JWT'nin imzalanması için kullanılan algoritma ve anahtar bilgilerini içerir. Bu örnekte, JWT SecurityAlgorithms.HmacSha256Signature algoritması ile imzalanır.
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);// JWT'nin oluşturulmasını sağlar ve securityToken değişkenine atar.
            var token = tokenHandler.WriteToken(securityToken);//securityToken değişkenindeki JWT'yi string bir formatta elde etmek için kullanılır

            return token;// Bu string, son olarak kullanıcıya döndürülür ve JWT, kullanıcının oturum açtığını doğrulamak için kullanılabilir.
        }


        public static ClaimsPrincipal GetPrincipal(string token)//Bu kod, bir JWT token'ı alır ve bu token'ı kullanarak bir JWT doğrulama işlemi gerçekleştirir. JWT doğrulama işlemi, JWT'nin geçerliliğini doğrular ve JWT içeriğinden gerekli bilgileri çıkarır.
                        //Metodun girişi, string token adlı bir parametre içerir, bu parametre JWT'yi temsil eder.
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();     
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;//yandaki kod JWT'nin okunması ve JwtSecurityToken nesnesine dönüştürülmesi işlemini gerçekleştirir.

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);//Bu kod, JWT doğrulama işlemi için kullanılan bir simetrik anahtarın oluşturulmasını sağlar. Bu anahtar, Secret adlı bir string değişkeninde saklanmıştır. Convert.FromBase64String() yöntemi, bir Base64 kodlamalı stringi byte dizisine dönüştürür.Secret değişkeni, Base64 kodlamalı bir string olarak tanımlanmıştır, bu nedenle Convert.FromBase64String(Secret) ifadesi, Secret string değişkeninin byte dizisine dönüştürülmesini sağlar. Bu anahtar, daha sonra TokenValidationParameters nesnesinde IssuerSigningKey özelliği olarak kullanılır.Bu özellik, JWT'nin doğrulanması için kullanılacak anahtarı belirtir.





                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,//Geçerli bir JWT'nin bir son kullanma tarihinin olması gerektiğini belirtir.
                    ValidateIssuer = false,//JWT'nin gönderenini doğrulamak için kullanılır. Bu örnek için false olarak belirlenmiştir.
                    ValidateAudience = false,// JWT'nin alıcısını doğrulamak için kullanılır. Bu örnek için false olarak belirlenmiştir.
                    LifetimeValidator = LifetimeValidator, //Token'nın geçerlilik zamanını kontrol ediyoruz.
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)//: JWT'nin doğrulanması için kullanılan simetrik anahtarı belirtir.
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);//JWT'nin doğrulanması gerçekleştirilir. Bu yöntem, JWT token'ını, doğrulama parametrelerini ve bir out parametresi kullanarak doğrular.

                return principal;//En son olarak, doğrulama işlemi başarılıysa, ValidateToken() yöntemi bir ClaimsPrincipal nesnesi döndürür. Bu nesne, JWT içeriğinde bulunan taleplerin listesini içerir. Eğer bir hata oluşursa, metod null değerini döndürür.
            }

            catch (Exception)
            {
                return null;
            }
        }


        //Aşağıdaki kod, bir JWT (JSON Web Token) nesnesinin ömrünü doğrulayan bir yöntem içerir. Bu yöntem, JWT'nin geçerli olup olmadığını belirlemek için bir doğrulama işlemi yapar.
        //Token'nın geçerlilik zamanını kontrol ediyoruz.
        static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        //notBefore: JWT'nin geçerli olmaya başlayacağı tarih ve saat. Bu tarih ve saat, UTC zaman diliminde bir DateTime nesnesi olarak temsil edilir. Bu parametre null olabilir.
        //expires: JWT'nin geçerli olacağı son tarih ve saat. Bu tarih ve saat, UTC zaman diliminde bir DateTime nesnesi olarak temsil edilir. Bu parametre null olabilir.
        //securityToken: doğrulanacak JWT nesnesi.
        //validationParameters: JWT doğrulama parametreleri.
        //Yöntem, JWT'nin ömrünü doğrulamak için expires ve notBefore değerlerini kullanır. 
        {

            if (expires != null)
            //Eğer expires değeri null değilse ve geçerli tarih ve saat, expires değerinden önce ise, yöntem true değerini döndürür.
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;//Aksi takdirde, yöntem false değerini döndürür.
        }
        // Bu yöntem, bir JWT'nin geçerliliğini doğrulamak için daha geniş bir JWT doğrulama işlemi içinde kullanılabilir.
    }
}