namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransRedirect
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string ErrorUrl { get; set; }
    public string Method { get; set; } = "GET";
}
