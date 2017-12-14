namespace Datatrans.Checkout.Core.Model
{
    public class DatatransSettlementRequest : DatatransRequest
    {
        public DatatransSettlementRequest()
        {
            ServiceVersion = "1";
        }

        public string ReferenceNumber { get; set; }

        public int Amount { get; set; }

        public string Currency { get; set; }
    }
}
