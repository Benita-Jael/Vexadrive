using System;

namespace VexaDriveAPI.DTO.Bill
{
    public class BillDetailsDTO
    {
        public int BillId { get; set; }
        public int ServiceRequestId { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }   // e.g., application/pdf
        public string StoragePath { get; set; }   // relative path or URL

        public DateTime UploadedAt { get; set; }
    }
}
