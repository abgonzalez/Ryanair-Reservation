using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ryanair.Reservation.Models
{
    public class FlightsRequest :  IEnumerable
    {
        public List<FlightsRequest> flights = new List<FlightsRequest>();

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < flights.Count; i++)
                yield return flights[i];
        }
    }

    public class FlightRequest
    {
        public string key { get; set; }

        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime time { get; set; }
    }
}
