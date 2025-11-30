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
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle!.Model))
                .ForMember(dest => dest.VehicleNumberPlate, opt => opt.MapFrom(src => src.Vehicle!.NumberPlate))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.Vehicle!.Type))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ServiceDateOnly, opt => opt.MapFrom(src => src.ServiceDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedAtDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EstimatedDeliveryDateOnly, opt => opt.MapFrom(src => src.EstimatedDeliveryDate.HasValue ? src.EstimatedDeliveryDate.Value.ToString("yyyy-MM-dd") : null));

            CreateMap<ServiceRequest, ServiceRequestDetailsDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle!.Model))
                .ForMember(dest => dest.VehicleNumberPlate, opt => opt.MapFrom(src => src.Vehicle!.NumberPlate))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.Vehicle!.Type))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ServiceDateOnly, opt => opt.MapFrom(src => src.ServiceDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedAtDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.UpdatedAtDate, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EstimatedDeliveryDateOnly, opt => opt.MapFrom(src => src.EstimatedDeliveryDate.HasValue ? src.EstimatedDeliveryDate.Value.ToString("yyyy-MM-dd") : null));

            // DTO → Model
            CreateMap<ServiceRequestCreateDTO, ServiceRequest>();
            CreateMap<ServiceRequestUpdateDTO, ServiceRequest>();
        }
    }
}
