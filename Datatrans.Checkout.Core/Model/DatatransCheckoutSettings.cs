using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Store.Model;

namespace Datatrans.Checkout.Core.Model
{
    public class DatatransCheckoutSettings
    {
        public Store Store { get; set; }

        public CustomerOrder Order { get; set; }

        public int Amount { get; set; }

        public string MerchantId { get; set; }

        public string ReferenceNumber { get; set; }

        public string Sign { get; set; }

        /// <summary>
        /// Settlement or Sale
        /// </summary>
        public string PaymentAction { get; set; }

        /// <summary>
        /// VIS,ECA etc
        /// </summary>
        public string PaymentMethod { get; set; }

        public string PurchaseCurrency { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// cart/externalpaymentcallback
        /// </summary>
        public string FormActionUrl { get; set; }

        /// <summary>
        /// Live or test
        /// </summary>
        public string FrontendApi { get; set; }

        /// <summary>
        /// Code of this payment method
        /// </summary>
        public string InternalPaymentMethodCode { get; set; }

        public string PaymentMethodCodeParamName { get; set; }
    }
}