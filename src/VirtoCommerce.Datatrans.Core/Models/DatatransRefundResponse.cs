namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransRefundResponse : DatatransResponseBase
{
    public string TransactionId { get; set; }
    public string AcquirerAuthorizationCode { get; set; }
}
