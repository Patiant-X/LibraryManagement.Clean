using AutoMapper;
using LibraryManagement.Application.Features.Reservations.Commands.CreateReservation;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Domain;

namespace LibraryManagement.Application.MappingProfiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, ReservationDto>().ReverseMap();
            CreateMap<CreateReservationCommand, Reservation>().ReverseMap();
        }
    }
}
