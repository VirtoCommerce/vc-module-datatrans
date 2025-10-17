namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransOptions
{
    public string SandboxBaseUrl { get; set; } = "https://api.sandbox.datatrans.com";
    public string ProductionBaseUrl { get; set; } = "https://api.datatrans.com";

    public string SandboxSecureFieldsScriptUrl { get; set; } = "https://pay.sandbox.datatrans.com/upp/payment/js/secure-fields-2.0.0.min.js";
    public string ProductionSecureFieldsScriptUrl { get; set; } = "https://pay.datatrans.com/upp/payment/js/secure-fields-2.0.0.min.js";

    public string SandboxStartUrlBase { get; set; } = "https://pay.sandbox.datatrans.com/v1/start/";
    public string ProductionStartUrlBase { get; set; } = "https://pay.datatrans.com/v1/start/";

    public bool UseSandbox { get; set; } = true;

    public string MerchantId { get; set; }
    public string Secret { get; set; }

    public DatatransApiRoutes Routes { get; set; } = new();
}

public class DatatransApiRoutes
{
    // POST
    public string SecureFieldsPath { get; set; } = "/v1/transactions/secureFields";

    // GET
    public string TransactionPath { get; set; } = "/v1/transactions/{transactionId}";

    // POST
    public string AuthorizeAuthenticatedPath { get; set; } = "/v1/transactions/{transactionId}/authorize";

    // POST
    public string CapturePath { get; set; } = "/v1/transactions/{transactionId}/capture";

    // POST
    public string VoidPath { get; set; } = "/v1/transactions/{transactionId}/cancel";

    // POST
    public string RefundPath { get; set; } = "/v1/transactions/{transactionId}/credit";
}
