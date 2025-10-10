namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransAuthorizeAuthenticatedRequest
{
    public string Token { get; set; }
    public string ThreeDSResult { get; set; }
    public string PaRes { get; set; }
    public object Extra { get; set; }
}
