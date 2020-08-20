using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ryanair.Reservation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            InitializeDatabase(host);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(ConfigureLogging)
                .UseStartup<Startup>()
                .Build();


        /// <summary>
        /// InitializeDatabase
        /// </summary>
        /// <param name="context">App's host context (including environment and configuration)</param>
        /// <param name="loggingBuilder">The ILoggingBuilde to add providers to</param>
        public static void InitializeDatabase(IWebHost host)
        {

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {

                    using (var context = services.GetRequiredService<ReservationApiDbContext>())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        IEnumerable<FlightRequest> results;

                        var configurationService = services.GetService<IConfiguration>();
                        var seedDataFile = configurationService["Settings:SeedDataFile"];
                        using (StreamReader r = new StreamReader(seedDataFile))
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
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }
        }
        /// <summary>
        /// Setup application logging
        /// </summary>
        /// <param name="context">App's host context (including environment and configuration)</param>
        /// <param name="loggingBuilder">The ILoggingBuilde to add providers to</param>
        public static void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            var env = context.HostingEnvironment;
            var config = context.Configuration;

            // Logging: The ILoggerFactory is used to keep track of all the different loggers that have been added. When
            //          using dependency injection to registered and can be accessed through dependency injection using the
            //          ILogger interface. Here there are three loggers that are added to the loggerFactory instance.
            loggingBuilder.AddDebug();

            if (env.IsDevelopment())
            {
                var serilogLogger = new LoggerConfiguration()
                                    .ReadFrom.Configuration(config)
                                    .CreateLogger();

                loggingBuilder.AddSerilog(serilogLogger);

            }

        }
    }
}
