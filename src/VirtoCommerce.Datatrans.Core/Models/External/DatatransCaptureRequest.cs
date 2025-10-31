namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransCaptureRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string Refno { get; set; }
}
