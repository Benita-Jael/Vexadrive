using AutoMapper;
using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Mapping
{
    public class VehicleMapperProfile : Profile
    {
        public VehicleMapperProfile()
        {
            // Model → DTO
            CreateMap<Vehicle, VehicleListDTO>();
            CreateMap<Vehicle, VehicleDetailsDTO>();

            // DTO → Model
            CreateMap<VehicleCreateDTO, Vehicle>();
            CreateMap<VehicleUpdateDTO, Vehicle>();
        }
    }
}
