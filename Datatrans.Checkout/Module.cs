using System;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Managers;
using Datatrans.Checkout.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using VirtoCommerce.Domain.Payment.Services;
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
            _container.RegisterType<IDatatransCheckoutService, DatatransCheckoutService>();

            Func<string, IDatatransClient> datatransClientFactory = endpoint => new DatatransClient.DatatransClient(endpoint);
            _container.RegisterInstance(datatransClientFactory);

            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();

            Func<DatatransCheckoutPaymentMethod> datatransPaymentMethod = () =>
            {
                var paymentMethod = new DatatransCheckoutPaymentMethod(_container.Resolve<IDatatransCheckoutService>(), _container.Resolve<Func<string, IDatatransClient>>());
                paymentMethod.Name = "Datatrans Checkout Gateway";
                paymentMethod.Description = "Datatrans Checkout payment gateway integration";
                paymentMethod.LogoUrl = "https://raw.githubusercontent.com/VirtoCommerce/vc-module-datatrans/master/Datatrans.Checkout/Content/logo.png";
                paymentMethod.Settings = settingsManager.GetModuleSettings("Datatrans.Checkout");
                return paymentMethod;
            };

            var paymentMethodsService = _container.Resolve<IPaymentMethodsService>();
            paymentMethodsService.RegisterPaymentMethod(datatransPaymentMethod);
        }
    }
}
