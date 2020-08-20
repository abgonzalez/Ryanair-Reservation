using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Mapping;
using Ryanair.Reservation.Models;
using Ryanair.Reservation.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ryanair.Reservation.Controllers
{
    public class FlightController_Test
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
        #endregion
        [Fact]
        public async Task GetAllFlightsAsync_FlightController_Test()
        {
            var mapperConfigMock = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapperServiceMock = mapperConfigMock.CreateMapper();
            var loggerServiceMock = new Mock<ILogger<FlightController>>();
            var contextMock = InitializeDatabase("GetAllFlightsAsync_FlightController_Test");
            var configurationMock = new Mock<IOptions<MySettings>>();
            var flightServiceMock = new FlightService(contextMock, mapperServiceMock,
                                                       configurationMock.Object);

            // Act on Test  
            var flightControllerMock = new FlightController(loggerServiceMock.Object,
                                                       mapperServiceMock,
                                                       flightServiceMock);
            var actionResult = await flightControllerMock.GetFlightsAsync();
            var content = actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var result = content.Value as List<FlightResponse>;
            ///Assert
            Assert.Equal(8, result.Count());
        }


        [Fact]
        public async void SearchFlights_FlightController_Test()
        {
            ////Arrange
            var mapperConfigMock = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapperServiceMock = mapperConfigMock.CreateMapper();
            var loggerServiceMock = new Mock<ILogger<FlightController>>();
            var contextMock = InitializeDatabase("SearchFlights_FlightController_Test");
            var configurationMock = new Mock<IOptions<MySettings>>();
            var flightServiceMock = new FlightService(contextMock, mapperServiceMock,
                                                       configurationMock.Object);

            // Act on Test  
            var flightControllerMock = new FlightController(loggerServiceMock.Object,
                                                       mapperServiceMock,
                                                        flightServiceMock);
            int passangers = 2;
            string origin = "DUBLIN";
            string destination = "LONDON";
            string dateOut = "2017-05-08";
            string dateIn = "2017-05-09";
            bool roundTrip = true;

            var actionResult = await flightControllerMock.GetFlightsAsync(passangers, origin,
                                                              destination, dateOut,
                                                              dateIn, roundTrip);
            var content = actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var result = content.Value as List<FlightResponse>;

            //Assert
            Assert.Equal(2, result.Count());
        }
    }
}
