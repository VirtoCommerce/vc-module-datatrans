namespace Datatrans.Checkout.Core.Model
{
    public abstract class DatatransRequest 
    {
        public string MerchangId { get; set; }

        public string TransactionId { get; set; }

        public string ServiceVersion { get; set; }
    }
}
