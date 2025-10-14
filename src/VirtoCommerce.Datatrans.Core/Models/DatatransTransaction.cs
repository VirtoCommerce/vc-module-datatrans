using Newtonsoft.Json;

namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransTransaction : DatatransResponseBase
{
    public string TransactionId { get; set; }
    public string MerchantId { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public string Currency { get; set; }
    public string Refno { get; set; }
    public string PaymentMethod { get; set; }
    public string AcquirerAuthorizationCode { get; set; }
    public DatatransTransactionDetail Detail { get; set; }
    public DatatransTransactionCard Card { get; set; }
}

public class DatatransTransactionAmount
{
    public long Amount { get; set; }
}

public class DatatransTransactionDetail
{
    public DatatransTransactionAmount Authorize { get; set; }
}

public class DatatransTransactionCard
{
    public string Alias { get; set; }
    public string Fingerprint { get; set; }
    public string Masked { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public DatatransTransactionCardInfo Info { get; set; }

    [JsonProperty("3D")]
    public DatatransTransactionCard3D ThreeD
    {
        get; set;
    }
}

public class DatatransTransactionCardInfo
{
    public string Brand { get; set; }
    public string Type { get; set; }
    public string Usage { get; set; }
    public string Country { get; set; }
    public string Issuer { get; set; }
}

public class DatatransTransactionCard3D
{
    public string AuthenticationResponse { get; set; }
    public string TransStatusReason { get; set; }
}
