using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Data
{
    public class FlightEntity : IEnumerable
    {
        [Column("key")]
        public string FlightKey { get; set; }
        [Column("origin")]
        public string Origin { get; set; }
        [Column("destination")]
        public string Destination { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
    }
}
