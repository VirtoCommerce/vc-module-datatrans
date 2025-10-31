using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Datatrans.Core;
using VirtoCommerce.Datatrans.Core.Models.External;
using VirtoCommerce.Datatrans.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.Datatrans.Data.Providers;

public class DatatransPaymentMethod(IDatatransClient datatransClient, ICurrencyService currencyService) : PaymentMethod(nameof(DatatransPaymentMethod)), ISupportCaptureFlow, ISupportRefundFlow
{
    public override PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;
    public override PaymentMethodGroupType PaymentMethodGroupType => PaymentMethodGroupType.BankCard;

    #region Overrides

    public override ProcessPaymentRequestResult ProcessPayment(ProcessPaymentRequest request)
    {
        return ProcessPaymentAsync(request).GetAwaiter().GetResult();
    }

    public override PostProcessPaymentRequestResult PostProcessPayment(PostProcessPaymentRequest request)
    {
        return PostProcessPaymentAsync(request).GetAwaiter().GetResult();
    }

    public override VoidPaymentRequestResult VoidProcessPayment(VoidPaymentRequest request)
    {
        return VoidProcessPaymentAsync(request).GetAwaiter().GetResult();
    }

    public override CapturePaymentRequestResult CaptureProcessPayment(CapturePaymentRequest context)
    {
        return CaptureProcessPaymentAsync(context).GetAwaiter().GetResult();
    }

    public override RefundPaymentRequestResult RefundProcessPayment(RefundPaymentRequest context)
    {
        return RefundProcessPaymentAsync(context).GetAwaiter().GetResult();
    }

    public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
    {
        var transactionId = queryString["transactionId"];

        var result = new ValidatePostProcessRequestResult
        {
            IsSuccess = !transactionId.IsNullOrEmpty(),
            OuterId = transactionId,
        };

        return result;
    }

    #endregion

    #region Protected async methods

    protected virtual async Task<ProcessPaymentRequestResult> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;

        var initRequest = await CreateInitRequest(request);
        var initResponse = await datatransClient.InitTransactionAsync(initRequest);

        var success = initResponse.Error is null && !initResponse.TransactionId.IsNullOrEmpty();

        payment.OuterId = initResponse.TransactionId;
        payment.PaymentStatus = success ? PaymentStatus.Pending : PaymentStatus.Voided;
        payment.Status = payment.PaymentStatus.ToString();

