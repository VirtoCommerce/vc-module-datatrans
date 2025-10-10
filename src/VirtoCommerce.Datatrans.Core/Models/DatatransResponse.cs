namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransResponse
{
    public string TransactionId { get; set; }
    public DatatransError Error { get; set; }
}
