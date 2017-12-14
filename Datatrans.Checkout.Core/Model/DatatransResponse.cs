namespace Datatrans.Checkout.Core.Model
{
    public class DatatransResponse
    {
        public string TransactionId { get; set; }

        public string InnerXml { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorDetail { get; set; }
    }
}
