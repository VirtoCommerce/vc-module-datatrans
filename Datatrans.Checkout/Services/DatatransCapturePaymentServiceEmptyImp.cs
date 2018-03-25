using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.Core.Services;

namespace Datatrans.Checkout.Services
{
    /// <summary>
    /// Mock implementation, should be overridden in custom implementations 
    /// </summary>
    public class DatatransCapturePaymentServiceEmptyImp : IDatatransCapturePaymentService
    {
        #region Implementation of IDatatransCapturePaymentService

        public DatatransAirlineData GetAirlineData(GetAirlineDataContext context)
        {
            return null;
        }

        #endregion
    }
}