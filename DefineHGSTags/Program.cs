using RntCar.BusinessLibrary.Estepe;
using RntCar.ClassLibrary;
using RntCar.ClassLibrary.Estepe.HGS;
using RntCar.ClassLibrary.HGS;
using RntCar.IntegrationHelper;
using RntCar.IntegrationHelper.HGSService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DefineHGSTags
{
    public class Program
    {
        public static IEstepeBL _estepeBL = new EstepeBL();

        static void Main(string[] args)
        {

            var HGSUnsuccessList = new List<HGSSendMailDto>();

            SaleProduct(HGSUnsuccessList);
            CancelProduct(HGSUnsuccessList);
            SendMail(HGSUnsuccessList);
          

        }

        private static void SendMail(List<HGSSendMailDto> HGSUnsuccessList)
        {

            var smtpSenderMail = ConfigurationManager.AppSettings.Get("SmtpSenderMail");//uygulamanın yapılandırma dosyasında tanımlanmış SmtpSenderMail anahtarının değerini alır.
            var smtpSenderPassword = ConfigurationManager.AppSettings.Get("SmtpSenderPassword");// uygulamanın yapılandırma dosyasında tanımlanmış SmtpSenderPassword anahtarının değerini alır.
            var smtpRecipientMails = ConfigurationManager.AppSettings.Get("SmtpRecipientMails");//uygulamanın yapılandırma dosyasında tanımlanmış SmtpRecipientMails anahtarının değerini alır.
            var host = "smtp.office365.com";//Bu değişken, SMTP sunucusunun adresini içerir.
            var port = 587;// Bu değişken, SMTP sunucusunun port numarasını içerir.

            var subject = "Rentgo Filo HGS-YKB Entegrasyonu";//Bu değişken, e-postanın konusunu içerir.
            var body = @"<table><tr><th></th><th>İşlem Tipi</th><th>Plaka</th><th>Etiket Id</th><th>Ruhsat Belge No</th><th>Hata Detayı</th></tr>";//Bu değişken, e-postanın içeriğini HTML formatında içerir. İçeriği, bir tablo şeklinde oluşturulur.
            foreach (var item in HGSUnsuccessList)
            {
                //Bu satır, body değişkenine bir satır eklemek için kullanılır. Bu satır, HTML formatında bir tablo satırıdır. Her bir tablo satırı, tr etiketi içinde yer alır. İlk sütunda, bir siyah nokta(bullet point) simgesi yer alır.Bu simge, td etiketi içinde yer alır. &#9; ise birkaç boşluk karakteri eklemek için kullanılan bir HTML karakter kodudur.Sonraki sütunlarda, item nesnesinin özellikleri kullanılarak, satırın diğer hücreleri doldurulur.Örneğin, item.Process ifadesi, item nesnesinin Process özelliğine karşılık gelir ve bu özellik, satırın ikinci sütununda yer alır.Satırın sonunda, </ tr > etiketi kullanılarak satır kapatılır.Bu satır, bir foreach döngüsü içinde kullanıldığı için, döngünün her bir öğesi için bir satır ekler. Bu şekilde, body değişkeni, HGSUnsuccessList listesindeki her bir öğe için bir satır içeren bir HTML tablosu olarak oluşturulur.



                body += $"<tr><td>• &#9;</td><td>{item.Process}</td><td>{item.PlateNo}</td><td>{item.ProductId}</td><td>{item.LicenseRegistrationDocumentNo}</td><td>{item.ExceptionDetail}</td></tr>";
            }
            body += "</table>";//bu ifade, body değişkeninin içeriğinin sonuna, bir </table> etiketi ekler. Bu, HTML tablosunun kapatılması için gereklidir.

            var client = new SmtpClient(host, port)//host değişkeni SMTP sunucusunun adresini, port değişkeni ise SMTP sunucusunun port numarasını belirtir.
            {
                Credentials = new NetworkCredential(smtpSenderMail, smtpSenderPassword),//Credentials özelliği, SMTP sunucusuna yetkilendirme yapmak için kullanılır. Bu özelliğe, NetworkCredential sınıfından bir örnek atanır. smtpSenderMail ve smtpSenderPassword değişkenleri, bu sınıfın yapıcı metoduna parametre olarak geçilir ve SMTP sunucusuna bağlanmak için kullanılır.
                EnableSsl = true//EnableSsl özelliği, SMTP bağlantısını güvenli bir SSL bağlantısı olarak ayarlamak için kullanılır. Bu özelliğin true olarak ayarlanması, e-posta gönderim işleminin güvenli bir şekilde gerçekleştirilmesini sağlar.
            };
            MailMessage mailMessage = new MailMessage();//MailMessage sınıfından yeni bir örnek oluşturulur
            mailMessage.From = new MailAddress(smtpSenderMail);//From özelliği, smtpSenderMail değişkenindeki e-posta adresi olarak ayarlanır
            mailMessage.To.Add(smtpRecipientMails);// To özelliği, e-postanın gönderileceği alıcı e-posta adresleri listesini belirtmek için kullanılır 
            mailMessage.Subject = subject;//Subject özelliği, e-postanın konusunu belirtmek için kullanılır ve subject değişkenindeki değer bu özelliğe atanır.
            mailMessage.Body = body;//Body özelliği, e-postanın içeriğini belirtmek için kullanılır ve body değişkenindeki HTML kodları bu özelliğe atanır. IsBodyHtml özelliği, e-posta içeriğinin HTML formatında olduğunu belirtmek için kullanılır ve bu özelliğin true olarak ayarlanması, HTML kodlarının e-posta içeriği olarak doğru şekilde işlenmesini sağlar.
            mailMessage.IsBodyHtml = true;//IsBodyHtml özelliği true olarak ayarlandığında, e-posta içeriği HTML formatında olacağı anlamına gelir.

            client.Send(mailMessage);//Son olarak, client.Send(mailMessage) ifadesi, SMTP sunucusu üzerinden e-postanın gönderilmesini sağlar. mailMessage değişkeni, SmtpClient sınıfının Send metoduna parametre olarak geçilir ve e-posta gönderme işlemi tamamlanır.
        }

        private static void CancelProduct(List<HGSSendMailDto> HGSUnsuccessList)//Bu kod bloğu, HGS etiketlerinin iptal edilmesini sağlar. //Bu kod bloğu, _estepeBL sınıfındaki GetHGSDefineList() metodu aracılığıyla HGS etiketlerinin listesini alır. Daha sonra her bir HGS etiketi için cancelProduct() metodunu kullanarak iptal işlemi yapılır.
        {
            var HGSCancelList = _estepeBL.GetHGSDefineList((short)HGSListType.Cancel);//HGSCancelList değişkenine, HGS etiketlerinin listesi atanır.
            foreach (var hgsCancel in HGSCancelList)
            {
                HGSHelper hGSHelper = new HGSHelper();
                CancelProductParameter cancelProductParameter = new CancelProductParameter()
                {
                    cancelReason = 5,//diğer
                    productId = hgsCancel.serino
                };

                var result = hGSHelper.cancelProduct(cancelProductParameter);//hGSHelper nesnesinin cancelProduct metodu, yukarıdaki parametreleri alarak çağrılır ve bir sonuç nesnesi (result) döndürür.
                _estepeBL.InsertHgsResult(new HGS_INTEGRATION_RESULT() { OGS_KGS_NO = hgsCancel.ogskgsno, PROCCESS_TYPE = "Cancel", RESULT = result.ResponseResult.Result, EXCEPTION_DETAIL = result.ResponseResult.ExceptionDetail });//Bu kod, HGS etiketlerinin iptal işlemi sonrasında, iptal işleminin sonucunu (başarılı/başarısız) ve herhangi bir hata durumunda hata ayrıntılarını veritabanına kaydeder.
                //HGS_INTEGRATION_RESULT bir veritabanı tablosudur ve bu tabloya ait bir örnek oluşturularak, new HGS_INTEGRATION_RESULT() şeklinde kullanılır. Bu örneğin OGS_KGS_NO ve PROCCESS_TYPE alanlarına, iptal işlemi yapılan etiketin bilgileri kaydedilir. RESULT alanı, iptal işleminin başarılı olup olmadığını tutar. EXCEPTION_DETAIL ise, herhangi bir hata durumunda hata ayrıntılarını tutar.

                if (!result.ResponseResult.Result)
                {
                    //Bu kod bloğu, HGS etiketlerinin iptal edilmesi işlemi sırasında oluşan hataları kontrol eder. Eğer işlem başarısız olursa, HGSUnsuccessList listesine yeni bir HGSSendMailDto öğesi ekler. Bu öğede ilgili HGS etiketinin plakası(PlateNo), seri numarası(ProductId), ruhsat / tescil belge numarası(LicenseRegistrationDocumentNo) ve hata detayı(ExceptionDetail) tutulur.Bu listedeki öğeler daha sonra bir e - posta göndermek için kullanılacaktır.

                    HGSUnsuccessList.Add(new HGSSendMailDto() { Process = "Etiket İptal", PlateNo = hgsCancel.plaka, ProductId = hgsCancel.serino, LicenseRegistrationDocumentNo = hgsCancel.ruhsat_tescil_belgeno, ExceptionDetail = result.ResponseResult.ExceptionDetail });
                }
            }
        }

        private static void SaleProduct(List<HGSSendMailDto> HGSUnsuccessList)
        {
            var HGSSaleList = _estepeBL.GetHGSDefineList((short)HGSListType.Sale);//Satış yapılacak HGS etiketlerinin listesi HGSSaleList değişkenine atanır.
            foreach (var hgsSale in HGSSaleList)
            {
                HGSHelper hGSHelper = new HGSHelper();
                SaleProductParameter saleProductParameter = new SaleProductParameter()
                {
                    licenseNo = hgsSale.ruhsat_tescil_belgeno,//Satış yapılacak aracın ruhsat tescil belgesi numarası atanır.
                    plateNo = hgsSale.plaka,//Satış yapılacak aracın plaka numarası atanır.
                    productId = hgsSale.serino,// Satış yapılacak HGS etiketinin seri numarası atanır.
                    productType = "E",//Ürün tipi belirtilir. Burada "E" harfi, HGS etiketini temsil eder.
                    vehicleClass = 1// Satış yapılacak aracın sınıfı belirtilir. Burada 1, aracın binek araç olduğunu gösterir.
                };
                var result = hGSHelper.saleProduct(saleProductParameter);// satış işlemi gerçekleştirilir. Sonuçlar result değişkenine atanır.
                _estepeBL.InsertHgsResult(new HGS_INTEGRATION_RESULT() { OGS_KGS_NO = hgsSale.ogskgsno, PROCCESS_TYPE = "Sale", RESULT = result.ResponseResult.Result, EXCEPTION_DETAIL = result.ResponseResult.ExceptionDetail });//Satış sonucu HGS_INTEGRATION_RESULT nesnesi olarak veritabanına kaydedilir.
                if (!result.ResponseResult.Result)// Eğer satış başarısız olursa, aşağıdaki kodlar çalıştırılır:
                {
                    HGSUnsuccessList.Add(new HGSSendMailDto() { Process = "Etiket Tanımlama", PlateNo = hgsSale.plaka, ProductId = hgsSale.serino, LicenseRegistrationDocumentNo = hgsSale.ruhsat_tescil_belgeno, ExceptionDetail = result.ResponseResult.ExceptionDetail });// Başarısız olan satış işlemi için bir HGSSendMailDto nesnesi oluşturulur ve bu nesne HGSUnsuccessList listesine eklenir.

                }
            }
        }
    }
}
