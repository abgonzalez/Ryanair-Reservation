using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ryanair.Reservation.Data
{
    public class ReservationDetailsEntity :  IEnumerable
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; }
        public string FlightKey { get; set; }
        public string Name { get; set; }
        public int NumberOfBags { get; set; }
        public int Seat { get; set; }

        public virtual FlightEntity FlightKeyNavigation { get; set; }
        public virtual ReservationEntity ReservationNoNavigation { get; set; }

       
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

      
    }
}
