using System.Xml.Linq;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient.Converters
{
    public static class StatusServiceRequestConverter
    {
        public static string ToDatatransRequest(this coreModel.DatatransTransactionRequest coreModel)
        {
            XElement requestXml =
                new XElement("statusService", new XAttribute("version", coreModel.ServiceVersion),
                    new XElement("body", new XAttribute("merchantId", coreModel.MerchantId),
                        new XElement("transaction",
                            new XElement("request",
                                new XElement("uppTransactionId", coreModel.TransactionId),
                                new XElement("reqtype", coreModel.ReqestType)
                            )
                        )
                    )
                );

            return requestXml.ToString();
        }
    }
}