using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Models;
using Ryanair.Reservation.Services;

namespace Ryanair.Reservation.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;


        public ReservationController(ILogger<ReservationController> logger,
                                     IMapper mapper,
                                     IReservationService reservationService)
        {
            _logger = logger;
            _mapper = mapper;
            _reservationService = reservationService;

        }

        [HttpGet("{reservationNumber}", Name = nameof(GetReservation))]
        [ProducesResponseType(400)] // Bad input data
        [ProducesResponseType(404)] // Not found
        [ProducesResponseType(200)] //OK
        public async Task<ActionResult<GetReservationResponse>> GetReservation(GetReservationRequest reservationNumber)
        {
            _logger.LogInformation(nameof(GetReservation), "Logging GetReservation");
            if (ModelState.IsValid)
            {
                var result = await _reservationService.GetReservationAsync(reservationNumber);
                _logger.LogInformation(nameof(GetReservation), "End GetReservation");
                if (result != null)
                    return Ok(result);
                else
                    return NotFound();
            }
            else
            {
                _logger.LogWarning(nameof(GetReservation), "Wrong input data");
                return BadRequest();
            }
        }
        // Create a Reservation
        [HttpPost("CreateReservation", Name = nameof(CreateReservationAsync))]
        [ProducesResponseType(400)] // Bad input data
        [ProducesResponseType(404)] // Not found
        [ProducesResponseType(500)] // Server Error
        [ProducesResponseType(201)] // created
        public async Task<ActionResult<ReservationResponse>> CreateReservationAsync(ReservationRequest reservation)
        {
            _logger.LogInformation(nameof(CreateReservationAsync), "Logging CreateReservation");
            if (ModelState.IsValid)
            {
                var reservationNumber = await _reservationService.CreateReservationAsync(reservation);
                _logger.LogInformation(nameof(CreateReservationAsync), "End CreateReservationAsync");
                if (reservationNumber != null)
                {
                    return Created( nameof(ReservationController.GetReservation)+ reservationNumber, reservationNumber);
                    //return Created(Url.Link(nameof(ReservationController.GetReservation), reservationNumber), reservationNumber);
                }
                else
                    return NotFound();
            }
            else
            {
                _logger.LogWarning(nameof(CreateReservationAsync), "Wrong input data");
                return BadRequest();
            }
        }
    }
}
