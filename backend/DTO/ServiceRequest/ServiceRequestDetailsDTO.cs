using System;
using System.Collections.Generic;
using VexaDriveAPI.DTO.Bill;
using VexaDriveAPI.DTO.Notification;


namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestDetailsDTO
    {
        public int ServiceRequestId { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
    public string VehicleNumberPlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
        public string ProblemDescription { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        // Date-only formatted strings for UI convenience (YYYY-MM-DD)
        public string ServiceDateOnly { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtDate { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedAtDate { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? EstimatedDeliveryDateOnly { get; set; }

        public BillDetailsDTO Bill { get; set; } = null!;
        public IEnumerable<NotificationListDTO> Notifications { get; set; } = Array.Empty<NotificationListDTO>();
    }
}
