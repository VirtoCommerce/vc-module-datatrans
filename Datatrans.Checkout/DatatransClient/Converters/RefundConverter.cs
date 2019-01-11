using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.DatatransClient.Models;
using System.Linq;
using System.Xml.Linq;

namespace Datatrans.Checkout.DatatransClient.Converters
{
    public static class RefundConverter
    {
        public static string ToDatatransRequest(this DatatransRefundRequest request)
        {
            var requestXml =
                new XElement("paymentService", new XAttribute("version", request.ServiceVersion),
                    new XElement("body", new XAttribute("merchantId", request.MerchantId),
                        new XElement("transaction", new XAttribute("refno", request.ReferenceNumber),
                            new XElement("request",
                                new XElement("amount", request.Amount),
                                new XElement("currency", request.Currency),
                                new XElement("uppTransactionId", request.TransactionId),
                                new XElement("transtype", request.TransType),
                                new XElement("sign", request.Sign)
                            )
                        )
                    )
                );

            return requestXml.ToString();
        }

        public static DatatransRefundResponse ToCoreModel(this RefundResponse.paymentService response)
        {
            var coreModel = new DatatransRefundResponse();

            var refundResponseBody = response.body.FirstOrDefault();
            if (refundResponseBody == null)
            {
                return coreModel;
            }

            var transaction = refundResponseBody.transaction?.FirstOrDefault();
            var actualResponse = transaction?.response?.FirstOrDefault();
            var transactionError = transaction?.error?.FirstOrDefault();
            var generalError = refundResponseBody.error?.FirstOrDefault();

            if (transactionError != null)
            {
                coreModel.ErrorMessage = transactionError.errorMessage;
                coreModel.ErrorCode = transactionError.errorCode;
                coreModel.ErrorDetail = transactionError.errorDetail;
            }

            if (generalError != null)
            {
                coreModel.ErrorMessage = generalError.errorMessage;
                coreModel.ErrorCode = generalError.errorCode;
                coreModel.ErrorDetail = generalError.errorDetail;
            }

            if (actualResponse != null)
            {
                coreModel.ResponseCode = actualResponse.responseCode;
                coreModel.ResponseMessage = actualResponse.responseMessage;
                coreModel.TransactionId = actualResponse.uppTransactionId;
            }

            return coreModel;
        }
    }
}