using System.Collections.Generic;

namespace Datatrans.Checkout.Core.Model
{
    public class DatatransAirlineData
    {
        public DatatransAirlineData()
        {
            Tickets = new List<DatatransTicket>();
        }

        public string CountryCode { get; set; }

        public string AgentCode { get; set; }

        public string PNR { get; set; }

        public string IssueDate { get; set; }

        public IList<DatatransTicket> Tickets { get; set; }
    }

    public class DatatransTicket
    {
        public DatatransTicket()
        {
            Flights = new List<DatatransFlight>();
        }

        public string Index { get; set; }

        public string TicketNumber { get; set; }

        public string PassengerName { get; set; }

        public string DescrCode { get; set; }

        public IList<DatatransFlight> Flights { get; set; }
    }

    public class DatatransFlight
    {
        public string Index { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public string Carrier { get; set; }

        public string Class { get; set; }

        public string FareBasis { get; set; }

        public string FlightNumber { get; set; }

        public string FlightDate { get; set; }
    }
}
