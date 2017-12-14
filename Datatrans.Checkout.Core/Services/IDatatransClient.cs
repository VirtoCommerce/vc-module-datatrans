using Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.Core.Services
{
    public interface IDatatransClient
    {
        string ServiceEndpoint { get; set; }

        DatatransSettlementResponse SettleTransaction(DatatransSettlementRequest request);

        DatatransTransactionResponse GetTransactionStatus(DatatransTransactionRequest request);
    }
}
