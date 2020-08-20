using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using Ryanair.Reservation.Models;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Ryanair.Reservation.Filters;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Ryanair.Reservation.Mapping;
using Ryanair.Reservation.Data;
using Ryanair.Reservation.Services;

namespace Ryanair.Reservation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MySettings>(Configuration.GetSection("Settings"));
            services.AddSingleton<ReservationNumberService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IReservationService, ReservationService>();
            // Use in-memory database according to the requierment.
            services.AddDbContext<ReservationApiDbContext>(
                        options => options.UseInMemoryDatabase("Reservations"));

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Static", new CacheProfile { Duration = 86400 });
                options.Filters.Add<JsonExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddMvcCore().AddXmlSerializerFormatters();
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services
                .AddRouting(options => options.LowercaseUrls = true);
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader
                    = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector
                     = new CurrentImplementationApiVersionSelector(options);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
          //  loggerFactory.AddDebug(LogLevel.Information);
            var timingLogger = loggerFactory.CreateLogger("Ryanair.Reservation.Startup.TimingMiddleware");

            loggerFactory.AddDebug();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                app.UseSwaggerUi3WithApiExplorer(options =>
                {
                    options.GeneratorSettings
                        .DefaultPropertyNameHandling
                    = NJsonSchema.PropertyNameHandling.CamelCase;
                    options.GeneratorSettings.Title = Configuration.GetSection("API").Get<Dictionary<string, string>>()["Title"] ?? "Ryanair.Reservation.API";
                });

            }


            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                var timer = new Stopwatch();
                timer.Start();

                // Middleware: Calling the next delegate will invoke the next piece of middleware
                await next();

                // Middleware: Code after 'next' will usually run after another piece of middleware
                //             has written a response, so context.Response should not be written to here.
                timer.Stop();
                timingLogger.LogInformation("Request to {RequestMethod}:{RequestPath} processed in {ElapsedMilliseconds} ms", context.Request.Method, context.Request.Path, timer.ElapsedMilliseconds);
            });


            app.UseMvc();
        }

   
    }
}
