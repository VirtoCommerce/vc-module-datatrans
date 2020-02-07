using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Managers;
using Datatrans.Checkout.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;

namespace Datatrans.Checkout.Tests
{
    public class DatatransCheckoutPaymentMethodTests
    {
        [Fact]
        public void ValidatePostProcessRequestWithHmacAndSign2Test()
        {
            var signProviderFactory = CreateMockSignProviderFactory(CreateSignProvider());

            var paymentMethod = CreateDatatransCheckoutPaymentMethod(
                CreateMockDatatransCheckoutService().Object,
                CreateMockDatatransFactory().Object,
                CreateMockDatatransCapturePaymentService().Object,
                signProviderFactory.Object);

            paymentMethod.Settings = new List<SettingEntry>
            {
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.HMACHex",
                    Value = "testHMACHex"
                },
            };

            var queryParams = new NameValueCollection
            {
                {"paymentMethodCode", "DatatransCheckout"},
                {"sign2", "testSign2"},
                {"uppTransactionId", "testUppTransactionId"},
                {"amount", "123"},
                {"currency", "RU-ru"},
                {"merchantId", "testMerchantId"},
                {"status", "success"},
            };

            var result = paymentMethod.ValidatePostProcessRequest(queryParams);

            signProviderFactory.Verify(x => x("testHMACHex"), Times.Once);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void ValidatePostProcessRequestWithHmacWithoutSign2Test()
        {
            var signProviderFactory = CreateMockSignProviderFactory(CreateSignProvider());

            var paymentMethod = CreateDatatransCheckoutPaymentMethod(
                CreateMockDatatransCheckoutService().Object,
                CreateMockDatatransFactory().Object,
                CreateMockDatatransCapturePaymentService().Object,
                signProviderFactory.Object);

            paymentMethod.Settings = new List<SettingEntry>
            {
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.HMACHex",
                    Value = "test"
                },
            };

            var queryParams = new NameValueCollection
            {
                {"paymentMethodCode", "DatatransCheckout"},
                {"uppTransactionId", "testUppTransactionId"},
                {"amount", "123"},
                {"currency", "RU-ru"},
                {"merchantId", "testMerchantId"},
            };

            var result = paymentMethod.ValidatePostProcessRequest(queryParams);

            signProviderFactory.Verify(x => x("test"), Times.Never);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ValidatePorstProcessRequestWitHmac2WithSign2Test()
        {
            var signProviderFactory = CreateMockSignProviderFactory(CreateSignProvider());

            var paymentMethod = CreateDatatransCheckoutPaymentMethod(
                CreateMockDatatransCheckoutService().Object,
                CreateMockDatatransFactory().Object,
                CreateMockDatatransCapturePaymentService().Object,
                signProviderFactory.Object);

            paymentMethod.Settings = new List<SettingEntry>
            {
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.HMACHEXSign2",
                    Value = "testHMACHEXSign2"
                },
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.HMACHex",
                    Value = "testHMACHEX"
                },
            };

            var queryParams = new NameValueCollection
            {
                {"paymentMethodCode", "DatatransCheckout"},
                {"sign2", "testSign2"},
                {"uppTransactionId", "testUppTransactionId"},
                {"amount", "123"},
                {"currency", "RU-ru"},
                {"merchantId", "testMerchantId"},
                {"status", "success"},
            };

            var result = paymentMethod.ValidatePostProcessRequest(queryParams);

            signProviderFactory.Verify(x => x("testHMACHEXSign2"), Times.Once);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void No_Validation_Test()
        {
            var signProviderFactory = CreateMockSignProviderFactory(CreateSignProvider());

            var paymentMethod = CreateDatatransCheckoutPaymentMethod(
                CreateMockDatatransCheckoutService().Object,
                CreateMockDatatransFactory().Object,
                CreateMockDatatransCapturePaymentService().Object,
                signProviderFactory.Object);

            paymentMethod.Settings = new List<SettingEntry>
            {
                new SettingEntry
                {
                    Name = "Datatrans.Checkout.HMACHex",
                    Value = null
                },
            };

            var queryParams = new NameValueCollection
            {
                {"paymentMethodCode", "DatatransCheckout"},
                {"uppTransactionId", "testUppTransactionId"},
                {"amount", "123"},
                {"currency", "RU-ru"},
                {"merchantId", "testMerchantId"},
                {"status", "success"},
            };

            var result = paymentMethod.ValidatePostProcessRequest(queryParams);

            signProviderFactory.Verify(x => x(It.IsAny<string>()), Times.Never);
            Assert.True(result.IsSuccess);
        }


        private ISignProvider CreateSignProvider()
        {
            var signProvider = new Mock<ISignProvider>();

            signProvider
                .Setup(x =>
                    x.ValidateSignature(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    )
                )
                .Returns(true);

            return signProvider.Object;
        }

        private Mock<Func<string, ISignProvider>> CreateMockSignProviderFactory(ISignProvider signProvider)
        {
            var providerFactory = new Mock<Func<string, ISignProvider>>();

            providerFactory.Setup(x => x(It.IsAny<string>())).Returns(signProvider);

            return providerFactory;
        }

        private Mock<IDatatransCapturePaymentService> CreateMockDatatransCapturePaymentService()
        {
            return new Mock<IDatatransCapturePaymentService>();
        }

        private Mock<Func<string, string, string, IDatatransClient>> CreateMockDatatransFactory()
        {
            return new Mock<Func<string, string, string, IDatatransClient>>();
        }

        private Mock<IDatatransCheckoutService> CreateMockDatatransCheckoutService()
        {
            return new Mock<IDatatransCheckoutService>();
        }

        private DatatransCheckoutPaymentMethod CreateDatatransCheckoutPaymentMethod(
            IDatatransCheckoutService datatransCheckoutService, 
            Func<string, string, string, IDatatransClient> datatransClientFactory, 
            IDatatransCapturePaymentService datatransCapturePaymentService,
            Func<string, ISignProvider> signProviderFactory)
        {
            return new DatatransCheckoutPaymentMethod(datatransCheckoutService, datatransClientFactory, datatransCapturePaymentService, signProviderFactory);
        }
    }
}
