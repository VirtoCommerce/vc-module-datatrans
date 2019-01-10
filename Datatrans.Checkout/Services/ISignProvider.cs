namespace Datatrans.Checkout.Services
{
    public interface ISignProvider
    {
        string Sign(string merchantId, int amount, string currency, string refno, string aliasCC = null);
    }
}
