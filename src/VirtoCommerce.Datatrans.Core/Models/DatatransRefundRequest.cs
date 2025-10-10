namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransRefundRequest
{
    public long AmountMinor { get; set; }
    public string Currency { get; set; }
    public string Reason { get; set; }
}
