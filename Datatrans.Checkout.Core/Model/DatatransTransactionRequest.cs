namespace Datatrans.Checkout.Core.Model
{
    public class DatatransTransactionRequest : DatatransRequest
    {
        public DatatransTransactionRequest()
        {
            ReqestType = "STX";
            ServiceVersion = "3";
        }

        public string ReqestType { get; set; }
    }
}
