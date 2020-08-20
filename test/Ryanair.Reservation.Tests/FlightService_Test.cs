using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class FlightService_Test
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
        #region FlightService Tests

        [Fact]
        public async void GetAllFlightsAsync_FlightService_Test()
        {
            var mapperConfigMock = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapperServiceMock = mapperConfigMock.CreateMapper();
            var contextMock = InitializeDatabase("GetAllFlightsAsync_FlightService_Test");
            var configurationMock = new Mock<IOptions<MySettings>>();
            var flightServiceMock = new FlightService(contextMock, mapperServiceMock,
                                                       configurationMock.Object);
            // Act
            var result = await flightServiceMock.GetAllFlightsAsync();

      
            //Assert
            Assert.Equal(8, result.Count());
            
        }

        #endregion
        [Fact]
        public async void SearchFlightsByOriginDestination_FlightService_Test()
        {

            // Initialize the database
            var mapperConfigMock = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapperServiceMock = mapperConfigMock.CreateMapper();
            var contextMock = InitializeDatabase("SearchFlightsByOriginDestination_FlightService_Test");
            var configurationMock = new Mock<IOptions<MySettings>>();
            var flightServiceMock = new FlightService(contextMock, mapperServiceMock,
                                                       configurationMock.Object);

            // Act
            int passangers = 2;
            string origin = "DUBLIN";
            string destination = "LONDON";
            string dateOut = "2017-05-08";
            string dateIn = "2017-05-09";
            bool roundTrip = true;
            var result = await flightServiceMock.SearchingFlights(passangers, origin,
                                                              destination, dateOut,
                                                              dateIn, roundTrip);

            // Assert
            Assert.Equal(2, result.Count());

        }
    }
}
