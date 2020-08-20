using AutoMapper;
using Ryanair.Reservation.Models;
using Ryanair.Reservation.Data;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Ryanair.Reservation.Mapping
{
    public class MappingProfile :Profile
    {
       
        public MappingProfile()
        {

            CreateMap<FlightResponse, FlightEntity>()
                .ForMember(dest => dest.Time, source => source.MapFrom(src => src.time))
                .ForMember(dest => dest.FlightKey, source => source.MapFrom(src => src.key))
                .ForMember(dest => dest.Origin, source => source.MapFrom(src => src.origin))
                .ForMember(dest => dest.Destination, source => source.MapFrom(src => src.destination));


            CreateMap<FlightEntity, FlightResponse>()
                .ForMember(dest => dest.time, source => source.MapFrom(src => src.Time))
                .ForMember(dest => dest.key, source => source.MapFrom(src => src.FlightKey))
                 .ForMember(dest => dest.origin, source => source.MapFrom(src => src.Origin))
                  .ForMember(dest => dest.destination, source => source.MapFrom(src => src.Destination));

            CreateMap<FlightEntity, FlightRequest>()
                .ForMember(dest => dest.time, source => source.MapFrom(src => src.Time))
                .ForMember(dest => dest.key, source => source.MapFrom(src => src.FlightKey))
                .ForMember(dest => dest.origin, source => source.MapFrom(src => src.Origin))
                .ForMember(dest => dest.destination, source => source.MapFrom(src => src.Destination));


            // TODO: Manualy the map between ReservationFlightRequest-> ReservationDetailsEntity
            CreateMap<ReservationFlightRequest, ReservationDetailsEntity>()
                        .ForMember(dest => dest.FlightKey, source => source.MapFrom(src => src.key))
                        .ForMember(dest => dest.Id, source => source.Ignore())
                        .ForMember(dest => dest.ReservationNumber, source => source.Ignore())
                        .ForMember(dest => dest.Name, source => source.Ignore())
                        .ForMember(dest => dest.NumberOfBags, source => source.Ignore())
                        .ForMember(dest => dest.Seat, source => source.Ignore())
                        .ForMember(dest => dest.FlightKeyNavigation, source => source.Ignore())
                        .ForMember(dest => dest.ReservationNoNavigation, source => source.Ignore());


            CreateMap<ReservationRequest, ReservationEntity>()
                   .ForMember(dest => dest.Email, source => source.MapFrom(src => src.email))
                   .ForMember(dest => dest.CreditCard, source => source.MapFrom(src => src.creditCard))
                   .ForMember(dest => dest.ReservationNumber, source => source.Ignore())
                   .ForMember(dest => dest.ReservationDetails, source => source.Ignore());


            CreateMap<GetReservationResponse, ReservationEntity>()
                   .ForMember(dest => dest.ReservationNumber, source => source.MapFrom(src => src.reservationNumber))
                   .ForMember(dest => dest.Email, source => source.MapFrom(src => src.email))
                   .ForMember(dest => dest.CreditCard, source => source.Ignore())
                   .ForMember(dest => dest.ReservationDetails, source => source.Ignore());

            CreateMap<ReservationEntity, GetReservationResponse>()
                     .ForMember(dest => dest.reservationNumber, source => source.MapFrom(src => src.ReservationNumber))
                     .ForMember(dest => dest.email, source => source.MapFrom(src => src.Email))
                     .ForMember(dest => dest.flights, source => source.Ignore());
        }

    }
}
