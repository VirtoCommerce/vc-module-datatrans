using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Datatrans.Core.Models;

namespace VirtoCommerce.Datatrans.Core.Services;

public interface IDatatransClient
{
    Task<DatatransInitResponse> InitTransactionAsync(DatatransInitRequest request, CancellationToken cancellationToken = default);

    Task<DatatransTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default);

    Task<DatatransAuthorizeResponse> AuthorizeAuthenticatedAsync(string transactionId, DatatransAuthorizeAuthenticatedRequest request, CancellationToken cancellationToken = default);

    Task<DatatransCaptureResponse> CaptureAsync(string transactionId, DatatransCaptureRequest request, CancellationToken cancellationToken = default);

    Task<DatatransVoidResponse> VoidAsync(string transactionId, CancellationToken cancellationToken = default);

    Task<DatatransRefundResponse> RefundAsync(string transactionId, DatatransRefundRequest request, CancellationToken cancellationToken = default);

    Uri BuildStartPaymentUri(string transactionId);

    string GetSecureFieldsScriptUrl();
}
