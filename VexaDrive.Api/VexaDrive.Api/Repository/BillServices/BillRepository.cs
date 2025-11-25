using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.Bill;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.BillServices
{
    public class BillRepository : IBillRepository
    {
        private readonly VexaDriveDbContext _context;
        private readonly IMapper _mapper;

        public BillRepository(VexaDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Bill?> UploadBillAsync(int serviceRequestId, string fileName, string contentType, string storagePath)
        {
            var request = await _context.ServiceRequests.FindAsync(serviceRequestId);
            if (request == null)
                throw new Exception("Invalid ServiceRequestId. Cannot upload bill.");

            var bill = new Bill
            {
                ServiceRequestId = serviceRequestId,
                FileName = fileName,
                ContentType = contentType,
                StoragePath = storagePath,
                UploadedAt = DateTime.UtcNow
            };

            await _context.Bills.AddAsync(bill);
            await _context.SaveChangesAsync();

            return bill;
        }

        public async Task<BillDetailsDTO?> GetBillByRequestIdAsync(int serviceRequestId)
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.ServiceRequestId == serviceRequestId);

            if (bill == null) return null;
            return _mapper.Map<BillDetailsDTO>(bill);
        }
    }
}
