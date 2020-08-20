using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Models
{
    public class GetReservationResponse  
    {
        public GetReservationResponse()
        {
            flights = new List<ReservationFlightResponse>();
        }
            public string reservationNumber { get; set; }
            public string email { get; set; }
            public List<ReservationFlightResponse> flights { get; set; }
    }
    public class ReservationFlightResponse
    {
        public ReservationFlightResponse()
        {
            passengers = new List<PassengersReservationDetailsResponse>();
        }
        public string key { get; set; }
        public List<PassengersReservationDetailsResponse> passengers { get; set; }

    }

    public class PassengersReservationDetailsResponse
    {
        public string name { get; set; }

        public int bags { get; set; }

        public int seat { get; set; }
    }
}
