namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransInitResponse
{
    public string TransactionId { get; set; }
    public DatatransError Error { get; set; }
}
