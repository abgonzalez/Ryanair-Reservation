using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Mapping;
using Ryanair.Reservation.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;


namespace Ryanair.Reservation.Services
{
    public class ReservationService_Test
    {
        #region Mock Database
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
        public async void CreateReservation_ReservationService_Test()
        {
            ////Arrange
            ///
            var db = InitializeDatabase("CreateReservation_ReservationService_Test");
            var loggerServiceMock = new Mock<ILogger<ReservationService>>();

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
            var result = await reservationService.CreateReservationAsync(createReservation);

            ///Asserrt
            Assert.Equal("AAA001", result.reservationNumber);
        }

        
        [Fact]
        public async void GetReservation_ReservationService_Test()
        {

            ////Arrange
            ///
            var db = InitializeDatabase("GetReservation_ReservationService_Test");
            var loggerServiceMock = new Mock<ILogger<ReservationService>>();

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
            var resultCreation = await reservationService.CreateReservationAsync(createReservation);


            GetReservationRequest newReservationNumber = new GetReservationRequest()
            {
                reservationNumber = resultCreation.reservationNumber
            };
            var resultGet = await reservationService.GetReservationAsync(newReservationNumber);

            // Assert
            // The process returns 2 flights.
            Assert.Equal(2, resultGet.flights.Count());

        }
    }
}
