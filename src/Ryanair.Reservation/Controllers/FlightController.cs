using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ryanair.Reservation.Models;
using Ryanair.Reservation.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class FlightController : Controller
    {
        private readonly ILogger<FlightController> _logger;
        private readonly IMapper _mapper;
        private readonly IFlightService _flightService;


        public FlightController(ILogger<FlightController> logger,
                               IMapper mapper,
                                IFlightService flightService)
        {
            _logger = logger;
            _mapper = mapper;
            _flightService = flightService;
        }



        [HttpGet("", Name = nameof(GetFlightsAsync))]
        [ProducesResponseType(400)] // Bad input data
        [ProducesResponseType(404)] // Not found
        [ProducesResponseType(200)] //OK
        public async Task<ActionResult<List<FlightResponse>>> GetFlightsAsync(int passengers = 0, string origin = null, string destination = null,
                                                      string dateOut = null, string dateIn = null, bool roundTrip = false)
        {

            _logger.LogInformation(nameof(GetFlightsAsync), "Logging GetAllFlightsAsync");
            if (ModelState.IsValid)
            {
                List<FlightResponse> flights = new List<FlightResponse>();
                if (passengers == 0 && origin == null && destination == null &&
                    dateOut == null && dateIn == null && roundTrip == false)
                {
                    _logger.LogInformation(nameof(GetFlightsAsync), "Looking for all the Flights");
                    flights = await _flightService.GetAllFlightsAsync();
                }
                else
                {
                    //  var queryString = HttpContext.Request.GetDisplayUrl();
                    //  _logger.LogInformation(nameof(GetFlightsAsync), "Searching for " + queryString + " on the Flights");
                    if (IsValidQueryString(passengers, origin, destination, dateOut, dateIn, roundTrip, out string Message))
                        flights = await _flightService.SearchingFlights(passengers, origin, destination, dateOut, dateIn, roundTrip);
                    else
                        return BadRequest(Message);
                }
                _logger.LogInformation(nameof(GetFlightsAsync), "End GetFlightsAsync");
                if (flights != null && flights.Count() != 0)
                    return Ok(flights);
                else
                    return NotFound(flights);
            }
            _logger.LogInformation(nameof(GetFlightsAsync), "Not found any Flights");
            return NotFound();

        }

        private bool IsValidQueryString(int passengers, string origin, string destination, string dateOut, string dateIn, bool roundTrip, out string Message)
        {
            Message = "";
            if (roundTrip && dateIn == null)
            {
                Message = "Invalid parameters,  if you are looking for a return Trip, you need to select a date In.";
                return false;
            }
            return true;

        }
    }
}
