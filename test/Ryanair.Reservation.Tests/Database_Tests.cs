using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ryanair.Reservation.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Ryanair.Reservation.Data
{
    public class Database_Tests
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

        private void SeedData(ReservationApiDbContext repository)
        {
            #region Load Reservation
            ReservationEntity firstReservation = new ReservationEntity
            {
                CreditCard = "0123456789012345",
                Email = "john@contact.com",
                ReservationDetails = new List<ReservationDetailsEntity> {
                    new ReservationDetailsEntity {
                            FlightKey= "Flight00001",
                            Name= "John Smith",
                            NumberOfBags=2,
                            Seat=1,
                    },
                   new ReservationDetailsEntity {
                            FlightKey= "Flight00001",
                            Name= "Anna White",
                            NumberOfBags=2,
                            Seat=2,
                    },
                   new ReservationDetailsEntity {
                            FlightKey= "Flight00001",
                            Name= "John True",
                            NumberOfBags=0,
                            Seat=3,
                    },
                   new ReservationDetailsEntity {
                             FlightKey= "Flight00001",
                            Name= "Mary Smith",
                            NumberOfBags=2,
                            Seat=4,
                    },
                }
            };
            repository.Reservations.Add(firstReservation);
            ReservationEntity secondReservation = new ReservationEntity
            {
                CreditCard = "0123456789012345",
                Email = "mary@contact.com",
                ReservationDetails = new List<ReservationDetailsEntity> {
                      new ReservationDetailsEntity {
                            FlightKey= "Flight00052",
                            Name= "John Morris",
                            NumberOfBags=2,
                            Seat=1,
                    },
                   new ReservationDetailsEntity {
                            FlightKey= "Flight00052",
                            Name= "Anna Smith",
                            NumberOfBags=2,
                            Seat=2,
                    },
                   new ReservationDetailsEntity {
                            FlightKey= "Flight00052",
                            Name= "Tomm True",
                            NumberOfBags=0,
                            Seat=3
                    },
                   new ReservationDetailsEntity {
                             FlightKey= "Flight00052",
                            Name= "John Smith",
                            NumberOfBags=2,
                            Seat=4
                    },
                }
            };
            repository.Reservations.Add(secondReservation);
            #endregion
            repository.SaveChanges();

        }
        #endregion MockDatabase

        [Fact]
        public void ShouldReturnTheLoadedFlights()
        {

            // Initialize the database
            var repository = InitializeDatabase("ShouldReturnTheLoadedFlights");

            // Act
            var flights = repository.Flights;

            // Assert
            Assert.Equal(8, flights.Count());
            Assert.Contains(flights, d => d.FlightKey == "Flight00001");
            Assert.Contains(flights, d => d.FlightKey == "Flight00052");
            Assert.DoesNotContain(flights, d => d.FlightKey == "Flight00002");


        }
        [Fact]
        public void ShoulCreateReservations()
        {

            // Initialize the database
            var repository = InitializeDatabase("ShoulCreateReservations");
            SeedData(repository);

            //Act
            var reservations = repository.Reservations;


            // Assert
            Assert.Equal(2, repository.Reservations.Count());
            Assert.Contains(reservations, d => d.Email == "john@contact.com");
            Assert.Contains(reservations, d => d.CreditCard == "0123456789012345");
            Assert.Contains(reservations, d => d.ReservationDetails.Any(f => f.FlightKey == "Flight00001"));
            Assert.Contains(reservations, d => d.ReservationDetails.Any(f => f.FlightKey == "Flight00052"));
            Assert.DoesNotContain(reservations, d => d.ReservationDetails.Any(f => f.FlightKey == "Flight00002"));

        }

    }
}
