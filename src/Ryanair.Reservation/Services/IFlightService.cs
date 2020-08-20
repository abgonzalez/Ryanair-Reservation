using Ryanair.Reservation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Services
{
    public interface IFlightService
    {
        Task<List<FlightResponse>> GetAllFlightsAsync();

        Task<List<FlightResponse>> SearchingFlights(int passengers = 0, string origin = null, string destination = null,
                                                 string dateOut = null, string dateIn = null, bool roundTrip = false);
    }
}
