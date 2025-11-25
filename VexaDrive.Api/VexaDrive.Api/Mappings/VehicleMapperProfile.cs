using AutoMapper;
using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Mapping
{
    public class VehicleMapperProfile : Profile
    {
        public VehicleMapperProfile()
        {
            // Mapping Model → List/Details DTO
            CreateMap<Vehicle, VehicleListDTO>();
            CreateMap<Vehicle, VehicleDetailsDTO>()
                .ForMember(dest => dest.OwnerFullName, opt =>
                    opt.MapFrom(src => src.Owner != null
                        ? $"{src.Owner.FirstName} {src.Owner.LastName}"
                        : null))
                .ForMember(dest => dest.OwnerContact, opt =>
                    opt.MapFrom(src => src.Owner != null ? src.Owner.ContactNumber : null))
                .ForMember(dest => dest.OwnerEmail, opt =>
                    opt.MapFrom(src => src.Owner != null ? src.Owner.Email : null));
 
            // Mapping DTO → Model
            CreateMap<VehicleCreateDTO, Vehicle>();
            CreateMap<VehicleUpdateDTO, Vehicle>();
        }
    }
}