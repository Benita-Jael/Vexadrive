using System;
using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }

        [Required]
        public int ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string ContentType { get; set; } // application/pdf

        [Required]
        public string StoragePath { get; set; } // e.g., /Storage/Bills/{guid}.pdf

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
