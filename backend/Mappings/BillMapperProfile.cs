using AutoMapper;
using VexaDriveAPI.DTO.Bill;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Mapping
{
    public class BillMapperProfile : Profile
    {
        public BillMapperProfile()
        {
            // Model → DTO
            CreateMap<Bill, BillDetailsDTO>();
        }
    }
}
