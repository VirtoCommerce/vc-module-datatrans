namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransRefundResponse : DatatransResponseBase
{
    public string TransactionId { get; set; }
    public string AcquirerAuthorizationCode { get; set; }
}
