namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransInitRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }

    /// <summary>
    /// 2-letter language code for UI localization (e.g. "en", "de", "fr").
    /// See https://docs.datatrans.ch/docs/redirect-lightbox#language-support.
    /// </summary>
    public string Language { get; set; }

    // Secure Fields-specific fields
    public string ReturnUrl { get; set; }
    public string ReturnMethod { get; set; }

    // Lightbox-specific fields
    public string[] PaymentMethods { get; set; }
    public bool? AutoSettle { get; set; }
    public DatatransRedirect Redirect { get; set; }
    public string Refno { get; set; }
}
