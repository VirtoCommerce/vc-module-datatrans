using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Datatrans.Core;
using VirtoCommerce.Datatrans.Core.Models;
using VirtoCommerce.Datatrans.Core.Services;
using VirtoCommerce.Datatrans.Data.Providers;
using VirtoCommerce.Datatrans.Data.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Datatrans.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<DatatransOptions>().Bind(Configuration.GetSection("Payments:Datatrans")).ValidateDataAnnotations();

        serviceCollection.AddTransient<DatatransPaymentMethod>();

        serviceCollection.AddTransient<IDatatransClient, DatatransClient>();

        serviceCollection.AddHttpClient("Datatrans", (sp, http) =>
        {
            var opt = sp.GetRequiredService<IOptions<DatatransOptions>>().Value;
            http.BaseAddress = new Uri(opt.UseSandbox ? opt.SandboxBaseUrl : opt.ProductionBaseUrl);
            http.Timeout = TimeSpan.FromSeconds(30);

            if (!string.IsNullOrEmpty(opt.MerchantId))
            {
                var bytes = Encoding.UTF8.GetBytes($"{opt.MerchantId}:{opt.Secret}");
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            }
        });
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        var paymentMethodsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPaymentMethodsRegistrar>();
        paymentMethodsRegistrar.RegisterPaymentMethod(() => appBuilder.ApplicationServices.GetService<DatatransPaymentMethod>());

        settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.General.AllGeneralSettings, nameof(DatatransPaymentMethod));

    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
