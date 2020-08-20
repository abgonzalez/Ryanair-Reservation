using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ryanair.Reservation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ryanair.Reservation.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace Ryanair.Reservation.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ReservationApiDbContext _context;
		private readonly IMapper _mapper;
        private readonly ReservationNumberService _reservationNumber;
        private readonly IConfiguration _configuration;


        public ReservationService(
            ReservationApiDbContext context,
            IMapper mapper,
            ReservationNumberService reservationNumber,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _reservationNumber = reservationNumber;
            _configuration = configuration;
        }
        public async Task<GetReservationResponse> GetReservationAsync(GetReservationRequest reservationNumber)
        {

            var reservationEntity = await _context.Reservations
                .SingleOrDefaultAsync(r => r.ReservationNumber == reservationNumber.reservationNumber);

            if (reservationEntity == null) return null;
            reservationEntity.ReservationDetails = _context.ReservationDetails.Where(c => c.ReservationNumber == reservationNumber.reservationNumber).ToList();

            GetReservationResponse result= _mapper.Map<GetReservationResponse>(reservationEntity);
            string flightItemOld = "";
            foreach (var flightItem in reservationEntity.ReservationDetails )
            {
                if (string.Compare(flightItem.FlightKey, flightItemOld) != 0)
                {
                    var flightToInsert = new ReservationFlightResponse
                    {
                        key = flightItem.FlightKey,
                        passengers = new List<PassengersReservationDetailsResponse>()
                    };
                    
                    result.flights.Add(flightToInsert);
                }
                var passengerToInsert = new PassengersReservationDetailsResponse
                {
                    name = flightItem.Name,
                    bags = flightItem.NumberOfBags,
                    seat = flightItem.Seat
                };
                result.flights.Where(c => c.key == flightItem.FlightKey).FirstOrDefault().passengers.Add(passengerToInsert);
                flightItemOld = flightItem.FlightKey;
            }
            return result;
        }
        public async Task<ReservationResponse> CreateReservationAsync(ReservationRequest reservation)
        {
            var newReservationToCreate = _mapper.Map<ReservationEntity>(reservation);
            //TODO: Complete the mapping from  CreateReservationRequest to ReservationEntity;
            //Create function in Mapping folder
            foreach( var flight in reservation.flights)
            {
                foreach ( var item in flight.passengers)
                {
                    ReservationDetailsEntity detailsItem = new ReservationDetailsEntity();
                    detailsItem.FlightKey = flight.key; detailsItem.Name = item.name;
                    detailsItem.NumberOfBags = item.bags;
                    detailsItem.Seat = item.seat;
                    newReservationToCreate.ReservationDetails.Add(detailsItem);
                }
            }
            if ( newReservationToCreate !=null)
            {
                if( IsValidReservation(newReservationToCreate))
                {
                    _reservationNumber.Next();
                    newReservationToCreate.ReservationNumber = _reservationNumber.Value;
                    newReservationToCreate.ReservationDetails.ForEach(c => c.ReservationNumber = _reservationNumber.Value);
 
                    await _context.Reservations.AddAsync(newReservationToCreate);
                    await _context.SaveChangesAsync();
                    return new ReservationResponse
                    {
                        reservationNumber = _reservationNumber.Value
                    };
                }
                else
                {
                    throw new InvalidOperationException("Could not create booking. This reservation doesn't follow the below criteria:\n " +
                                                        "There is a maximum of 50 bags per flight in total for all the passengers." +
                                                        "Each passenger can have a maximum of 5 bags per flight. " +
                                                        "There are 50 seats available per flight."+
                                                        "Flight doesn't exit");
                }
            }
            return null;
        }

        #region private
        /// <summary>
        /// Check is the Flight is still available , the constrains are :
        ///  -  There is a maximum of 50 bags per flight in total for all the passengers.
        ///  -  Each passenger can have a maximum of 5 bags per flight.
        ///  -  There are 50 seats available per flight, numbered sequentially: “01”, “02”… “50”.
        ///  -  Apart from this, I am checking if there are more than 50 bookings in a flight, in case the seats are duplicated.
        /// </summary>
        /// <returns></returns>
        private bool IsValidReservation(ReservationEntity reservationToValidate)
        {
            var flightKeys = reservationToValidate.ReservationDetails.Select(m => m.FlightKey).Distinct();
            foreach (var flightKey in flightKeys)
            {
                if (!IsValidFlight(flightKey))
                    return false;
                // Calculate the number of bags in the current Reservation by every Flight
                // Calculate the number of bags in the previous Reservations by every Flight
                // Check the Total of number of bags are not more than the limit
                // TODO: Check if the seats are duplicated.
                int BagsLimitByFlight = int.Parse(_configuration["Settings:Reservation:BagsLimitByFlight"]);
                int SeatsLimitByFlight = int.Parse(_configuration["Settings:Reservation:SeatsLimitByFlight"]);
                int TicketsLimitByFlight = int.Parse(_configuration["Settings:Reservation:TicketsLimitByFlight"]);
                getFlightDetails(flightKey, out int TotalBagsCurrentReservation, out int MaxSeatCurrentReservation, out int MaxTicketsCurrentReservation, reservationToValidate.ReservationDetails);
                getFlightDetails(flightKey, out int TotalBagsByFlight, out int MaxSeatByFlight, out int MaxTicketsByFlight, null);
                if (((TotalBagsCurrentReservation+ TotalBagsByFlight) > BagsLimitByFlight) ||
                    (Math.Max(MaxSeatCurrentReservation, MaxSeatByFlight) > SeatsLimitByFlight) ||
                    (Math.Max(MaxTicketsCurrentReservation, MaxTicketsByFlight) > TicketsLimitByFlight))
                    return false;
            }
            return true;
         }
        private bool IsValidFlight(string flight)
        {
            return _context.Flights.Where(c => c.FlightKey == flight).Count() != 0;
        }
        private void getFlightDetails(string FlightKey, out int TotalBagsByFlight,
                                      out int MaxSeatByFlight,
                                      out int MaxTicketsByFlight,
                                   List<ReservationDetailsEntity> reservationDetailsToLookFor = null)
        {
            TotalBagsByFlight = 0;
            MaxSeatByFlight = 0;
            MaxTicketsByFlight = 0;

            //    var passengerReservationDetails=null;
            if (reservationDetailsToLookFor == null)
                reservationDetailsToLookFor = _context.ReservationDetails.Where(c => c.FlightKey == FlightKey).ToList();

            foreach (var rervationItem in reservationDetailsToLookFor)
            {
                TotalBagsByFlight += rervationItem.NumberOfBags;
                MaxSeatByFlight = Math.Max(rervationItem.Seat, MaxSeatByFlight);
                ++MaxTicketsByFlight;
            }
        }
        #endregion

    }
}
