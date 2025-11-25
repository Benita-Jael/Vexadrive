using System;
using System.Collections.Generic;
using VexaDriveAPI.DTO.Bill;
using VexaDriveAPI.DTO.Notification;


namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestDetailsDTO
    {
        public int ServiceRequestId { get; set; }
        public string CustomerUserId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; }
        public string ProblemDescription { get; set; }
        public string ServiceAddress { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }

        public BillDetailsDTO Bill { get; set; }
        public IEnumerable<NotificationListDTO> Notifications { get; set; }
    }
}
