using Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.Core.Event
{
    public class DatatransBeforeCapturePaymentEvent
    {
        public DatatransAirlineData AirlineData { get; set; }
    }
}
