using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Models
{
    public class GetReservationRequest  
    {
            [Required]
            [MinLength(6, ErrorMessage = "Reservation Number can't be smaller than 6 characters")]
            [MaxLength(6, ErrorMessage = "Reservation Number can't be longer than 6 characters")]
            [DisplayFormat(DataFormatString = "{######}")]
            public string reservationNumber { get; set; }
    }
}
