using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace Ryanair.Reservation.Data
{
    public class ReservationEntity
    {
        public ReservationEntity()
        {
            ReservationDetails = new List<ReservationDetailsEntity>();
        }

        public string ReservationNumber { get; set; }
        public string CreditCard { get; set; }
        public string Email { get; set; }


        // Navigation to Dependen Entity
        public virtual List<ReservationDetailsEntity> ReservationDetails { get; set; }
        
    }
}
