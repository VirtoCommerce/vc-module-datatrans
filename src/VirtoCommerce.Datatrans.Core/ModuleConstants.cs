using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Datatrans.Core;

public static class ModuleConstants
{
    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor Sandbox { get; } = new()
            {
                Name = "VirtoCommerce.Payment.Datatrans.Sandbox",
                GroupName = "Payment|Datatrans",
                ValueType = SettingValueType.Boolean,
                DefaultValue = true,
            };

            public static SettingDescriptor ReturnUrl { get; } = new()
            {
                Name = "VirtoCommerce.Payment.Datatrans.ReturnUrl",
                GroupName = "Payment|Datatrans",
                ValueType = SettingValueType.ShortText,
                DefaultValue = "/account/orders/{orderId}/payment",
            };

            public static SettingDescriptor PaymentMode { get; } = new()
            {
                Name = "VirtoCommerce.Payment.Datatrans.PaymentMode",
                GroupName = "Payment|Datatrans",
                ValueType = SettingValueType.ShortText,
                DefaultValue = "SecureFields",
                AllowedValues = new object[] { "SecureFields", "Lightbox" },
            };

            public static SettingDescriptor LightboxPaymentMethods { get; } = new()
            {
                Name = "VirtoCommerce.Payment.Datatrans.LightboxPaymentMethods",
                GroupName = "Payment|Datatrans",
                ValueType = SettingValueType.ShortText,
                DefaultValue = "",
            };

            public static SettingDescriptor LightboxAutoSettle { get; } = new()
            {
                Name = "VirtoCommerce.Payment.Datatrans.LightboxAutoSettle",
                GroupName = "Payment|Datatrans",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return Sandbox;
                    yield return ReturnUrl;
                    yield return PaymentMode;
                    yield return LightboxPaymentMethods;
                    yield return LightboxAutoSettle;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }
}
