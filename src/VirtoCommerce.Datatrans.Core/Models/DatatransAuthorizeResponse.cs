namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransAuthorizeResponse
{
    public string TransactionId { get; set; }
    public string State { get; set; }
    public DatatransError Error { get; set; }
    public object Raw { get; set; }
}
