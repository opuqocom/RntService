using Newtonsoft.Json;
using RntCar.ClassLibrary.Estepe;
using RntCar.ClassLibrary.Estepe.HGS;
using RntCar.Logger;
using System.ServiceModel;
using System;
using System.Configuration;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;
using RntCar.IntegrationHelper.HGSService;

namespace RntCar.IntegrationHelper
{
    public class HGSHelper
    {
        private HGSService.HgsWebUtilServicesClient hgsWebUtilServicesClient { get; set; }
        private string[] loginInfo { get; set; }
        private string endpointUrl { get; set; }
        private OperationContextScope scope { get; set; }
        private static EndpointAddress myEndpointAddress { get; set; }
        private static BasicHttpBinding myBasicHttpBinding { get; set; }

        public HGSHelper()
        {
            this.prepareServiceConfiguration();//Bu metod, web servis istemcisi için gerekli olan ayarları yapar.
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;//özelliğini TLS 1.1 ve TLS 1.2 ile ayarlar. Bu, web servisi çağırmak için gerekli olan güvenlik protokollerini belirler.

            hgsWebUtilServicesClient = new HGSService.HgsWebUtilServicesClient(myBasicHttpBinding, myEndpointAddress);//HGSService.HgsWebUtilServicesClient nesnesi oluşturur. Bu, web servisi çağırmak için kullanılacak olan istemcidir.
            scope = new OperationContextScope(hgsWebUtilServicesClient.InnerChannel);//scope adında bir OperationContextScope nesnesi de oluşturur. Bu nesne, web servisi çağrısında kullanılan OperationContext nesnesini temsil eder.
            var sec = new SecurityHeader(loginInfo[0], loginInfo[1]);//loginInfo adında bir dizi değişken tanımlanır ve özellikleri aracılığıyla sec adında bir SecurityHeader nesnesi oluşturulur. Bu nesne, web servis isteğine giriş bilgilerini eklemek için kullanılır.
            OperationContext.Current.OutgoingMessageHeaders.Add(sec);//çağrılan web servis isteği başlığına eklenir. Bu, web servisi çağrısının kimliğini doğrular.
        }

