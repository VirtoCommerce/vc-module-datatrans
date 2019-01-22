using Datatrans.Checkout.Core.Event;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Managers;
using Datatrans.Checkout.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace Datatrans.Checkout
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            _container.RegisterType<IEventPublisher<DatatransBeforeCapturePaymentEvent>, EventPublisher<DatatransBeforeCapturePaymentEvent>>();

            _container.RegisterType<IDatatransCheckoutService, DatatransCheckoutService>();
            _container.RegisterType<IDatatransCapturePaymentService, DatatransCapturePaymentServiceEmptyImp>();

            IDatatransClient DatatransClientFactory(string endpoint, string username, string password) => new DatatransClient.DatatransClient(endpoint, username, password);
            _container.RegisterInstance((Func<string, string, string, IDatatransClient>) DatatransClientFactory);

            ISignProvider SignProviderFactory(string hmacKey) => new SignProvider(hmacKey);
            _container.RegisterInstance((Func<string, ISignProvider>) SignProviderFactory);

            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();

            Func<DatatransCheckoutPaymentMethod> datatransPaymentMethod = () =>
            {
                var paymentMethod = new DatatransCheckoutPaymentMethod(
                    _container.Resolve<IDatatransCheckoutService>(), 
                    _container.Resolve<Func<string, string, string, IDatatransClient>>(), 
                    _container.Resolve<IEventPublisher<DatatransBeforeCapturePaymentEvent>>(),
                    _container.Resolve<IDatatransCapturePaymentService>(),
                    _container.Resolve<Func<string, ISignProvider>>());
                paymentMethod.Name = "Datatrans Checkout Gateway";
                paymentMethod.Description = "Datatrans Checkout payment gateway integration";
                paymentMethod.LogoUrl = "https://raw.githubusercontent.com/VirtoCommerce/vc-module-datatrans/master/Datatrans.Checkout/Content/logo.png";
                paymentMethod.Settings = settingsManager.GetModuleSettings("Datatrans.Checkout");
                return paymentMethod;
            };

            var paymentMethodsService = _container.Resolve<IPaymentMethodsService>();
            paymentMethodsService.RegisterPaymentMethod(datatransPaymentMethod);

            _container.RegisterType<ISignProvider, SignProvider>();
        }
    }
}
