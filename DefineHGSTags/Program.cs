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
            HGSHelper hGSHelper = new HGSHelper();
            var HGSUnsuccessList = new List<HGSSendMailDto>();

            SaleProduct(hGSHelper, HGSUnsuccessList);
            CancelProduct(hGSHelper, HGSUnsuccessList);
            SendMail(HGSUnsuccessList);

        }

        private static void SendMail(List<HGSSendMailDto> HGSUnsuccessList)
        {
            var smtpSenderMail = ConfigurationManager.AppSettings.Get("SmtpSenderMail");
            var smtpSenderPassword = ConfigurationManager.AppSettings.Get("SmtpSenderPassword");
            var smtpRecipientMails = ConfigurationManager.AppSettings.Get("SmtpRecipientMails");
            var host = "smtp.office365.com";
            var port = 587;

            var subject = "Rentgo Filo HGS-YKB Entegrasyonu";
            var body = @"<table><tr><th></th><th>İşlem Tipi</th><th>Plaka</th><th>Etiket Id</th><th>Ruhsat Belge No</th><th>Hata Detayı</th></tr>";
            foreach (var item in HGSUnsuccessList)
            {
                body += $"<tr><td>• &#9;</td><td>{item.Process}</td><td>{item.PlateNo}</td><td>{item.ProductId}</td><td>{item.LicenseRegistrationDocumentNo}</td><td>{item.ExceptionDetail}</td></tr>";
            }
            body += "</table>";

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(smtpSenderMail, smtpSenderPassword),
                EnableSsl = true
            };
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(smtpSenderMail);
            mailMessage.To.Add(smtpRecipientMails);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            client.Send(mailMessage);
        }

        private static void CancelProduct(HGSHelper hGSHelper, List<HGSSendMailDto> HGSUnsuccessList)
        {
            var HGSCancelList = _estepeBL.GetHGSDefineList((short)HGSListType.Cancel);
            foreach (var hgsCancel in HGSCancelList)
            {
                CancelProductParameter cancelProductParameter = new CancelProductParameter()
                {
                    cancelReason = 5,//diğer
                    productId = hgsCancel.serino
                };

                var result = hGSHelper.cancelProduct(cancelProductParameter);
                _estepeBL.InsertHgsResult(new HGS_INTEGRATION_RESULT() { OGS_KGS_NO = hgsCancel.ogskgsno, PROCCESS_TYPE = "Cancel", RESULT = result.ResponseResult.Result, EXCEPTION_DETAIL = result.ResponseResult.ExceptionDetail });
                if (!result.ResponseResult.Result)
                {
                    HGSUnsuccessList.Add(new HGSSendMailDto() { Process = "Etiket İptal", PlateNo = hgsCancel.plaka, ProductId = hgsCancel.serino, LicenseRegistrationDocumentNo = hgsCancel.ruhsat_tescil_belgeno, ExceptionDetail = result.ResponseResult.ExceptionDetail });
                }
            }
        }

        private static void SaleProduct(HGSHelper hGSHelper, List<HGSSendMailDto> HGSUnsuccessList)
        {
            var HGSSaleList = _estepeBL.GetHGSDefineList((short)HGSListType.Sale);
            foreach (var hgsSale in HGSSaleList)
            {
                SaleProductParameter saleProductParameter = new SaleProductParameter()
                {
                    licenseNo = hgsSale.ruhsat_tescil_belgeno,
                    plateNo = hgsSale.plaka,
                    productId = hgsSale.serino,
                    productType = "E",
                    vehicleClass = 1
                };
                var result = hGSHelper.saleProduct(saleProductParameter);
                _estepeBL.InsertHgsResult(new HGS_INTEGRATION_RESULT() { OGS_KGS_NO = hgsSale.ogskgsno, PROCCESS_TYPE = "Sale", RESULT = result.ResponseResult.Result, EXCEPTION_DETAIL = result.ResponseResult.ExceptionDetail });
                if (!result.ResponseResult.Result)
                {
                    HGSUnsuccessList.Add(new HGSSendMailDto() { Process = "Etiket Tanımlama", PlateNo = hgsSale.plaka, ProductId = hgsSale.serino, LicenseRegistrationDocumentNo = hgsSale.ruhsat_tescil_belgeno, ExceptionDetail = result.ResponseResult.ExceptionDetail });
                }
            }
        }
    }
}
