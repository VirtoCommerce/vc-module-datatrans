using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.DatatransClient.Converters;
using Datatrans.Checkout.DatatransClient.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using VirtoCommerce.Platform.Core.Common;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient
{
    public class DatatransClient : IDatatransClient
    {
        public string ServiceEndpoint { get; set; }

        protected string AuthorizationEndpoint => ServiceEndpoint + "/upp/jsp/XML_authorize.jsp";

        protected string StatusEndpoint => ServiceEndpoint + "/upp/jsp/XML_status.jsp";

        protected string ProcessEndpoint => ServiceEndpoint + "/upp/jsp/XML_processor.jsp";

        private readonly string _username;
        private readonly string _password;

        #region Implementation of IDatatransClient

        public DatatransClient(string serviceEndpoint, string username, string password)
        {
            ServiceEndpoint = serviceEndpoint;
            _username = username;
            _password = password;
        }

        public coreModel.DatatransSettlementResponse SettleTransaction(DatatransSettlementRequest request)
        {
            var requestXml = request.ToDatatransRequest();
            var response = MakeDatatransCall(ProcessEndpoint, requestXml);

            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                return new coreModel.DatatransSettlementResponse
                {
                    ErrorMessage = response.ErrorMessage
                };
            }

            var paymentServiceResponse = response.ResponseContent.DeserializeXml<paymentService>();
            var result = paymentServiceResponse.ToCoreModel();
            result.ResponseContent = response.ResponseContent;
            return result;
        }

        public coreModel.DatatransTransactionResponse GetTransactionStatus(DatatransTransactionRequest request)
        {
            var requestXml = request.ToDatatransRequest();
            var response = MakeDatatransCall(StatusEndpoint, requestXml);

            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                return new coreModel.DatatransTransactionResponse
                {
                    ErrorMessage = response.ErrorMessage
                };
            }

            var statusServiceResponse = response.ResponseContent.DeserializeXml<statusService>();
            var result = statusServiceResponse.ToCoreModel();
            result.ResponseContent = response.ResponseContent;
            return result;
        }

        public coreModel.DatatransRefundResponse Refund(DatatransRefundRequest request)
        {
            var requestXml = request.ToDatatransRequest();
            var response = MakeDatatransCall(ProcessEndpoint, requestXml);

            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                return new DatatransRefundResponse
                {
                    ErrorMessage = response.ErrorMessage
                };
            }

            var datatransRefundResponse = response.ResponseContent.DeserializeXml<RefundResponse.paymentService>();

            var result = datatransRefundResponse.ToCoreModel();

            result.ResponseData = response.ResponseContent;

            return result;
        }

        private ServiceResponse MakeDatatransCall(string endpoint, string sXml)
        {
            var result = new ServiceResponse();
            var endpointUri = new Uri(endpoint);
            try
            {
                var req = (HttpWebRequest) WebRequest.Create(endpointUri);
                req.Method = "POST";
                req.ContentType = "text/xml; charset=utf-8";

                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));

                req.Headers.Add("Authorization", $"Basic {credentials}");

                req.ContentLength = sXml.Length;
                using (var sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(sXml);
                    sw.Close();
                }

                var res = (HttpWebResponse)req.GetResponse();

                var responseStream = res.GetResponseStream();
                using (var streamReader = new StreamReader(responseStream))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(streamReader.ReadToEnd());
                    result.ResponseContent = xml.InnerXml;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion
    }
}