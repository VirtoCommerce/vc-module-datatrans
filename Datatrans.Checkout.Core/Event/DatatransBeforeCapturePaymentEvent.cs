using System.Collections.Specialized;
using Datatrans.Checkout.Core.Model;
using VirtoCommerce.Domain.Order.Model;

namespace Datatrans.Checkout.Core.Event
{
    public class DatatransBeforeCapturePaymentEvent
    {
        public DatatransAirlineData AirlineData { get; set; }

        public CustomerOrder Order { get; set; }

        public PaymentIn Payment { get; set; }

        public NameValueCollection Parameters { get; set; }

        public DatatransBeforeCapturePaymentEvent()
        {
        }

        public DatatransBeforeCapturePaymentEvent(CustomerOrder order, PaymentIn payment, NameValueCollection parameters, DatatransAirlineData airlineData)
        {
            Order = order;
            Payment = payment;
            Parameters = parameters;
            AirlineData = airlineData;
        }
    }
}
