using System.Linq;
using Datatrans.Checkout.DatatransClient.Models;
using VirtoCommerce.Platform.Core.Common;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient.Converters
{
    public static class SettlementServiceResponseConverter
    {
        public static coreModel.DatatransSettlementResponse ToCoreModel(this paymentService dataModel)
        {
            var coreModel = new coreModel.DatatransSettlementResponse();

            var statusServiceBody = dataModel.body.FirstOrDefault();
            if (statusServiceBody == null) return coreModel;

            var transaction = statusServiceBody.transaction.FirstOrDefault();
            if (transaction == null) return coreModel;

            if (!transaction.response.IsNullOrEmpty())
            {
                var response = transaction.response.FirstOrDefault();
                coreModel.ResponseCode = response.responseCode;
                coreModel.ResponseMessage = response.responseMessage;
            }

            if (!transaction.error.IsNullOrEmpty())
            {
                var error = transaction.error.FirstOrDefault();
                coreModel.ErrorCode = error.errorCode;
                coreModel.ErrorDetail = error.errorDetail;
                coreModel.ErrorMessage = error.errorMessage;
            }

            return coreModel;
        }
    }
}