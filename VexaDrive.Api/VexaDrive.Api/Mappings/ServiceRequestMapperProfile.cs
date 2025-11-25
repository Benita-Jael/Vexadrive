using AutoMapper;
using VexaDriveAPI.DTO.ServiceRequest;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Mapping
{
    public class ServiceRequestMapperProfile : Profile
    {
        public ServiceRequestMapperProfile()
        {
            // Model → DTO
            CreateMap<ServiceRequest, ServiceRequestListDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ServiceRequest, ServiceRequestDetailsDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // DTO → Model
            CreateMap<ServiceRequestCreateDTO, ServiceRequest>();
            CreateMap<ServiceRequestUpdateDTO, ServiceRequest>();
        }
    }
}
