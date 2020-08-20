namespace Ryanair.Reservation.Models
{
    public class MySettings 
    {
        public string SeedDataFile { get; set; }
        public ConstrainsFlights Reservation { get; set; }
        public class ConstrainsFlights
        {
            public int BagsLimitByFlight { get; set; }
            public int BagsLimitByPassenger { get; set; }
            public int SeatsLimitByFlight { get; set; }
            public int TicketsLimitByFlight { get; set; }
            public string SeedDataFile { get; set; }
        }

    }
}