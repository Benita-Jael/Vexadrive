using AutoMapper;

using VexaDriveAPI.DTO.OwnerDTO;

using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Mapping

{

    public class OwnerMapperProfile : Profile

    {

        public OwnerMapperProfile()

        {

            // Mapping DTO → Model

            CreateMap<OwnerCreateDTO, Owner>();

            CreateMap<OwnerUpdateDTO, Owner>();
 
            // Mapping Model → DTO

            CreateMap<Owner, OwnerDetailsDTO>();
 
            CreateMap<Owner, OwnerListDTO>()

                .ForMember(dest => dest.FullName,

                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        }

    }

}

 