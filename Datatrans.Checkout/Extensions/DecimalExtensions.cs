using System;

namespace Datatrans.Checkout.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal NormalizeDecimal(this decimal source)
        {
            return Math.Abs(source);
        }
    }
}