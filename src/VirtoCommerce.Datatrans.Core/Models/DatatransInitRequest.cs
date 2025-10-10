namespace VirtoCommerce.Datatrans.Core.Models;
public class DatatransInitRequest
{
    public long AmountMinor { get; set; }
    public string Currency { get; set; }
    public string OrderId { get; set; }
    public string ReturnUrl { get; set; }
    public string PushUrl { get; set; }
    public string Description { get; set; }
    public string PaymentMethod { get; set; }
    public object Metadata { get; set; }
}
