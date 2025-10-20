namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransInitRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string ReturnUrl { get; set; }
    public string ReturnMethod { get; set; } = "GET";
}