        public SaleProductResponse saleProduct(SaleProductParameter saleProductParameter)//Bu kod bloğu bir HGS ürününü satın almak için gerekli olan servis çağrısını yapar ve çağrı sonrasında gelen cevabı kontrol eder.
        {
            LoggerHelper loggerHelper = new LoggerHelper();
            var convertedParameter = new HGSService.requestSaleProductWEB// çağrının parametreleri HGS servisi tarafından kabul edilen parametreler şeklinde dönüştürülür ve değişkene atanır.
            {

                licenseNo = saleProductParameter.licenseNo,
                plateNo = saleProductParameter.plateNo,
                productId = saleProductParameter.productId,
                productType = saleProductParameter.productType,
                vehicleClass = saleProductParameter.vehicleClass,
                vehicleClassSpecified = true,
            };
            loggerHelper.traceInfo("Parameter " + JsonConvert.SerializeObject(saleProductParameter));

            var response = hgsWebUtilServicesClient.saleProduct(convertedParameter);//çağrının yapıldığı servis methoduna parametre olarak dönüştürülmüş nesne gönderilir ve cevap değişkene atanır.

            loggerHelper.traceInfo("Response " + JsonConvert.SerializeObject(response));

            if (!response.errorCode.Equals("000")) // 000 means there is no error
            {//Daha sonra, gelen cevabın hata kodu "000" eşit değilse, ResponseResult sınıfının ReturnError metoduna hata ayrıntılarıyla birlikte cevap gönderilir. 
                return new SaleProductResponse
                {
                    ResponseResult = ResponseResult.ReturnError(response.errorInfo + " Parameter " + JsonConvert.SerializeObject(saleProductParameter))//hata ayrıntıları gönderilir
                };
            }

            return new SaleProductResponse
            {
                ResponseResult = ResponseResult.ReturnSuccess()//Aksi takdirde, ResponseResult sınıfının ReturnSuccess metoduna cevap gönderilir.
            };
        }
        public CancelProductResponse cancelProduct(CancelProductParameter cancelProductParameter)//Bu metod, "HGSService" adlı bir servis yardımıyla bir ürün iptal işlemi gerçekleştirir
        {
            LoggerHelper loggerHelper = new LoggerHelper();//İlk olarak, "LoggerHelper" sınıfından bir nesne oluşturulur. Bu nesne, ileride oluşabilecek hataları izlemeye yardımcı olmak için kullanılır.
            var convertedParameter = new HGSService.requestCancelProductWEB//Daha sonra, "convertedParameter" adlı bir değişken oluşturulur ve "requestCancelProductWEB" tipinde bir nesne atanır. Bu nesnenin özellikleri, "cancelProductParameter" adlı parametrenin özelliklerine atanır.
            {
                productId = cancelProductParameter.productId,
                cancelReason = cancelProductParameter.cancelReason,
                cancelReasonSpecified = true
            };

            loggerHelper.traceInfo("Parameter " + JsonConvert.SerializeObject(cancelProductParameter));//"loggerHelper" nesnesi, "cancelProductParameter" adlı parametrenin özelliklerinin seri hale getirilmiş halini izlemek için kullanılır.

            var response = hgsWebUtilServicesClient.cancelProduct(convertedParameter);//Sonra, "hgsWebUtilServicesClient" nesnesi kullanılarak "cancelProduct" metodu çağrılır ve "convertedParameter" değişkeni kullanılarak bir "response" adlı değişkene atanır.

            loggerHelper.traceInfo("Response " + JsonConvert.SerializeObject(response));//Bir sonraki adımda, "loggerHelper" nesnesi, "response" değişkeninin seri hale getirilmiş halini izlemek için kullanılır.


            if (!response.errorCode.Equals("000")) // 000 means there is no error
            //En son olarak, "response" değişkenindeki "errorCode" özelliği kontrol edilir. Eğer bu özellik "000" değilse, bir hata olduğu anlaşılır ve "CancelProductResponse" tipinde bir değer döndürülür.
            {
                return new CancelProductResponse
                {
                    ResponseResult = ResponseResult.ReturnError(response.errorInfo + " Parameter " + JsonConvert.SerializeObject(cancelProductParameter))// Bu değer, "ResponseResult" özelliği "ReturnError" değeri ile atanır ve hata mesajı ile birlikte "cancelProductParameter" parametresinin seri hale getirilmiş hali de dahil edilir. 
                };
            }

            return new CancelProductResponse//Ancak, eğer "errorCode" özelliği "000" ise, başarılı bir işlem gerçekleştirilmiştir ve "CancelProductResponse" tipinde bir değer döndürülür. Bu değer, "ResponseResult" özelliği "ReturnSuccess" değeri ile atanır.
            {
                ResponseResult = ResponseResult.ReturnSuccess()
            };
        }

        private void prepareServiceConfiguration()//Bu metod, HGS web servisini yapılandırmak için kullanılır.
        {
            loginInfo = ConfigurationManager.AppSettings["hgsServiceLoginInfo"].Split(';'); //İlk olarak, web servisi için kullanılacak kullanıcı adı ve şifre, konfigürasyon dosyasından hgsServiceLoginInfo anahtar kelimesiyle alınır.
            endpointUrl = ConfigurationManager.AppSettings["hgsEndPointUrl"]; //Daha sonra, web servisinin adresi de konfigürasyon dosyasından hgsEndPointUrl anahtar kelimesiyle alınır.
            myBasicHttpBinding = new BasicHttpBinding();//Ardından, myBasicHttpBinding isimli yeni bir BasicHttpBinding örneği oluşturulur. Bu binding, web servisi için gerekli ayarları barındırır:
            myBasicHttpBinding.Name = "HGSServiceSoap";   //Name özelliği, binding'in adını belirtir.
            myBasicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport;//  Security.Mode özelliği, transport seviyesinde güvenlik kullanılacağını belirtir.
            myBasicHttpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;//Security.Message.ClientCredentialType özelliği, kullanıcı adı / şifre kimlik doğrulama türünü belirtir.
            myBasicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(10);//OpenTimeout, CloseTimeout, ReceiveTimeout ve SendTimeout özellikleri, ilgili işlemlerin zaman aşımı sürelerini belirtir.
            myBasicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.SendTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.MaxReceivedMessageSize = 2147483647;////MaxReceivedMessageSize özelliği, en büyük alınabilecek mesaj boyutunu belirtir.
            myEndpointAddress = new EndpointAddress(endpointUrl);//Son olarak, myEndpointAddress adlı yeni bir EndpointAddress örneği oluşturulur. Bu örnek, web servisinin adresini ve binding'in özelliklerini içerir.
        }

