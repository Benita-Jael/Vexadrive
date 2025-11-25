using System;
using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.DTO.ServiceRequest
{
    public class ServiceRequestCreateDTO
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public string ProblemDescription { get; set; } = string.Empty;

        [Required]
        public string ServiceAddress { get; set; } = string.Empty;

        [Required]
        public DateTime ServiceDate { get; set; }
    }
}
