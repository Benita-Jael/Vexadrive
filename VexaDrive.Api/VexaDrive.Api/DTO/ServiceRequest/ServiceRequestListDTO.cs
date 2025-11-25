using System;

namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestListDTO
    {
        public int ServiceRequestId { get; set; }
        public string VehicleModel { get; set; }
        public string Status { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
