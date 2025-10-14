using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Datatrans.Core.Models;

namespace VirtoCommerce.Datatrans.Core.Services;

public interface IDatatransClient
{
    Task<DatatransInitResponse> InitTransactionAsync(DatatransInitRequest request, CancellationToken ct = default);

    Task<DatatransTransaction> GetTransactionAsync(string transactionId, CancellationToken ct = default);

    Task<DatatransAuthorizeResponse> AuthorizeAuthenticatedAsync(string transactionId, DatatransAuthorizeAuthenticatedRequest request,
        CancellationToken ct = default);

    Task<DatatransCaptureResponse> CaptureAsync(string transactionId, DatatransCaptureRequest request, CancellationToken ct = default);

    Task<DatatransVoidResponse> VoidAsync(string transactionId, CancellationToken ct = default);

    Task<DatatransRefundResponse> RefundAsync(string transactionId, DatatransRefundRequest request, CancellationToken ct = default);

    Uri BuildStartPaymentUri(string transactionId);

    string GetSecureFieldsScriptUrl();
}
