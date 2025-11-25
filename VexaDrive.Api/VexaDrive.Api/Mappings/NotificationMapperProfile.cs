using AutoMapper;
using VexaDriveAPI.DTO.Notification;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Mapping
{
    public class NotificationMapperProfile : Profile
    {
        public NotificationMapperProfile()
        {
            // Model → DTO
            CreateMap<Notification, NotificationListDTO>();
            CreateMap<Notification, NotificationDetailsDTO>();
        }
    }
}
