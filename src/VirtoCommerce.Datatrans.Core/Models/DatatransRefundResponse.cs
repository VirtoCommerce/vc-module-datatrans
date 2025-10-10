namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransRefundResponse
{
    public string TransactionId { get; set; }
    public string State { get; set; }       // refunded/partially_refunded/…
    public DatatransError Error { get; set; }
    public object Raw { get; set; }
}
