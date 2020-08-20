using Ryanair.Reservation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Services
{
    public interface IReservationService
    {
        Task<ReservationResponse> CreateReservationAsync(ReservationRequest reservation);
        Task<GetReservationResponse> GetReservationAsync(GetReservationRequest reservationNumber);

    }

}
