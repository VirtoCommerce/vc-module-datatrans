using System.Collections.Specialized;
using VirtoCommerce.Domain.Order.Model;

namespace Datatrans.Checkout.Core.Model
{
    public class GetAirlineDataContext
    {
        public CustomerOrder Order { get; set; }

        public PaymentIn Payment { get; set; }

        public NameValueCollection Parameters { get; set; }

        public GetAirlineDataContext()
        {
        }

        public GetAirlineDataContext(CustomerOrder order, PaymentIn payment, NameValueCollection parameters)
        {
            Order = order;
            Payment = payment;
            Parameters = parameters;
        }
    }
}
