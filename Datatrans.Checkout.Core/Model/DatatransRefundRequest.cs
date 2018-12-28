namespace Datatrans.Checkout.Core.Model
{
    /// <summary>
    /// https://docs.datatrans.ch/docs/payment-process-refund
    /// </summary>
    public class DatatransRefundRequest
    {
        public DatatransRefundRequest()
        {
            TransType = "06"; // 06 Indicate a refund request
        }

        public string MerchantId { get; set; }

        public string RefNo { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string TransactionId { get; set; }

        public string TransType { get; set; }
    }
}
