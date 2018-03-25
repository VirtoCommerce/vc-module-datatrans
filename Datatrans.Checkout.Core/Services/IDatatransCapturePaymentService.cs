using Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.Core.Services
{
    public interface IDatatransCapturePaymentService
    {
        DatatransAirlineData GetAirlineData(GetAirlineDataContext context);
    }
}