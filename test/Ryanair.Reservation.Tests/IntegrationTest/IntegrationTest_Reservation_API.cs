using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Xunit;

namespace Ryanair.Reservation
{
    public class IntegrationTest_Reservation_API
    {
        private readonly HttpClient _client;
        public IntegrationTest_Reservation_API()
        {
            var configuration = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", false, true)
                                 .Build();

            var server = new TestServer(new WebHostBuilder()
                                .UseConfiguration(configuration)
                                .UseEnvironment("Development")
                                .UseKestrel()
                                .UseIISIntegration()
                                .ConfigureLogging(Program.ConfigureLogging)
                                        .UseStartup<Startup>());

            Program.InitializeDatabase(server.Host);
            _client = server.CreateClient();
        }
        [Fact]
        public async void GetAllFlights_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:5005/Flight");
            //Act
            var response = await _client.SendAsync(request);
            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async void SearchFlights_Test()
        {
            //Arrange
            int passengers = 2;
            string origin = "DUBLIN";
            string destination = "LONDON";
            string dateOut = "2017-05-08";
            string dateIn = "2017-05-09";
            bool roundTrip = true;
            string queryString= "?passengers=" +
                                 passengers.ToString() +
                                "&origin=" +
                                origin +
                                "&destination=" +
                                destination +
                                "&dateOut=" +
                                dateOut.ToString() +
                                "&dateIn=" +
                                dateIn.ToString() +
                                "&roundTrip=" +
                                (roundTrip ? "true" : "false");

            var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:5005/Flight" + queryString);
            //Act
            var response = await _client.SendAsync(request);
            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

     
    }
}
