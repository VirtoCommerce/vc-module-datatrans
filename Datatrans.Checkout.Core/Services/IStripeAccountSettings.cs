namespace Stripe.Checkout.Core.Services
{
    public interface IStripeAccountSettings
    {
        string PublishableKey { get; }

        string SecretKey { get; }
    }
}