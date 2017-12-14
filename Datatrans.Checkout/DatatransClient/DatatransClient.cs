using System;
using System.IO;
using System.Net;
using System.Xml;
using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.DatatransClient.Converters;
using Datatrans.Checkout.DatatransClient.Models;
using VirtoCommerce.Platform.Core.Common;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient
{
    public class DatatransClient : IDatatransClient
    {
        public string ServiceEndpoint { get; set; }

        protected string AuthorizationEndpoint => ServiceEndpoint + "/upp/jsp/XML_authorize.jsp";

        protected string StatusEndpoint => ServiceEndpoint + "/upp/jsp/XML_status.jsp";

        protected string SettleEndpoint => ServiceEndpoint + "/upp/jsp/XML_processor.jsp";

        #region Implementation of IDatatransClient

        public DatatransClient(string serviceEndpoint)
        {
            ServiceEndpoint = serviceEndpoint;
        }

        public coreModel.DatatransSettlementResponse SettleTransaction(DatatransSettlementRequest request)
        {
            var requestXml = request.ToDatatransRequest();
            var response = MakeDatatransCall(SettleEndpoint, requestXml);

            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                return new coreModel.DatatransSettlementResponse
                {
                    ErrorMessage = response.ErrorMessage
                };
            }

            var paymentServiceResponse = response.ResponseContent.DeserializeXml<paymentService>();
            return paymentServiceResponse.ToCoreModel();
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
            return statusServiceResponse.ToCoreModel();
        }

        private ServiceResponse MakeDatatransCall(string endpoint, string sXml)
        {
            var result = new ServiceResponse();
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(endpoint);
                req.Method = "POST";
                req.ContentType = "text/xml; charset=utf-8";

                req.ContentLength = sXml.Length;
                using (var sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(sXml);
                    sw.Close();
                }

                var res = (HttpWebResponse)req.GetResponse();

                string innerXml;
                Stream responseStream = res.GetResponseStream();
                using (var streamReader = new StreamReader(responseStream))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(streamReader.ReadToEnd());
                    innerXml = xml.InnerXml;
                }

                result.ResponseContent = innerXml;
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