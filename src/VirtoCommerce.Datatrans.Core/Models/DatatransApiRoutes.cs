namespace VirtoCommerce.Datatrans.Core.Models;

public class DatatransApiRoutes
{
    // POST - Secure Fields transaction init
    public string GetSecureFieldsPath() => "/v1/transactions/secureFields";

    // POST - Lightbox/Redirect transaction init
    public string GetInitTransactionPath() => "/v1/transactions";

    // GET
    public string GetTransactionPath(string transactionId) => $"/v1/transactions/{transactionId}";

    // POST
    public string GetAuthorizeAuthenticatedPath(string transactionId) => $"/v1/transactions/{transactionId}/authorize";

    // POST
    public string GetCapturePath(string transactionId) => $"/v1/transactions/{transactionId}/settle";

    // POST
    public string GetVoidPath(string transactionId) => $"/v1/transactions/{transactionId}/cancel";

    // POST
    public string GetRefundPath(string transactionId) => $"/v1/transactions/{transactionId}/credit";
}
