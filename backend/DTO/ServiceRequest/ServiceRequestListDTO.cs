using System;

namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestListDTO
    {
        public int ServiceRequestId { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleNumberPlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        // Date-only formatted strings for UI convenience (YYYY-MM-DD)
        public string ServiceDateOnly { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtDate { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? EstimatedDeliveryDateOnly { get; set; }
        public string ProblemDescription { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = string.Empty;
    }
}
