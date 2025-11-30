using System;

namespace VexaDriveAPI.DTO.Bill
{
    public class BillDetailsDTO
    {
        public int BillId { get; set; }
        public int ServiceRequestId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;   // e.g., application/pdf
        public string StoragePath { get; set; } = string.Empty;   // relative path or URL

        public DateTime UploadedAt { get; set; }
    }
}
