using System.Collections.Specialized;

namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransPushPayload
{
    public string TransactionId { get; set; }
    public string OrderId { get; set; }
    public string State { get; set; }
    public long AmountMinor { get; set; }
    public string Currency { get; set; }
    public string Signature { get; set; }   // если Datatrans присылает подпись/хеш
    public NameValueCollection Raw { get; set; }
}
