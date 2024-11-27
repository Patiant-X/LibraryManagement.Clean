using AutoMapper;
using LibraryManagement.Application.Features.Notifications.Commands.CreateNotification;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Domain;

namespace LibraryManagement.Application.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<CreateNotificationCommand, Notification>();
        }
    }
}
