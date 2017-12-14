using Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.Core.Services
{
    public interface IDatatransCheckoutService
    {
        string GetCheckoutFormContent(DatatransCheckoutSettings context);
    }
}