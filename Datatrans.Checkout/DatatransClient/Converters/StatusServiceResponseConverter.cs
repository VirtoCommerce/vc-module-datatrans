using System.Linq;
using Datatrans.Checkout.DatatransClient.Models;
using VirtoCommerce.Platform.Core.Common;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient.Converters
{
    public static class StatusServiceResponseConverter
    {
        public static coreModel.DatatransTransactionResponse ToCoreModel(this statusService dataModel)
        {
            var coreModel = new coreModel.DatatransTransactionResponse();

            var statusServiceBody = dataModel.body.FirstOrDefault();
            if (statusServiceBody == null) return coreModel;

            if (!statusServiceBody.transaction.IsNullOrEmpty())
            {
                var transaction = statusServiceBody.transaction.FirstOrDefault();
                if (transaction?.response != null)
                {
                    var actualResponse = transaction.response.FirstOrDefault();
                    coreModel.ResponseCode = actualResponse.responseCode;
                    coreModel.ResponseMessage = actualResponse.responseMessage;
                    coreModel.ReferenceNumber = actualResponse.refno;
                    coreModel.Amount = actualResponse.amount;
                    coreModel.Currency = actualResponse.currency;
                    coreModel.AuthorizationCode = actualResponse.authorizationCode;
                    coreModel.PaymentMethod = actualResponse.pmethod;
                    coreModel.TransactionId = actualResponse.uppTransactionId;
                    coreModel.MaskedCC = actualResponse.maskedCC;
                    coreModel.AliasCC = actualResponse.aliasCC;
                    coreModel.ExpirationMonth = actualResponse.expm;
                    coreModel.ExpirationYear = actualResponse.expy;
                    coreModel.TransactionDate = actualResponse.trxDate;
                    coreModel.TransactionTime = actualResponse.trxTime;
                    coreModel.TransactionType = actualResponse.trtype;
                    coreModel.SettledAmount = actualResponse.settledAmount;
                    coreModel.ItemNumber = actualResponse.itemNr;
                }
            }

            if (!statusServiceBody.error.IsNullOrEmpty())
            {
                var error = statusServiceBody.error.FirstOrDefault();
                coreModel.ErrorCode = error.errorCode;
                coreModel.ErrorDetail = error.errorDetail;
                coreModel.ErrorMessage = error.errorMessage;
            }

            return coreModel;
        }
    }
}
