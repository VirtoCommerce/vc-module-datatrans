using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.Datatrans.Core.Models;
using VirtoCommerce.Datatrans.Core.Models.External;
using VirtoCommerce.Datatrans.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Datatrans.Data.Services;

public class DatatransClient(IHttpClientFactory httpClientFactory, IOptions<DatatransOptions> options) : IDatatransClient
{
    private readonly DatatransOptions _options = options.Value;

    private string BaseStartUrl => _options.UseSandbox ? _options.SandboxStartUrlBase : _options.ProductionStartUrlBase;
    private string SecureScriptUrl => _options.UseSandbox ? _options.SandboxSecureFieldsScriptUrl : _options.ProductionSecureFieldsScriptUrl;

    public Uri BuildStartPaymentUri(string transactionId)
    {
        if (transactionId.IsNullOrEmpty())
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(transactionId));
        }

        return new Uri($"{BaseStartUrl}{Uri.EscapeDataString(transactionId)}");
    }

    public string GetSecureFieldsScriptUrl() => SecureScriptUrl;

    public async Task<DatatransInitResponse> InitTransactionAsync(DatatransInitRequest request, CancellationToken cancellationToken = default)
    {
        var resp = await SendAsync(HttpMethod.Post, _options.Routes.GetSecureFieldsPath(), request, cancellationToken);
        return ParseResponse<DatatransInitResponse>(resp);
    }

    public async Task<DatatransTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        var path = _options.Routes.GetTransactionPath(transactionId);
        var resp = await SendAsync(HttpMethod.Get, path, null, cancellationToken);
        return ParseResponse<DatatransTransaction>(resp);
    }

    public async Task<DatatransAuthorizeResponse> AuthorizeAuthenticatedAsync(string transactionId, DatatransAuthorizeAuthenticatedRequest request, CancellationToken cancellationToken = default)
    {
        var path = _options.Routes.GetAuthorizeAuthenticatedPath(transactionId);
        var resp = await SendAsync(HttpMethod.Post, path, request, cancellationToken);
        return ParseResponse<DatatransAuthorizeResponse>(resp);
    }

    public async Task<DatatransCaptureResponse> CaptureAsync(string transactionId, DatatransCaptureRequest request, CancellationToken cancellationToken = default)
    {
        var path = _options.Routes.GetCapturePath(transactionId);
        var resp = await SendAsync(HttpMethod.Post, path, request, cancellationToken);
        return ParseResponse<DatatransCaptureResponse>(resp);
    }

    public async Task<DatatransVoidResponse> VoidAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        var path = _options.Routes.GetVoidPath(transactionId);
        var resp = await SendAsync(HttpMethod.Post, path, new { }, cancellationToken);
        return ParseResponse<DatatransVoidResponse>(resp);
    }

    public async Task<DatatransRefundResponse> RefundAsync(string transactionId, DatatransRefundRequest request, CancellationToken cancellationToken = default)
    {
        var path = _options.Routes.GetRefundPath(transactionId);
        var resp = await SendAsync(HttpMethod.Post, path, request, cancellationToken);
        return ParseResponse<DatatransRefundResponse>(resp);
    }

    private async Task<string> SendAsync(HttpMethod method, string path, object body, CancellationToken ct)
    {
        using var msg = new HttpRequestMessage(method, path);

        if (body is not null)
        {
            var json = JsonConvert.SerializeObject(body);
            msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var httpClient = httpClientFactory.CreateClient("Datatrans");

        using var resp = await httpClient.SendAsync(msg, ct);
        var content = await resp.Content.ReadAsStringAsync(ct);

        return content;
    }

    private static T ParseResponse<T>(string json) where T : DatatransResponseBase, new()
    {
        try
        {
            if (json.IsNullOrEmpty())
            {
                return new T();
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception exception)
        {
            return new T
            {
                Error = new DatatransError
                {
                    Message = exception.Message,
                    Raw = json,
                },
            };
        }
    }
}
