using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Models
{
    public class FlightResponse 
    {
        public DateTime time { get; set; }
        public string key { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }

    }
}
