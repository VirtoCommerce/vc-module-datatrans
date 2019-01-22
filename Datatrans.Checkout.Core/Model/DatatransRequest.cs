namespace Datatrans.Checkout.Core.Model
{
    public abstract class DatatransRequest 
    {
        public string MerchantId { get; set; }

        public string TransactionId { get; set; }

        public string ServiceVersion { get; set; }

        public string Sign { get; set; }
    }
}
