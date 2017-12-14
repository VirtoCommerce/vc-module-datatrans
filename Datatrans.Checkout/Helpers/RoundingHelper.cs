using System;

namespace Datatrans.Checkout.Helpers
{
    public static class RoundingHelper
    {
        public static int Round(this decimal value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        public static int ToInt(this decimal value)
        {
            return (value * 100).Round();
        }
    }
}