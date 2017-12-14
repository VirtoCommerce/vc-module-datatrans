using System.IO;
using System.Reflection;
using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.Core.Services;
using DotLiquid;
using VirtoCommerce.Domain.Store.Model;

namespace Datatrans.Checkout.Services
{
    public class DatatransCheckoutService : IDatatransCheckoutService
    {
        /// <summary>
        /// NOA =  authorization only
        /// CAA = authorization with immediate settlement in case of successful authorization
        /// </summary>
        public const string DefaultPaymentActionCode = "NOA";

        /// <summary>
        /// Returns Datatrans Checkout Card Payment Form built with Datatrans Lighbox
        /// </summary>
        public string GetCheckoutFormContent(DatatransCheckoutSettings context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Datatrans.Checkout.Content.paymentForm.liquid");
            if (stream == null)
            {
                return string.Empty;
            }

            StreamReader sr = new StreamReader(stream);
            var formContent = sr.ReadToEnd();

            Template template = Template.Parse(formContent);
            var content = template.Render(Hash.FromAnonymousObject(new
            {
                storeUrl = GetStoreUrl(context.Store),
                orderId = context.Order.Number,
                amount = context.Amount,
                merchantId = context.MerchantId,
                referenceNumber = context.ReferenceNumber,
                sign = context.Sign,
                paymentAction = DefaultPaymentActionCode,
                paymentMethod = context.PaymentMethod,
                purchaseCurrency = context.PurchaseCurrency,
                language = context.Language,
                formActionUrl = context.FormActionUrl,
                frontendApi = context.FrontendApi,
                paymentMethodCode = context.InternalPaymentMethodCode,
                paymentMethodCodeParamName = context.PaymentMethodCodeParamName
            }));

            return content;
        }

        private string GetStoreUrl(Store store)
        {
            if (!string.IsNullOrEmpty(store.SecureUrl))
            {
                return store.SecureUrl;
            }

            if (!string.IsNullOrEmpty(store.Url))
            {
                return store.Url;
            }

            return "";
        }
    }
}