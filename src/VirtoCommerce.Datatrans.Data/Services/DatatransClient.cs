using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Datatrans.Core.Models;
using VirtoCommerce.Datatrans.Core.Services;

namespace VirtoCommerce.Datatrans.Data.Services;

public class DatatransClient(IHttpClientFactory httpClientFactory, IOptions<DatatransOptions> options) : IDatatransClient
{
    private readonly DatatransOptions _options = options.Value;

    private string BaseStartUrl => _options.UseSandbox ? _options.SandboxStartUrlBase : _options.ProductionStartUrlBase;
    private string SecureScriptUrl => _options.UseSandbox ? _options.SandboxSecureFieldsScriptUrl : _options.ProductionSecureFieldsScriptUrl;

    public string GetSecureFieldsScriptUrl() => SecureScriptUrl;

    public Uri BuildStartPaymentUri(string transactionId)
    {
        return new Uri($"{BaseStartUrl}{Uri.EscapeDataString(transactionId)}");
    }

    public async Task<DatatransResponse> GetTransactionId(DatatransRequest request, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient();

        var token = GetToken();

        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri("https://api.sandbox.datatrans.com/v1/transactions/secureFields"),
            Method = HttpMethod.Post,
            Headers =
                {
                    { "Authorization", $"Basic {token}" },
                    { "User-Agent", "VirtoCommerce/3.0" },
                    { "Idempotency-Key", request.OrderId }, // Using OrderId as Idempotency-Key for simplicity
                },
            Content = JsonContent.Create(new // should it be a model?
            {
                request.Currency,
                request.Amount,
                returnUrl = "https://localhost:3000", // storeUrl??
            }),
        };
        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<DatatransResponse>(cancellationToken);
        return result;
    }


    private string GetToken()
    {
        var merchantId = "1110020617"; // todo: get from settings
        var password = "eVZ5bq237EUzbpOF";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{merchantId}:{password}"));
    }
}
