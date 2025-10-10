namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransCaptureRequest
{
    public long AmountMinor { get; set; }
    public string Currency { get; set; }
    public string Reference { get; set; }
}
