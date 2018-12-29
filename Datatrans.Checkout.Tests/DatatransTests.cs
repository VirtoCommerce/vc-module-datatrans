using Datatrans.Checkout.Core.Event;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Managers;
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

            var datatransClient = CreateDatatransClient("https://api.sandbox.datatrans.com");

            var datatransClientFactory = CreateDatatransClientFactory(datatransClient);

            var datatransCheckoutService = CreateDatatransCheckoutService();
            var eventPublisher = CreateEventPublisher();
            var datatransCapturePaymentService = CreateDatatransCapturePaymentService();

            var datatransCheckoutPaymentMethod = CreateDatatransCheckoutPaymentMethod(
                datatransCheckoutService.Object, 
                datatransClientFactory, 
                eventPublisher.Object, 
                datatransCapturePaymentService.Object);

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

        private IDatatransClient CreateDatatransClient(string serviceEndpoint)
        {
            return new DatatransClient.DatatransClient(serviceEndpoint);
        }

        private Mock<IEventPublisher<DatatransBeforeCapturePaymentEvent>> CreateEventPublisher()
        {
            return new Mock<IEventPublisher<DatatransBeforeCapturePaymentEvent>>();
        }

        private Mock<IDatatransCapturePaymentService> CreateDatatransCapturePaymentService()
        {
            return new Mock<IDatatransCapturePaymentService>();
        }

        private Func<string, IDatatransClient> CreateDatatransClientFactory(IDatatransClient datatransClient)
        {
            return s => datatransClient;
        }

        private DatatransCheckoutPaymentMethod CreateDatatransCheckoutPaymentMethod(
            IDatatransCheckoutService datatransCheckoutService, 
            Func<string, IDatatransClient> datatransClientFactory,
            IEventPublisher<DatatransBeforeCapturePaymentEvent> eventPublisher,
            IDatatransCapturePaymentService datatransCapturePaymentService)
        {
            return new DatatransCheckoutPaymentMethod(
                datatransCheckoutService, 
                datatransClientFactory, 
                eventPublisher,
                datatransCapturePaymentService);
        }
    }
}
