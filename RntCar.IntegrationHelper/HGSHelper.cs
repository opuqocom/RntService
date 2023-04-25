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
            this.prepareServiceConfiguration();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            hgsWebUtilServicesClient = new HGSService.HgsWebUtilServicesClient(myBasicHttpBinding, myEndpointAddress);
            scope = new OperationContextScope(hgsWebUtilServicesClient.InnerChannel);
            var sec = new SecurityHeader(loginInfo[0], loginInfo[1]);
            OperationContext.Current.OutgoingMessageHeaders.Add(sec);
        }

        public SaleProductResponse saleProduct(SaleProductParameter saleProductParameter)
        {
            LoggerHelper loggerHelper = new LoggerHelper();
            var convertedParameter = new HGSService.requestSaleProductWEB
            {
                licenseNo = saleProductParameter.licenseNo,
                plateNo = saleProductParameter.plateNo,
                productId = saleProductParameter.productId,
                productType = saleProductParameter.productType,
                vehicleClass = saleProductParameter.vehicleClass,
                vehicleClassSpecified = true,
            };
            loggerHelper.traceInfo("Parameter " + JsonConvert.SerializeObject(saleProductParameter));

            var response = hgsWebUtilServicesClient.saleProduct(convertedParameter);

            loggerHelper.traceInfo("Response " + JsonConvert.SerializeObject(response));

            if (!response.errorCode.Equals("000")) // 000 means there is no error
            {
                return new SaleProductResponse
                {
                    ResponseResult = ResponseResult.ReturnError(response.errorInfo + " Parameter " + JsonConvert.SerializeObject(saleProductParameter))
                };
            }

            return new SaleProductResponse
            {
                ResponseResult = ResponseResult.ReturnSuccess()
            };
        }
        public CancelProductResponse cancelProduct(CancelProductParameter cancelProductParameter)
        {
            LoggerHelper loggerHelper = new LoggerHelper();
            var convertedParameter = new HGSService.requestCancelProductWEB
            {
                productId = cancelProductParameter.productId,
                cancelReason = cancelProductParameter.cancelReason,
                cancelReasonSpecified = true
            };

            loggerHelper.traceInfo("Parameter " + JsonConvert.SerializeObject(cancelProductParameter));

            var response = hgsWebUtilServicesClient.cancelProduct(convertedParameter);

            loggerHelper.traceInfo("Response " + JsonConvert.SerializeObject(response));
            if (!response.errorCode.Equals("000")) // 000 means there is no error
            {
                return new CancelProductResponse
                {
                    ResponseResult = ResponseResult.ReturnError(response.errorInfo + " Parameter " + JsonConvert.SerializeObject(cancelProductParameter))
                };
            }

            return new CancelProductResponse
            {
                ResponseResult = ResponseResult.ReturnSuccess()
            };
        }

        private void prepareServiceConfiguration()
        {
            loginInfo = ConfigurationManager.AppSettings["hgsServiceLoginInfo"].Split(';'); 
            endpointUrl = ConfigurationManager.AppSettings["hgsEndPointUrl"]; 

            myBasicHttpBinding = new BasicHttpBinding();
            myBasicHttpBinding.Name = "HGSServiceSoap";
            myBasicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            myBasicHttpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            myBasicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.SendTimeout = TimeSpan.FromMinutes(10);
            myBasicHttpBinding.MaxReceivedMessageSize = 2147483647;

            myEndpointAddress = new EndpointAddress(endpointUrl);
        }

        ~HGSHelper()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.hgsWebUtilServicesClient = null; 
                this.scope = null;
            }
        }

        internal class SecurityHeader : MessageHeader
        {
            private const string HeaderName = "Security";
            private const string HeaderNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            public override string Name => HeaderName;
            public override string Namespace => HeaderNamespace;
            public string headerString;

            public SecurityHeader(string username, string password)
            {
                this.headerString = @"<UsernameToken xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""> 
                <Username>" + username + @"</Username> 
                <Password>" + password + @"</Password> 
                <Nonce >" + Guid.NewGuid().ToString() + @"</Nonce> 
                <Created>" + DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") + @"</Created> </UsernameToken>";
            } 

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                var r = XmlReader.Create(new StringReader(headerString));
                r.MoveToContent();
                writer.WriteNode(r, false);
            }
        }
    }
}
