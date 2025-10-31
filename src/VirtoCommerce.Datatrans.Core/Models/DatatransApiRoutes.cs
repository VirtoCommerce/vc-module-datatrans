namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransApiRoutes
{
    // POST
    public string GetSecureFieldsPath() => "/v1/transactions/secureFields";

    // GET
    public string GetTransactionPath(string transactionId) => $"/v1/transactions/{transactionId}";

    // POST
    public string GetAuthorizeAuthenticatedPath(string transactionId) => $"/v1/transactions/{transactionId}/authorize";

    // POST
    public string GetCapturePath(string transactionId) => $"/v1/transactions/{transactionId}/capture";

    // POST
    public string GetVoidPath(string transactionId) => $"/v1/transactions/{transactionId}/cancel";

    // POST
    public string GetRefundPath(string transactionId) => $"/v1/transactions/{transactionId}/credit";
}
