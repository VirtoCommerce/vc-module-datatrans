namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransRequest
{
    public string OrderId { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }

}
