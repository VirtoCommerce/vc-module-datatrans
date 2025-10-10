using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Datatrans.Data.Providers;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.Datatrans.Web.Controllers;

[Route("api/datatrans")]
public class TestController(DatatransPaymentMethod paymentMethod) : Controller
{
    [HttpGet]
    [Route("test")]
    public IActionResult Test()
    {
        var processPaymentRequest = new ProcessPaymentRequest
        {
            OrderId = "order-id",
            Order = new CustomerOrder(),
            PaymentId = "payment-id",
            Payment = new PaymentIn(),
            StoreId = "store-id",
            Store = new Store()
        };

        var processPaymentResult = paymentMethod.ProcessPayment(processPaymentRequest);

        return Ok(processPaymentResult);
    }
}
