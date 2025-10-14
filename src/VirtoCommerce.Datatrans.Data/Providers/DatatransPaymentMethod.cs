using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Datatrans.Core.Models;
using VirtoCommerce.Datatrans.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.Datatrans.Data.Providers;

public class DatatransPaymentMethod(IDatatransClient datatransClient) : PaymentMethod(nameof(DatatransPaymentMethod)), ISupportCaptureFlow, ISupportRefundFlow
{
    public override PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;
    public override PaymentMethodGroupType PaymentMethodGroupType => PaymentMethodGroupType.BankCard;

    #region overrides

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
        var tx = queryString["transactionId"];
        var result = new ValidatePostProcessRequestResult
        {
            IsSuccess = !string.IsNullOrEmpty(tx),
            OuterId = tx,
        };
        return result;
    }

    #endregion

    #region protected async methods

    protected virtual async Task<ProcessPaymentRequestResult> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;

        var initRequest = CreateInitRequest(request);
        var initResponse = await datatransClient.InitTransactionAsync(initRequest);

        var ok = initResponse.Error is null && !string.IsNullOrEmpty(initResponse.TransactionId);

        payment.OuterId = initResponse.TransactionId;
        payment.PaymentStatus = ok ? PaymentStatus.Pending : PaymentStatus.Voided;
        payment.Status = payment.PaymentStatus.ToString();

        return CreateInitRequestResult(initResponse, request);
    }

    protected virtual async Task<PostProcessPaymentRequestResult> PostProcessPaymentAsync(PostProcessPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var payment = (PaymentIn)request.Payment;
        var order = (CustomerOrder)request.Order;

        var transactionId = request.Parameters?.Get("transactionId");

        if (string.IsNullOrEmpty(transactionId))
        {
            return new PostProcessPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "transactionId not found"
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId, cancellationToken);

        if (transaction.Status is "authenticated")
        {
            transaction.Refno = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            var authReq = CreateAuthorizeRequest(request, transaction);
            var authResp = await datatransClient.AuthorizeAuthenticatedAsync(transactionId, authReq, cancellationToken);

            transaction = await datatransClient.GetTransactionAsync(transactionId, cancellationToken);
        }

        var result = GetDatatransPostResult(transaction, payment, order);

        return result;
    }

    protected virtual async Task<VoidPaymentRequestResult> VoidProcessPaymentAsync(VoidPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;
        var transactionId = GetTransactionId(request);

        if (payment.PaymentStatus == PaymentStatus.Voided)
        {
            return new VoidPaymentRequestResult
            {
                IsSuccess = true,
                NewPaymentStatus = PaymentStatus.Voided
            };
        }

        if (payment.PaymentStatus == PaymentStatus.Paid || payment.CapturedDate.HasValue)
        {
            return new VoidPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Cannot void a captured payment. Use refund instead."
            };
        }

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
            IsSuccess = response.Error != null,
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

        var amountToCapture = context.CaptureAmount ?? (total - alreadyCaptured);

        if (amountToCapture <= 0m)
        {
            return new CapturePaymentRequestResult
            {
                IsSuccess = true,
                NewPaymentStatus = payment.PaymentStatus,
                ErrorMessage = null
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId);
        var amountMinor = ToMinorUnits(amountToCapture);

        var captureRequest = new DatatransCaptureRequest
        {
            Amount = amountMinor,
            Currency = payment.Currency,
            Refno = transaction.Refno,
        };

        var response = await datatransClient.CaptureAsync(transactionId, captureRequest);

        payment.OuterId = transactionId;

        if (response.Error != null)
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
            IsSuccess = response.Error != null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = response.Error?.Message,
        };
    }

    protected virtual async Task<RefundPaymentRequestResult> RefundProcessPaymentAsync(RefundPaymentRequest context)
    {
        var payment = (PaymentIn)context.Payment;
        var order = (CustomerOrder)context.Order;
        var transactionId = GetTransactionId(context);

        if (!payment.CapturedDate.HasValue)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "No captured amount to refund."
            };
        }

        if (payment.PaymentStatus == PaymentStatus.Refunded)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Refund amount exceeds captured amount."
            };
        }

        var amountToRefund = context.AmountToRefund;

        if (amountToRefund <= 0m)
        {
            return new RefundPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "Refund amount must be greater than zero."
            };
        }

        var transaction = await datatransClient.GetTransactionAsync(transactionId);
        var amountMinor = ToMinorUnits(amountToRefund);

        var request = new DatatransRefundRequest
        {
            Amount = amountMinor,
            Currency = payment.Currency,
            Refno = transaction.Refno,
        };

        var response = await datatransClient.RefundAsync(transactionId, request);

        if (response.Error != null)
        {
            transaction = await datatransClient.GetTransactionAsync(transactionId);
            payment.PaymentStatus = ConvertStatus(transaction.Status);
        }

        payment.Status = payment.PaymentStatus.ToString();

        return new RefundPaymentRequestResult
        {
            IsSuccess = response.Error != null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = response.Error?.Message,
        };
    }

    #endregion

    #region extension points

    protected virtual DatatransInitRequest CreateInitRequest(ProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;
        var order = (CustomerOrder)request.Order;
        var store = (Store)request.Store;

        var url = (store.SecureUrl.IsNullOrEmpty() ? store.Url : store.SecureUrl)?.TrimEnd('/');

        var result = AbstractTypeFactory<DatatransInitRequest>.TryCreateInstance();
        result.Amount = ToMinorUnits(payment.Sum);
        result.Currency = payment.Currency ?? order.Currency;
        result.ReturnUrl = $"{url}/account/orders/{order.Id}/payment";
        return result;
    }

    protected virtual ProcessPaymentRequestResult CreateInitRequestResult(DatatransInitResponse initResponse, ProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;

        return new ProcessPaymentRequestResult
        {
            IsSuccess = initResponse.Error != null,
            NewPaymentStatus = payment.PaymentStatus,
            ErrorMessage = initResponse.Error?.Message,
            PublicParameters = new()
            {
                ["transactionId"] = initResponse.TransactionId,
                ["clientScript"] = datatransClient.GetSecureFieldsScriptUrl(),
                ["startUrl"] = datatransClient.BuildStartPaymentUri(initResponse.TransactionId).ToString(),
            },
        };
    }

    protected virtual DatatransAuthorizeAuthenticatedRequest CreateAuthorizeRequest(PostProcessPaymentRequest request, DatatransTransaction transaction)
    {
        var result = AbstractTypeFactory<DatatransAuthorizeAuthenticatedRequest>.TryCreateInstance();

        result.Amount = transaction.Detail.Authorize.Amount;
        result.Refno = transaction.Refno;

        return result;
    }

    #endregion

    #region Helpers

    private string GetTransactionId(PaymentRequestBase context)
    {
        var payment = (PaymentIn)context.Payment;

        var transactionId =
            payment?.OuterId
            ?? context.Parameters?.Get("transactionId")
            ?? context.OuterId;

        return transactionId;
    }

    private static long ToMinorUnits(decimal amount)
    {
        return (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
    }

    #endregion

    #region post process status mappers

    private PostProcessPaymentRequestResult GetDatatransPostResult(
        DatatransTransaction transaction,
        PaymentIn payment,
        CustomerOrder order
    )
    {
        var newStatus = ConvertStatus(transaction.Status);
        return newStatus switch
        {
            PaymentStatus.Authorized or PaymentStatus.Paid => PaymentApproved(transaction, newStatus, payment, order),
            PaymentStatus.Declined => PaymentDeclined(transaction, payment),
            PaymentStatus.Pending => PaymentPending(transaction, payment),
            _ => PaymentInvalid(transaction, payment),
        };
    }

    private PostProcessPaymentRequestResult PaymentApproved(DatatransTransaction tx, PaymentStatus newStatus, PaymentIn payment, CustomerOrder order)
    {
        var result = new PostProcessPaymentRequestResult
        {
            NewPaymentStatus = newStatus,
            OrderId = order.Id,
            OuterId = tx.TransactionId,
            IsSuccess = true,
        };

        payment.PaymentStatus = newStatus;
        payment.Status = newStatus.ToString();
        payment.IsApproved = true;
        payment.OuterId = tx.TransactionId;
        payment.AuthorizedDate ??= DateTime.UtcNow;

        payment.CapturedDate ??= DateTime.UtcNow;
        payment.Captures ??= new List<Capture>();
        payment.Captures.Add(new Capture
        {
            TransactionId = tx.TransactionId,
            Amount = payment.Sum,
            Currency = payment.Currency,
            CreatedDate = DateTime.UtcNow,
            OuterId = tx.TransactionId,
        });

        var note = $"Transaction ID: {tx.TransactionId}";
        if (!string.IsNullOrEmpty(tx.MerchantId))
        {
            note += $", Merchant ID: {tx.MerchantId}";
        }

        payment.Comment = $"Paid successfully via Datatrans. {note}{Environment.NewLine}";
        order.Status = "Processing";

        var transaction = new PaymentGatewayTransaction
        {
            IsProcessed = true,
            ProcessedDate = DateTime.UtcNow,
            CurrencyCode = payment.Currency,
            Amount = payment.Sum,
            Note = note,
            ResponseData = JsonConvert.SerializeObject(tx)
        };

        payment.Transactions ??= new List<PaymentGatewayTransaction>();
        payment.Transactions.Add(transaction);

        return result;
    }

    private PostProcessPaymentRequestResult PaymentDeclined(DatatransTransaction tx, PaymentIn payment)
    {
        var msg = tx.Error != null ? JsonConvert.SerializeObject(tx.Error) : "Invalid Datatrans response";
        var errorMessage = $"Your transaction was declined: {msg}";

        payment.Status = PaymentStatus.Declined.ToString();
        payment.ProcessPaymentResult = new ProcessPaymentRequestResult { ErrorMessage = errorMessage };
        payment.Comment = $"{errorMessage}{Environment.NewLine}";

        return new PostProcessPaymentRequestResult
        {
            NewPaymentStatus = PaymentStatus.Declined,
            ErrorMessage = errorMessage,
        };
    }

    private PostProcessPaymentRequestResult PaymentPending(DatatransTransaction tx, PaymentIn payment)
    {
        var msg = tx.Error != null ? JsonConvert.SerializeObject(tx.Error) : "Invalid Datatrans response";
        var errorMessage = $"Your transaction is pending: {msg}";

        payment.ProcessPaymentResult = new ProcessPaymentRequestResult { ErrorMessage = errorMessage };
        payment.Comment = $"{errorMessage}{Environment.NewLine}";
        payment.OuterId ??= tx.TransactionId;

        return new PostProcessPaymentRequestResult
        {
            ErrorMessage = errorMessage,
            IsSuccess = true,
            OuterId = tx.TransactionId
        };
    }

    private PostProcessPaymentRequestResult PaymentInvalid(DatatransTransaction tx, PaymentIn payment)
    {
        var msg = tx.Error != null ? JsonConvert.SerializeObject(tx.Error) : "Invalid Datatrans response";
        var errorMessage = $"There was an error processing your transaction: {msg}";

        payment.Status = PaymentStatus.Error.ToString();
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
            _ => PaymentStatus.Error
        };
    }

    #endregion
}
