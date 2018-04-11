using System.Linq;
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

            if (coreModel.AirlineData != null)
            {
                var ticketIndex = 1;
                foreach (var datatransTicket in coreModel.AirlineData.Tickets)
                {
                    var flighIndex = 1;
                    datatransTicket.Index = (ticketIndex++).ToString();
                    foreach (var datatransFlight in datatransTicket.Flights)
                    {
                        datatransFlight.Index = (flighIndex++).ToString();
                    }
                }

                XElement airlineDataXml = new XElement("AIRLINEDATA",
                    new XElement("CountryCode", coreModel.AirlineData.CountryCode),
                    new XElement("AgentCode", coreModel.AirlineData.AgentCode),
                    new XElement("PNR", coreModel.AirlineData.PNR),
                    new XElement("IssueDate", coreModel.AirlineData.IssueDate),

                    from ticket in coreModel.AirlineData.Tickets
                    select new XElement("Ticket", new XAttribute("nr", ticket.Index),
                        new XElement("TicketNumber", ticket.TicketNumber),
                        new XElement("PassengerName", ticket.PassengerName),
                        new XElement("DescrCode", ticket.DescrCode),

                        from flight in ticket.Flights
                        select new XElement("Flight", new XAttribute("nr", flight.Index),
                            new XElement("Origin", flight.Origin),
                            new XElement("Destination", flight.Destination),
                            new XElement("Carrier", flight.Carrier),
                            new XElement("Class", flight.Class),
                            new XElement("FareBasis", flight.FareBasis),
                            new XElement("FlightNumber", flight.FlightNumber),
                            new XElement("FlightDate", flight.FlightDate)))
                    );

                var requestElement = requestXml.Descendants().FirstOrDefault(x => x.Name == "request");
                requestElement?.Add(airlineDataXml);
            }

            return requestXml.ToString();
        }
    }
}