using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NJsonSchema.Infrastructure;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Infraestructure;
using Ryanair.Reservation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Services
{
    public class FlightService : IFlightService

    {
        private readonly ReservationApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<MySettings> _configuration;

        public FlightService(
            ReservationApiDbContext context,
            IMapper mapper,
            IOptions<MySettings> configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        /// <summary>
        ///  GetAllFlightsAsync
        ///  This function get all the Flights availables on the database. 
        ///  It's getting the information from the FightController with the methog GET 
        /// </summary>
        /// <returns>List<FlightResponse> - List all the Flights </returns>
        public async Task<List<FlightResponse>> GetAllFlightsAsync()
        {
            var entity = await _context.Flights.ToListAsync();

            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<List<FlightResponse>>(entity.ToList());
        }
        
        
        
        /// <summary>
        ///  GetAllFlightsAsync
        ///  This function get all the Flights availables on the database. 
        ///  It's getting the information from the FightController with the methog GET 
        /// </summary>
        /// <returns>List<FlightResponse> - List all the Flights </returns>
        /// 
        //>/Flight?passengers=3&origin=DUBLIN&destination=LONDON&dateOut=2017-05-08&dateIn=2017-05-10&roundTrip=true
        public async Task<List<FlightResponse>> SearchingFlights(int passengers = 0, string origin = null, string destination = null,
                                                      string dateOut = null, string dateIn = null, bool roundTrip = false)
        {
            // throw new NotImplementedException();
            IQueryable<FlightEntity> departureFlights = _context.Flights;
            //TODO: To do an extension to find the flights avaialbe for a passengersNo.
            if (passengers != 0)
                departureFlights = getFlightDetails(departureFlights, passengers);
            if (origin != null)
                departureFlights = departureFlights.Where(c => c.Origin == origin);
            if (destination != null)
                departureFlights = departureFlights.Where(c => c.Destination == destination);
            if (dateOut != null)
            {
                DateTime dateOutFlight = (DateTime)dateOut.StringToDate();
                departureFlights = departureFlights.Where(c => c.Time >= dateOutFlight);
                if (dateIn != null)
                {
                    DateTime dateInFlight = (DateTime)dateIn.StringToDate();
                    departureFlights = departureFlights.Where(c => c.Time <= dateInFlight);
                }
            }
            List<FlightEntity> foundFlights = departureFlights.ToList();

            // If they need to find the roundTrip back, we need to look for the new flights.
            if (roundTrip && dateIn != null)
            {
                IQueryable<FlightEntity> returnFlights = _context.Flights;
                if (origin != null)
                    returnFlights = returnFlights.Where(c => c.Destination == origin);
                if (destination != null)
                    returnFlights = returnFlights.Where(c => c.Origin == destination);
                if (dateOut != null)
                {
                    DateTime dateOutFlight = (DateTime)dateOut.StringToDate();
                    returnFlights = returnFlights.Where(c => c.Time >= dateOutFlight);
                }
                if (dateIn != null)
                {
                    DateTime dateInFlight = (DateTime)dateIn.StringToDate();
                    returnFlights = returnFlights.Where(c => c.Time <= dateInFlight);
                }
                foundFlights.AddRange(returnFlights);
            }
            var flights = _mapper.Map<List<FlightResponse>>(foundFlights);
            return flights;
        }

        #region private
        /// <summary>
        /// getFlightDetails
        /// This function return the Flights which has at least the required seats number(seatsRequired) available
        /// </summary>
        /// <param name="departureFlights"> All the avialables flights</param>
        /// <param name="seatsRequired"> Minumim number of requried seats</param>
        /// <returns></returns>
        private IQueryable<FlightEntity> getFlightDetails(IQueryable<FlightEntity> departureFlights, int seatsRequired = 0)
        {
            var StandardPassengersLimitByFlight = _configuration.TryGetPropertyValue<int>("Settings:Reservation:SeatsLimitByFlight", 50);
            // Now, this limit is different, because this limit has to be included the searchingPassengersNumber.
            var bookedFlights = from rd in _context.ReservationDetails
                                group rd by rd.FlightKey into global
                                select new { FlightKey = global.Key, Seats = global.Count() };

            var seatsAvailableByFlight =  (from a in _context.Flights
                                         join booked in bookedFlights on a.FlightKey equals booked.FlightKey
                                         into r
                                         from result in r.DefaultIfEmpty()
                                         select new
                                         {
                                             a.FlightKey,
                                             FreeSeats = r.FirstOrDefault() == null ? StandardPassengersLimitByFlight : StandardPassengersLimitByFlight - r.FirstOrDefault().Seats
                                         });

            var query = from d in departureFlights
                        join f in seatsAvailableByFlight on d.FlightKey equals f.FlightKey
                        where f.FreeSeats > seatsRequired
                        select d;


            return query;

        }
        #endregion
    }
}
