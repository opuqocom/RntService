using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.Logger
{
    public class LoggerHelper
    {
        private ILog logger { get; set; }
        private StackTrace stackTrace { get; set; }
        private bool isLogEnabled { get; set; }
        public LoggerHelper(string uniqueField = "")
        {
            stackTrace = new StackTrace();//StackTrace, bir hata meydana geldiğinde programın hangi işlevlerin çağrıldığını ve hangi satırların çalıştığını takip etmek için kullanılır.

            var logFileName = stackTrace.GetFrame(1).GetMethod().Name;// Bu satır, önceki satırda oluşturulan StackTrace örneğindeki çağrı yığınının ikinci işlevini (yukarıdan aşağıya sayılırsa) alır ve ismini alır.
            if (!string.IsNullOrWhiteSpace(uniqueField))//Eğer uniqueField değeri boş değilse (yani bir değere sahipse) aşağıdaki kod bloğunu çalıştırır.
            {
                logFileName = uniqueField;//uniqueField değeri logFileName değerine atanır.
            }
            var path = ConfigurationManager.AppSettings["logPath"];// Bu kod satırı, uygulama yapılandırma dosyasındaki (app.config veya web.config) "logPath" anahtarına karşılık gelen değeri okur.
            log4net.GlobalContext.Properties["LogName"] = path + "/" + logFileName;//Bu satır, Log4net isimli bir kütüphanenin GlobalContext özelliğine "LogName" anahtarına karşılık gelen değeri atar. Bu değer, log dosyasının yolunu ve adını temsil eder

            log4net.Config.XmlConfigurator.Configure();//Bu satır, Log4net kütüphanesini yapılandırır. Yapılandırma dosyasındaki ayarlar yüklenir.
            logger = LogManager.GetLogger(stackTrace.GetFrame(1).GetMethod().DeclaringType);//: Bu satır, Log4net kütüphanesinden bir Logger örneği oluşturur. Örneğin, bu sınıfın bulunduğu sınıfın türünü alır.
            var logEnabled = ConfigurationManager.AppSettings["isLogEnabled"];// Bu satır, uygulama yapılandırma dosyasındaki "isLogEnabled" anahtarına karşılık gelen değeri okur.
            this.isLogEnabled = !string.IsNullOrEmpty(logEnabled) ? Convert.ToBoolean(logEnabled) : false;//Bu satır, "isLogEnabled" değerinin doğru bir şekilde ayarlanıp ayarlanmadığını kontrol eder. Eğer ayarlandıysa, değeri doğru bir şekilde ayarlar; aksi takdirde false değeri atanır.
        }

        public void traceInfo(string text)

        //Bu metot, bir metin parametresi alan ve log dosyasına bir bilgi(info) mesajı yazan bir metottur.isLogEnabled değişkeni true ise (yani loglama etkinleştirilmişse), logger nesnesi kullanılarak log dosyasına bir bilgi mesajı yazılır.Mesajın başına şu anın tarihi eklenir(DateTime.Now) ve sonrasında verilen text parametresi eklenir.
        {
            if (isLogEnabled)
                logger.Info(DateTime.Now + " - " + text);
        }
        public void traceInputsInfo<T>(T item, string text = "") where T : class//Bu metot, bir generic tipli item ve bir metin parametresi alan bir loglama yöntemidir. Bu metod, JsonConvert.SerializeObject kullanarak item parametresini JSON formatında serileştirir ve daha sonra text parametresine ekleyerek log dosyasına yazar. isLogEnabled değişkeni true ise (yani loglama etkinleştirilmişse), logger nesnesi kullanılarak log dosyasına bir bilgi mesajı yazılır. Mesajın başına şu anın tarihi eklenir (DateTime.Now) ve sonrasında text parametresi ve serileştirilen JSON nesnesi eklenir.
        {
            if (isLogEnabled)
                logger.Info(DateTime.Now + " - " + text + JsonConvert.SerializeObject(item));
        }
        public void traceError(string text)
        {
            //Bu metot, bir metin parametresi alan ve log dosyasına bir hata (error) mesajı yazan bir metottur. isLogEnabled değişkeni true ise (yani loglama etkinleştirilmişse), logger nesnesi kullanılarak log dosyasına bir hata mesajı yazılır. Mesajın başına şu anın tarihi eklenir (DateTime.Now) ve sonrasında verilen text parametresi eklenir.
            if (isLogEnabled)
                logger.Error(DateTime.Now + " - " + text);
        }
        public void traceFlush()
        {
            //Bu metot, loglama verilerinin tamponlanmasını boşaltmak için kullanılır.
            var test = logger.Logger.Repository;// logger'ın Repository özelliğine erişerek bir değişkene atama işlemi yapılır.
            LogManager.Flush(50000);// LogManager üzerinden 50.000 log kaydı yapıldıktan sonra tamponu boşaltma işlemi gerçekleştirilir.

            TextWriterAppender textWriterAppender = new TextWriterAppender();// TextWriterAppender nesnesi oluşturulur.
            textWriterAppender.Flush(100); // TextWriterAppender nesnesi üzerinden 100 log kaydı yapıldıktan sonra tamponu boşaltma işlemi gerçekleştirilir.
            IAppender[] logAppenders = logger.Logger.Repository.GetAppenders();// Logger'ın Repository'sinden tüm appender'ları içeren bir dizi oluşturulur.

            for (int i = 0; i < logAppenders.Length; i++)// Appender dizisinin tüm elemanları için döngü oluşturulur.
            {
                if (logAppenders[i] != null)// Appender elemanı null değilse
                {
                    var buffered = logAppenders[i] as RollingFileAppender;// Appender elemanı bir RollingFileAppender nesnesi ise bu nesneyi buffered değişkenine atar.

                    if (buffered is RollingFileAppender)// buffered değişkeni bir RollingFileAppender nesnesi ise
                    {
                        ((RollingFileAppender)buffered).Flush(1000);// RollingFileAppender nesnesinin tamponu 1000 log kaydı yapıldıktan sonra boşaltılır.
                    }
                }
            }
        }
    }
}
