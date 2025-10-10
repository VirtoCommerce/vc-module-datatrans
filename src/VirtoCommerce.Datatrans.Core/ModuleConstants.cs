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


            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return Sandbox;
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
