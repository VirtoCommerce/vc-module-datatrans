namespace Datatrans.Checkout.Core.Model
{
    /// <summary>
    /// https://docs.datatrans.ch/docs/payment-process-refund
    /// </summary>
    public class DatatransRefundRequest : DatatransRequest
    {
        public DatatransRefundRequest()
        {
            TransType = "06"; // 06 Indicate a refund request
            ServiceVersion = "1";
        }

        public string ReferenceNumber { get; set; }

        public int Amount { get; set; }

        public string Currency { get; set; }

        public string TransType { get; set; }
    }
}
