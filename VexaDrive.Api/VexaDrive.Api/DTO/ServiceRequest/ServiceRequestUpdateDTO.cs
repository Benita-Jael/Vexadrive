using System;
using System.ComponentModel.DataAnnotations;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestUpdateDTO
    {
        [Required]
        public int ServiceRequestId { get; set; }

        public ServiceStatus Status { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
