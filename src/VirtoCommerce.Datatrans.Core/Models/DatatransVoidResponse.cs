namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransVoidResponse
{
    public string TransactionId { get; set; }
    public string State { get; set; }       // voided/canceled
    public DatatransError Error { get; set; }
    public object Raw { get; set; }
}