        return await CreateInitRequestResult(initResponse, request);
    }

    protected virtual async Task<PostProcessPaymentRequestResult> PostProcessPaymentAsync(PostProcessPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var transactionId = request.Parameters?.Get("transactionId");

        if (transactionId.IsNullOrEmpty())
        {
            return new PostProcessPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "transactionId not found",
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId, cancellationToken);

        if (transaction.Status is "authenticated")
        {
            transaction.Refno = Guid.NewGuid().ToString("N").ToLower();
            var authorizeRequest = await CreateAuthorizeRequest(transaction, request);
            await datatransClient.AuthorizeAuthenticatedAsync(transactionId, authorizeRequest, cancellationToken);

            transaction = await datatransClient.GetTransactionAsync(transactionId, cancellationToken);
        }

        var order = (CustomerOrder)request.Order;
        var payment = (PaymentIn)request.Payment;
        var result = GetDatatransPostResult(transaction, order, payment);

        return result;
    }

    protected virtual async Task<VoidPaymentRequestResult> VoidProcessPaymentAsync(VoidPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;

        if (payment.PaymentStatus == PaymentStatus.Voided)
        {
            return new VoidPaymentRequestResult
            {
                IsSuccess = true,
                NewPaymentStatus = PaymentStatus.Voided,
            };
        }

        if (payment.PaymentStatus == PaymentStatus.Paid || payment.CapturedDate.HasValue)
        {
            return new VoidPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Cannot void a captured payment. Use refund instead.",
            };
        }

        var transactionId = GetTransactionId(request);
        var response = await datatransClient.VoidAsync(transactionId);

        if (response.Error != null)
        {
            var transaction = await datatransClient.GetTransactionAsync(transactionId);
            payment.OuterId = transactionId;
            payment.PaymentStatus = ConvertStatus(transaction.Status);
        }

        payment.Status = payment.PaymentStatus.ToString();

        return new VoidPaymentRequestResult
        {
            IsSuccess = response.Error == null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = response.Error?.Message,
        };
    }

    protected virtual async Task<CapturePaymentRequestResult> CaptureProcessPaymentAsync(CapturePaymentRequest context)
    {
        var payment = (PaymentIn)context.Payment;
        var transactionId = GetTransactionId(context);

        var alreadyCaptured = payment.Captures.Sum(x => x.Amount);
        var total = payment.Sum;

        var amountToCapture = context.CaptureAmount ?? total - alreadyCaptured;

        if (amountToCapture <= 0m)
        {
            return new CapturePaymentRequestResult
            {
                IsSuccess = true,
                NewPaymentStatus = payment.PaymentStatus,
                ErrorMessage = null,
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId);
        var amountMinor = await ToMinorUnits(payment.Currency, amountToCapture);

        var captureRequest = new DatatransCaptureRequest
        {
            Amount = amountMinor,
            Currency = payment.Currency,
            Refno = transaction.Refno,
        };

        var response = await datatransClient.CaptureAsync(transactionId, captureRequest);

        payment.OuterId = transactionId;

        if (response.Error == null)
        {
            transaction = await datatransClient.GetTransactionAsync(transactionId);
            payment.PaymentStatus = ConvertStatus(transaction.Status);
            payment.CapturedDate ??= DateTime.UtcNow;
        }
        else
        {
            payment.PaymentStatus = PaymentStatus.Error;
        }

        payment.Status = payment.PaymentStatus.ToString();

        return new CapturePaymentRequestResult
        {
            IsSuccess = response.Error == null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = response.Error?.Message,
        };
    }

    protected virtual async Task<RefundPaymentRequestResult> RefundProcessPaymentAsync(RefundPaymentRequest context)
    {
        var payment = (PaymentIn)context.Payment;
        var transactionId = GetTransactionId(context);

        if (!payment.CapturedDate.HasValue)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "No captured amount to refund.",
            };
        }

        if (payment.PaymentStatus == PaymentStatus.Refunded)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Refund amount exceeds captured amount.",
            };
        }

        var amountToRefund = context.AmountToRefund;

        if (amountToRefund <= 0m)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Refund amount must be greater than zero.",
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId);
        var amountMinor = await ToMinorUnits(payment.Currency, amountToRefund);

        var request = new DatatransRefundRequest
        {
            Amount = amountMinor,
            Currency = payment.Currency,
            Refno = transaction.Refno,
        };

        var response = await datatransClient.RefundAsync(transactionId, request);

        if (response.Error == null)
        {
            transaction = await datatransClient.GetTransactionAsync(transactionId);
            payment.PaymentStatus = ConvertStatus(transaction.Status);
        }

        payment.Status = payment.PaymentStatus.ToString();

        return new RefundPaymentRequestResult
        {
            IsSuccess = response.Error == null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = response.Error?.Message,
        };
    }

    #endregion

    #region Extension points

    protected virtual async Task<DatatransInitRequest> CreateInitRequest(ProcessPaymentRequest request)
    {
        var store = (Store)request.Store;
        var order = (CustomerOrder)request.Order;
        var payment = (PaymentIn)request.Payment;

        var url = (store.SecureUrl.IsNullOrEmpty() ? store.Url : store.SecureUrl)?.TrimEnd('/');

        var result = AbstractTypeFactory<DatatransInitRequest>.TryCreateInstance();

        var currency = payment.Currency.EmptyToNull() ?? order.Currency;

        result.Amount = await ToMinorUnits(currency, payment.Sum);
        result.Currency = currency;

        var returnUrl = Settings.GetValue<string>(ModuleConstants.Settings.General.ReturnUrl)
            .Replace("{orderId}", order.Id)
            .TrimStart('/');

        result.ReturnUrl = $"{url}/{returnUrl}";

        return result;
    }

    protected virtual Task<ProcessPaymentRequestResult> CreateInitRequestResult(DatatransInitResponse initResponse, ProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;

        var result = new ProcessPaymentRequestResult
        {
            IsSuccess = initResponse.Error == null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = initResponse.Error?.Message,
        };

        if (result.IsSuccess)
        {
            result.PublicParameters = new()
            {
                ["transactionId"] = initResponse.TransactionId,
                ["clientScript"] = datatransClient.GetSecureFieldsScriptUrl(),
                ["startUrl"] = datatransClient.BuildStartPaymentUri(initResponse.TransactionId).ToString(),
            };
        }

        return Task.FromResult(result);
    }

    protected virtual Task<DatatransAuthorizeAuthenticatedRequest> CreateAuthorizeRequest(DatatransTransaction transaction, PostProcessPaymentRequest request)
    {
        var result = AbstractTypeFactory<DatatransAuthorizeAuthenticatedRequest>.TryCreateInstance();

        result.Amount = transaction.Detail.Authorize.Amount;
        result.Refno = transaction.Refno;

        return Task.FromResult(result);
    }

    #endregion

    #region Helpers

    private static string GetTransactionId(PaymentRequestBase context)
    {
        var payment = (PaymentIn)context.Payment;

        var transactionId =
            payment?.OuterId
            ?? context.Parameters?.Get("transactionId")
            ?? context.OuterId;

        return transactionId;
    }

    private async Task<long> ToMinorUnits(string currencyCode, decimal amount)
    {
        const int @base = 10;
        var currency = (await currencyService.GetAllCurrenciesAsync()).FirstOrDefault(x => x.Code.EqualsIgnoreCase(currencyCode));
        var multiplier = currency != null
            ? (decimal)Math.Pow(@base, currency.DecimalDigits) // 1, 100, or 1000
            : 100m;

        return (long)Math.Round(amount * multiplier, MidpointRounding.AwayFromZero);
    }

    #endregion

    #region Post process status mappers

    private static PostProcessPaymentRequestResult GetDatatransPostResult(DatatransTransaction transaction, CustomerOrder order, PaymentIn payment)
    {
        var newStatus = ConvertStatus(transaction.Status);

        return newStatus switch
        {
            PaymentStatus.Authorized or PaymentStatus.Paid => PaymentApproved(transaction, order, payment, newStatus),
            PaymentStatus.Declined => PaymentDeclined(transaction, payment),
            PaymentStatus.Pending => PaymentPending(transaction, payment),
            _ => PaymentInvalid(transaction, payment),
        };
    }

    private static PostProcessPaymentRequestResult PaymentApproved(DatatransTransaction transaction, CustomerOrder order, PaymentIn payment, PaymentStatus newStatus)
    {
        var result = new PostProcessPaymentRequestResult
        {
            NewPaymentStatus = newStatus,
            OrderId = order.Id,
            OuterId = transaction.TransactionId,
            IsSuccess = true,
        };

        payment.PaymentStatus = newStatus;
        payment.Status = newStatus.ToString();
        payment.IsApproved = true;
        payment.OuterId = transaction.TransactionId;
        payment.AuthorizedDate ??= DateTime.UtcNow;

        payment.CapturedDate ??= DateTime.UtcNow;
        payment.Captures ??= new List<Capture>();
        payment.Captures.Add(new Capture
        {
            TransactionId = transaction.TransactionId,
            Amount = payment.Sum,
            Currency = payment.Currency,
            CreatedDate = DateTime.UtcNow,
            OuterId = transaction.TransactionId,
        });

        var note = $"Transaction ID: {transaction.TransactionId}";
        if (!string.IsNullOrEmpty(transaction.MerchantId))
        {
            note += $", Merchant ID: {transaction.MerchantId}";
        }

        payment.Comment = $"Paid successfully via Datatrans. {note}{Environment.NewLine}";
        order.Status = "Processing";

        var paymentGatewayTransaction = new PaymentGatewayTransaction
        {
            IsProcessed = true,
            ProcessedDate = DateTime.UtcNow,
            CurrencyCode = payment.Currency,
            Amount = payment.Sum,
            Note = note,
            ResponseData = JsonConvert.SerializeObject(transaction),
        };

        payment.Transactions ??= new List<PaymentGatewayTransaction>();
        payment.Transactions.Add(paymentGatewayTransaction);

        return result;
    }

    private static PostProcessPaymentRequestResult PaymentDeclined(DatatransTransaction transaction, PaymentIn payment)
    {
        var error = transaction.Error != null ? JsonConvert.SerializeObject(transaction.Error) : "Invalid Datatrans response";
        var errorMessage = $"Your transaction was declined: {error}";

        payment.Status = nameof(PaymentStatus.Declined);
        payment.ProcessPaymentResult = new ProcessPaymentRequestResult { ErrorMessage = errorMessage };
        payment.Comment = $"{errorMessage}{Environment.NewLine}";

        return new PostProcessPaymentRequestResult
        {
            NewPaymentStatus = PaymentStatus.Declined,
            ErrorMessage = errorMessage,
        };
    }

    private static PostProcessPaymentRequestResult PaymentPending(DatatransTransaction transaction, PaymentIn payment)
    {
        var error = transaction.Error != null ? JsonConvert.SerializeObject(transaction.Error) : "Invalid Datatrans response";
        var errorMessage = $"Your transaction is pending: {error}";

        payment.ProcessPaymentResult = new ProcessPaymentRequestResult { ErrorMessage = errorMessage };
        payment.Comment = $"{errorMessage}{Environment.NewLine}";
        payment.OuterId ??= transaction.TransactionId;

        return new PostProcessPaymentRequestResult
        {
            ErrorMessage = errorMessage,
            IsSuccess = true,
            OuterId = transaction.TransactionId,
        };
    }

    private static PostProcessPaymentRequestResult PaymentInvalid(DatatransTransaction transaction, PaymentIn payment)
    {
        var error = transaction.Error != null ? JsonConvert.SerializeObject(transaction.Error) : "Invalid Datatrans response";
        var errorMessage = $"There was an error processing your transaction: {error}";

        payment.Status = nameof(PaymentStatus.Error);
        payment.ProcessPaymentResult = new ProcessPaymentRequestResult { ErrorMessage = errorMessage };
        payment.Comment = $"{errorMessage}{Environment.NewLine}";

        return new PostProcessPaymentRequestResult { ErrorMessage = errorMessage };
    }

    private static PaymentStatus ConvertStatus(string status)
    {
        return status switch
        {
            "initialized" => PaymentStatus.Pending,
            "authenticated" => PaymentStatus.Pending,
            "authorized" => PaymentStatus.Authorized,
            "settled" => PaymentStatus.Paid,
            "canceled" => PaymentStatus.Voided,
            "transmitted" => PaymentStatus.Pending,
            "failed" => PaymentStatus.Declined,
            _ => PaymentStatus.Error,
        };
    }

    #endregion
}
