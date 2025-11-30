using VexaDriveAPI.DTO.Bill;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.BillServices
{
    public interface IBillRepository
    {
        Task<Bill?> UploadBillAsync(int serviceRequestId, string fileName, string contentType, string storagePath);
        Task<BillDetailsDTO?> GetBillByRequestIdAsync(int serviceRequestId);
    }
}
