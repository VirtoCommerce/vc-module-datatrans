using Datatrans.Checkout.Core.Event;
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
using VirtoCommerce.Platform.Core.Events;
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
                    {"RefundAmount", (-15.33M).ToString(CultureInfo.InvariantCulture)}
                }
            };

            var endpoint = "http://api.sandbox.datatrans.com";
            var username = "";
            var password = "";

            var datatransClient = CreateDatatransClient(endpoint, username, password);

            var datatransClientFactory = CreateDatatransClientFactory(datatransClient);

            var datatransCheckoutService = CreateDatatransCheckoutService();
            var eventPublisher = CreateEventPublisher();
            var datatransCapturePaymentService = CreateDatatransCapturePaymentService();

            var datatransCheckoutPaymentMethod = CreateDatatransCheckoutPaymentMethod(
                datatransCheckoutService.Object, 
                datatransClientFactory, 
                eventPublisher.Object, 
                datatransCapturePaymentService.Object,
                CreateSignProvider());

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

        private Mock<IEventPublisher<DatatransBeforeCapturePaymentEvent>> CreateEventPublisher()
        {
            return new Mock<IEventPublisher<DatatransBeforeCapturePaymentEvent>>();
        }

        private Mock<IDatatransCapturePaymentService> CreateDatatransCapturePaymentService()
        {
            return new Mock<IDatatransCapturePaymentService>();
        }

        private Func<string, string, string, IDatatransClient> CreateDatatransClientFactory(IDatatransClient datatransClient)
        {
            return (s, s1, s2) => datatransClient;
        }

        private Func<string, ISignProvider> CreateSignProvider()
        {
            return s => new Mock<ISignProvider>().Object;
        }

        private DatatransCheckoutPaymentMethod CreateDatatransCheckoutPaymentMethod(
            IDatatransCheckoutService datatransCheckoutService, 
            Func<string, string, string, IDatatransClient> datatransClientFactory,
            IEventPublisher<DatatransBeforeCapturePaymentEvent> eventPublisher,
            IDatatransCapturePaymentService datatransCapturePaymentService,
            Func<string, ISignProvider> signProviderFactory)
        {
            return new DatatransCheckoutPaymentMethod(
                datatransCheckoutService, 
                datatransClientFactory, 
                eventPublisher,
                datatransCapturePaymentService,
                signProviderFactory);
        }
    }
}
