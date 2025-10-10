using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Datatrans.Core.Models;

namespace VirtoCommerce.Datatrans.Core.Services;

public interface IDatatransClient
{
    Task<DatatransResponse> GetTransactionId(DatatransRequest request, CancellationToken cancellationToken = default);



    Task<DatatransInitResponse> InitTransactionAsync(DatatransInitRequest request, CancellationToken ct = default);
    Task<DatatransInitResponse> GetTransactionId(DatatransInitRequest request, CancellationToken ct = default);
    Task<DatatransTransaction> GetTransactionAsync(string transactionId, CancellationToken ct = default);
    Task<DatatransAuthorizeResponse> AuthorizeAuthenticatedAsync(string transactionId, DatatransAuthorizeAuthenticatedRequest request, CancellationToken ct = default);
    Task<DatatransCaptureResponse> CaptureAsync(string transactionId, DatatransCaptureRequest request, CancellationToken ct = default);
    Task<DatatransVoidResponse> VoidAsync(string transactionId, CancellationToken ct = default);
    Task<DatatransRefundResponse> RefundAsync(string transactionId, DatatransRefundRequest request, CancellationToken ct = default);
    Uri BuildStartPaymentUri(string transactionId);
    string GetSecureFieldsScriptUrl();
    DatatransPushPayload ParsePushPayload(NameValueCollection formOrQuery);
    bool ValidatePushSignature(DatatransSignatureInput input);
}
