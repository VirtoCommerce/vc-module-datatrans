using System.Xml.Linq;
using coreModel = Datatrans.Checkout.Core.Model;

namespace Datatrans.Checkout.DatatransClient.Converters
{
    public static class SettlementServiceRequestConverter
    {
        public static string ToDatatransRequest(this coreModel.DatatransSettlementRequest coreModel)
        {
            XElement requestXml =
                new XElement("paymentService", new XAttribute("version", coreModel.ServiceVersion),
                    new XElement("body", new XAttribute("merchantId", coreModel.MerchangId),
                        new XElement("transaction", new XAttribute("refno", coreModel.ReferenceNumber),
                            new XElement("request",
                                new XElement("amount", coreModel.Amount),
                                new XElement("currency", coreModel.Currency),
                                new XElement("uppTransactionId", coreModel.TransactionId)
                            )
                        )
                    )
                );

            return requestXml.ToString();
        }
    }
}