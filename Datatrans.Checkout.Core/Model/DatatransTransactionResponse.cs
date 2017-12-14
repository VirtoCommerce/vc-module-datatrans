namespace Datatrans.Checkout.Core.Model
{
    public class DatatransTransactionResponse : DatatransResponse
    {
        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string ReferenceNumber { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set; }

        public string AuthorizationCode { get; set; }

        public string PaymentMethod { get; set; }

        public string MaskedCC { get; set; }

        public string AliasCC { get; set; }

        public string ExpirationMonth { get; set; }

        public string ExpirationYear { get; set; }

        public string TransactionDate { get; set; }

        public string TransactionTime { get; set; }

        public string TransactionType { get; set; }

        public string SettledAmount { get; set; }

        public string ItemNumber { get; set; }
    }
}