        ~HGSHelper()//HGSHelper() yapıcı, bir nesne örneği oluşturulduğunda çağrılır ve bir Dispose(false) çağrısı yaparak nesneyi yok etmeye hazırlar.
        {
            Dispose(false);
        }
        public void Dispose()//Dispose() yöntemi, IDisposable arayüzü tarafından tanımlanır ve HGSHelper nesnesinin kullanımdan kaldırılmasını sağlar. Dispose(true) yöntemi çağrıldığında, hgsWebUtilServicesClient ve scope nesneleri bellekten serbest bırakılır. Ayrıca, GC.SuppressFinalize(this) yöntemi, nesnenin nihai temizlenmesi gerektiğini belirtir.
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)//Dispose(bool disposing) yöntemi, IDisposable arayüzü tarafından tanımlanır ve gerçek kaynakları serbest bırakmak için kullanılır. 'disposing' değeri true olduğunda, IDisposable.Dispose() yöntemi çağrıldığından, tüm yönetilmeyen kaynaklar (genellikle nesneler) serbest bırakılır ve nesne yok edilir. 'disposing' değeri false olduğunda, sadece yönetilmeyen kaynaklar serbest bırakılır. Bu yöntem, nesnenin doğrudan silinmesiyle veya Garbage Collector tarafından otomatik olarak silinmesiyle çağrılabilir.
        {
            if (disposing)
            {
                this.hgsWebUtilServicesClient = null;
                this.scope = null;
            }
        }

        internal class SecurityHeader : MessageHeader
        //Bu kod parçası, bir özel mesaj başlığı oluşturur. Bu başlık, web servislerinde kimlik doğrulama ve güvenlik için kullanılan WS-Security standardını uygulayan bir kullanıcı adı ve parolası içerir.
        {
            //Kod, SecurityHeader adında bir sınıf tanımlar. Bu sınıf, MessageHeader sınıfından türetilir ve bir Name ve Namespace özelliği ile bir headerString özelliği içerir. 

            private const string HeaderName = "Security";
            private const string HeaderNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            public override string Name => HeaderName;// Name ve Namespace, oluşturulan özel başlığın adını ve ad alanını belirtir.
            public override string Namespace => HeaderNamespace;
            public string headerString;//headerString, kullanıcı adı, şifre, nonce (rastgele sayısal bir değer) ve oluşturma tarihinden oluşan bir XML dizesini içerir

            public SecurityHeader(string username, string password)
            //SecurityHeader sınıfının constructor'ı, kullanıcı adı ve şifreyi alır.
            {
                //Constructor, GUID kullanarak bir nonce oluşturur ve DateTime.Now kullanarak oluşturma tarihini belirler. headerString, bu bilgileri içeren XML dizesi olarak ayarlanır. 
                Guid guid = Guid.NewGuid();
                this.headerString = @"<UsernameToken xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""> 
                <Username>" + username + @"</Username> 
                <Password>" + password + @"</Password> 
                <Nonce >" + guid.ToString() + @"</Nonce> 
                <Created>" + DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") + @"</Created> </UsernameToken>";
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            // OnWriteHeaderContents yöntemi, MessageHeader sınıfından geçersiz kılınır ve belirtilen XmlDictionaryWriter nesnesi ve mesaj sürümüne göre headerString özelliğindeki XML dizesini yazdırır.Bu, özel başlık mesajına eklenir ve web servisine gönderilir.
            {
                var r = XmlReader.Create(new StringReader(headerString));
                r.MoveToContent();
                writer.WriteNode(r, false);
            }
        }
    }
}
