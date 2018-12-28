namespace Datatrans.Checkout.Core.Model
{
    public class DatatransRefundResponse
    {
        public string TransactionId { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDetail { get; set; }

        public string ResponseData { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }
}
