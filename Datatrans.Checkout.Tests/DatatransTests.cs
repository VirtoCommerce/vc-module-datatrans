using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Managers;
using Datatrans.Checkout.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;

namespace Datatrans.Checkout.Tests
{
    public class DatatransTests
    {
        [Fact]
        public void TestRefund()
        {
            var context = new RefundProcessPaymentEvaluationContext
            {
                Order = new CustomerOrder
                {
                    Number = "",
                    Currency = "USD"
                },
                Payment =  new PaymentIn
                {
                    OuterId = "",
                    Sum = 0M,
                    Transactions = new List<PaymentGatewayTransaction>()
                },
                Parameters = new NameValueCollection
                {
                    {"RefundAmount", (-5.33M).ToString(CultureInfo.InvariantCulture)}
                }
            };

            var endpoint = "https://api.sandbox.datatrans.com";
            var username = "";
            var password = "";
            var hmacKey = "";

            var datatransClient = CreateDatatransClient(endpoint, username, password);

            var datatransClientFactory = CreateDatatransClientFactory(datatransClient);

            var datatransCheckoutService = CreateDatatransCheckoutService();
            var datatransCapturePaymentService = CreateDatatransCapturePaymentService();

            var datatransCheckoutPaymentMethod = CreateDatatransCheckoutPaymentMethod(
                datatransCheckoutService.Object, 
                datatransClientFactory,
                datatransCapturePaymentService.Object,
                CreateSignProvider(hmacKey));

            datatransCheckoutPaymentMethod.Settings = new List<SettingEntry>
            {
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.MerchantId",
                    Value = ""
                }
            };

            var result = datatransCheckoutPaymentMethod.RefundProcessPayment(context);
        }

        private Mock<IDatatransCheckoutService> CreateDatatransCheckoutService()
        {
            return new Mock<IDatatransCheckoutService>();
        }

        private IDatatransClient CreateDatatransClient(string serviceEndpoint, string username, string password)
        {
            return new DatatransClient.DatatransClient(serviceEndpoint, username, password);
        }

        private Mock<IDatatransCapturePaymentService> CreateDatatransCapturePaymentService()
        {
            return new Mock<IDatatransCapturePaymentService>();
        }

        private Func<string, string, string, IDatatransClient> CreateDatatransClientFactory(IDatatransClient datatransClient)
        {
            return (s, s1, s2) => datatransClient;
        }

        private Func<string, ISignProvider> CreateSignProvider(string hmacKey)
        {
            return s => new SignProvider(hmacKey);
        }

        private DatatransCheckoutPaymentMethod CreateDatatransCheckoutPaymentMethod(
            IDatatransCheckoutService datatransCheckoutService, 
            Func<string, string, string, IDatatransClient> datatransClientFactory,
            IDatatransCapturePaymentService datatransCapturePaymentService,
            Func<string, ISignProvider> signProviderFactory)
        {
            return new DatatransCheckoutPaymentMethod(
                datatransCheckoutService, 
                datatransClientFactory,
                datatransCapturePaymentService,
                signProviderFactory);
        }
    }
}
