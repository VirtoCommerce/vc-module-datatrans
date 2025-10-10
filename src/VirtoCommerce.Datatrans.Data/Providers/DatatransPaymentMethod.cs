using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using VirtoCommerce.Datatrans.Core.Models;
using VirtoCommerce.Datatrans.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;

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
        var tx = queryString["transactionId"] ?? queryString["transId"] ?? queryString["transaction-id"];
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
        var datatransRequest = GenerateRequest(request);

        var transactionResponse = await datatransClient.GetTransactionId(datatransRequest);
        var result = new ProcessPaymentRequestResult
        {
            IsSuccess = transactionResponse.Error == null,
            NewPaymentStatus = transactionResponse.Error == null ? PaymentStatus.Pending : PaymentStatus.Voided,
            ErrorMessage = transactionResponse.Error?.Message,
            PublicParameters = new()
            {
                ["transactionId"] = transactionResponse.TransactionId,
                ["clientScript"] = "https://pay.sandbox.datatrans.com/upp/payment/js/secure-fields-2.0.0.min.js", // todo: replace with SANDBOX flag
            }
        };

        var payment = (PaymentIn)request.Payment;
        payment.OuterId = transactionResponse.TransactionId;
        payment.PaymentStatus = result.NewPaymentStatus;
        payment.Status = payment.PaymentStatus.ToString();

        return result;
    }

    protected virtual Task<PostProcessPaymentRequestResult> PostProcessPaymentAsync(PostProcessPaymentRequest request)
    {
        var payment = (PaymentIn)request.Payment;
        var transactionId = request.OuterId
                            ?? request.Parameters?.GetValueOrDefault("transactionId")
                            ?? payment?.OuterId;

        if (string.IsNullOrEmpty(transactionId))
        {
            return new PostProcessPaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = "transactionId not found"
            };
        }

        // 1) запросим Datatrans о состоянии транзакции
        var tx = await datatransClient.GetTransactionAsync(transactionId); // сам метод назови как хочешь

        // 2) мэппинг статуса
        var (status, err) = MapDatatransStatus(tx); // твоя приватная функция

        // 3) обновим VC-платёж
        payment.OuterId = transactionId;
        payment.PaymentStatus = status;
        payment.Status = status.ToString();

        if (status == PaymentStatus.Paid && payment.CapturedDate == null)
        {
            payment.CapturedDate = DateTime.UtcNow;
            payment.CapturedAmount = payment.Sum; // при partial capture укажи фактическую сумму
        }

        return new PostProcessPaymentRequestResult
        {
            IsSuccess = err is null && status is PaymentStatus.Authorized or PaymentStatus.Paid,
            NewPaymentStatus = status,
            ErrorMessage = err
        };

        // 2

        //var token = request.Parameters.Get("token");
        //var payment = (PaymentIn)request.Payment;
        //var order = (CustomerOrder)request.Order;

        //var clientRequest = PrepareProcessPaymentRequest(token, payment, order);

        //var paymentResult = await datatransClient.AuthorizePayment(clientRequest);

        //var result = GetPostPaymentResult(paymentResult, payment, order);
        //return result;

        // 1

        //var result = new PostProcessPaymentRequestResult
        //{
        //    IsSuccess = false,
        //    NewPaymentStatus = PaymentStatus.Pending,
        //};

        //if (request?.Payment == null)
        //{
        //    result.IsSuccess = false;
        //    result.ErrorMessage = "Payment is null.";
        //    return Task.FromResult(result);
        //}

        //var payment = (PaymentIn)request.Payment;

        //result.IsSuccess = true;
        //result.NewPaymentStatus = payment.PaymentStatus;

        //payment.PaymentStatus = result.NewPaymentStatus;
        //payment.Status = payment.PaymentStatus.ToString();

        //return Task.FromResult(result);
    }

    private static (PaymentStatus status, string error) MapDatatransStatus(DatatransTransaction tx)
    {
        // Примеры, проверь реальные поля ответа:
        return tx.State switch
        {
            "authorized" => (PaymentStatus.Authorized, null),
            "settled" or "captured" => (PaymentStatus.Paid, null),
            "canceled" or "voided" => (PaymentStatus.Voided, null),
            "failed" => (PaymentStatus.Voided, tx.Error?.Message ?? "Payment failed"),
            _ => (PaymentStatus.Pending, null),
        };
    }



    protected virtual Task<VoidPaymentRequestResult> VoidProcessPaymentAsync(VoidPaymentRequest request)
    {
        return Task.FromResult(new VoidPaymentRequestResult());
    }

    protected virtual Task<CapturePaymentRequestResult> CaptureProcessPaymentAsync(CapturePaymentRequest context)
    {
        return Task.FromResult(new CapturePaymentRequestResult());
    }

    protected virtual Task<RefundPaymentRequestResult> RefundProcessPaymentAsync(RefundPaymentRequest context)
    {
        return Task.FromResult(new RefundPaymentRequestResult());
    }

    #endregion

    #region extension points

    protected virtual DatatransRequest GenerateRequest(ProcessPaymentRequest request)
    {
        var order = (CustomerOrder)request.Order;

        var result = AbstractTypeFactory<DatatransRequest>.TryCreateInstance();

        result.Amount = order.Total * 100; // should be in coins? look at https://api-reference.datatrans.ch/#tag/v1transactions/operation/init
        result.Currency = order.Currency;
        result.OrderId = order.Id;

        return result;
    }

    #endregion
}
