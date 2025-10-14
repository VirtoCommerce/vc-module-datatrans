using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Datatrans.Data.Providers;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Datatrans.Web.Controllers;

[Route("api/payments/datatrans")]
public class DatatransController(IPaymentMethodsService paymentMethods,
    IPaymentService payments) : Controller
{
    [HttpPost("push")]
    public async Task<IActionResult> Push(CancellationToken cancellationToken = default)
    {
        // Datatrans шлёт form-urlencoded
        var form = Request.HasFormContentType
            ? Request.Form
            : throw new InvalidOperationException("Expected form data");

        var query = new NameValueCollection();
        foreach (var kvp in form)
        {
            query[kvp.Key] = kvp.Value;
        }

        return await HandleCallback(query, cancellationToken);
    }

    private async Task<IActionResult> HandleCallback(NameValueCollection query, CancellationToken cancellationToken = default)
    {
        var method = await paymentMethods.GetByIdAsync(nameof(DatatransPaymentMethod));
        if (method == null)
        {
            return NotFound("Datatrans payment method not found");
        }

        var datatrans = method as DatatransPaymentMethod;
        if (datatrans == null)
        {
            return BadRequest("Payment method type mismatch");
        }

        var validate = datatrans.ValidatePostProcessRequest(query);
        if (!validate.IsSuccess)
        {
            return BadRequest("Invalid Datatrans callback");
        }

        var transactionId = validate.OuterId;
        if (string.IsNullOrEmpty(transactionId))
        {
            return BadRequest("TransactionId not found");
        }

        var payment = await payments.GetByOuterIdAsync(transactionId);
        if (payment == null)
        {
            return NotFound($"Payment with transactionId={transactionId} not found");
        }

        var postReq = new PostProcessPaymentRequest
        {
            Payment = payment,
            OuterId = transactionId,
            Parameters = query
        };

        var result = datatrans.PostProcessPayment(postReq);

        if (result.IsSuccess)
        {
            await payments.SaveChangesAsync([payment]);
        }

        return Ok();
    }
}
