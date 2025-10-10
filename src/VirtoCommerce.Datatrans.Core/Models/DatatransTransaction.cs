namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransTransaction
{
    public string TransactionId { get; set; }
    public string State { get; set; }
    public long AmountMinor { get; set; }
    public string Currency { get; set; }
    public string AuthCode { get; set; }
    public DatatransError Error { get; set; }
    public object Raw { get; set; }
}
