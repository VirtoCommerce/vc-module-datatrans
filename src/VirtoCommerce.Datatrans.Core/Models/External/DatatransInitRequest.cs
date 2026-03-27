namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransInitRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }

    // Secure Fields-specific fields
    public string ReturnUrl { get; set; }
    public string ReturnMethod { get; set; }

    // Lightbox-specific fields
    public string[] PaymentMethods { get; set; }
    public bool? AutoSettle { get; set; }
    public DatatransRedirect Redirect { get; set; }
    public string Refno { get; set; }
}
