using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Mapping;
using Ryanair.Reservation.Models;
using Ryanair.Reservation.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Xunit;

namespace Ryanair.Reservation.Controllers
{
    public class ReservationController_Test
    {
        #region MockDatabase
        public ReservationApiDbContext InitializeDatabase(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ReservationApiDbContext>().UseInMemoryDatabase(databaseName).Options;
            var context = new ReservationApiDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            IEnumerable<FlightRequest> results;
            //TODO - Read the seed data fileName from the Settings files -> appsettings.json
            using (StreamReader r = new StreamReader(@".\..\..\..\..\..\InitialState.json"))
            {
                string json = r.ReadToEnd();
                results = JsonConvert.DeserializeObject<IEnumerable<FlightRequest>>(json);
                foreach (var item in results)
                {
                    FlightEntity flight = new FlightEntity
                    {
                        FlightKey = item.key,
                        Origin = item.origin,
                        Destination = item.destination,
                        Time = item.time,
                    };
                    context.Flights.Add(flight);
                }
            }
            context.SaveChanges();
            return context;

        }
        #endregion MockDatabase

        [Fact]
        public async void CreateReservation_ReservationController_Test()
        {
            ////Arrange
            ///
            var db = InitializeDatabase("CreateReservation_ReservationController_Test");
            var loggerServiceMock = new Mock<ILogger<ReservationController>>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var reservationNumberService = new ReservationNumberService();

            var reservationService = new ReservationService(db, mapper, reservationNumberService, configuration);

            var reservationController = new ReservationController(loggerServiceMock.Object,
                                                      mapper,
                                                      reservationService);

            ReservationRequest createReservation = new ReservationRequest
            {
                email = "ana.gonzalez@contact.com",
                creditCard = "123456789",
                flights = new ReservationFlightRequest[] 
                {
                    new ReservationFlightRequest 
                    {
                        key = "Flight00001",
                        passengers = new PassengersReservationDetailsRequest[]
                            {
                               new PassengersReservationDetailsRequest  { name="Robert Niro", bags= 3, seat= 1 },
                               new PassengersReservationDetailsRequest { name="John Smith", bags= 3, seat= 2 },
                               new PassengersReservationDetailsRequest  { name="Seb True", bags= 3, seat= 3 }
                            }
                    },
                    new ReservationFlightRequest
                    {
                        key = "Flight00103",
                        passengers = new PassengersReservationDetailsRequest[]
                            {
                               new PassengersReservationDetailsRequest  { name="Robert Niro", bags= 3, seat= 1 },
                               new PassengersReservationDetailsRequest { name="John Smith", bags= 3, seat= 2 },
                               new PassengersReservationDetailsRequest  { name="Seb True", bags= 3, seat= 3 }
                            }
                    }
                }
            };
            // Act on Test  
            var actionResult = await reservationController.CreateReservationAsync(createReservation);
            var content = actionResult.Result as Microsoft.AspNetCore.Mvc.CreatedResult;
            var result = content.Value as ReservationResponse;
            ///Assert
            Assert.Equal((int)HttpStatusCode.Created, content.StatusCode.Value);
            Assert.Equal("AAA001" , result.reservationNumber);
        }
        [Fact]
        public async void GetReservation_ReservationController_Test()
        {
            ////Arrange
            var contextMock = InitializeDatabase("GetReservation_ReservationController_Test");
            var loggerServiceMock = new Mock<ILogger<ReservationController>>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var reservationNumberService = new ReservationNumberService();

            var reservationService = new ReservationService(contextMock, mapper, reservationNumberService, configuration);
            var reservationController = new ReservationController(loggerServiceMock.Object,
                                                                      mapper,
                                                                     reservationService);

            ///Act
            ReservationRequest createReservation = new ReservationRequest
            {
                email = "ana.gonzalez@contact.com",
                creditCard = "123456789",
                flights = new ReservationFlightRequest[]
               {
                    new ReservationFlightRequest
                    {
                        key = "Flight00001",
                        passengers = new PassengersReservationDetailsRequest[]
                            {
                               new PassengersReservationDetailsRequest  { name="Robert Niro", bags= 3, seat= 1 },
                               new PassengersReservationDetailsRequest { name="John Smith", bags= 3, seat= 2 },
                               new PassengersReservationDetailsRequest  { name="Seb True", bags= 3, seat= 3 }
                            }
                    },
                    new ReservationFlightRequest
                    {
                        key = "Flight00103",
                        passengers = new PassengersReservationDetailsRequest[]
                            {
                               new PassengersReservationDetailsRequest  { name="Robert Niro", bags= 3, seat= 1 },
                               new PassengersReservationDetailsRequest { name="John Smith", bags= 3, seat= 2 },
                               new PassengersReservationDetailsRequest  { name="Seb True", bags= 3, seat= 3 }
                            }
                    }
               }
            };
            // Act on Test  
            var actionResult = await reservationController.CreateReservationAsync(createReservation);
            var content = actionResult.Result as Microsoft.AspNetCore.Mvc.CreatedResult;
            var result = content.Value as ReservationResponse;

            GetReservationRequest newReservationNumber = new GetReservationRequest()
            {
                reservationNumber = result.reservationNumber
            };
            var actionResultGet = await reservationController.GetReservation(newReservationNumber);
            var contentGet = actionResultGet.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var resultGet = content.Value as GetReservationResponse;
 
            ///Assert
            Assert.Equal((int)HttpStatusCode.OK, contentGet.StatusCode.Value);
            Assert.Equal("AAA001", result.reservationNumber);
        }

    }
}
