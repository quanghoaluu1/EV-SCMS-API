using Maintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.DTOs
{
    public static class MaintenanceDTOs
    {
        public class MaintenanceInventoryItemDto
        {
            public Guid InventoryId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitCost { get; set; }
            public decimal TotalCost { get; set; }
        }

        public class CreateMaintenanceDto
        {
            [Required(ErrorMessage = "Vehicle ID is required")]
            public Guid VehicleId { get; set; }

            [Required(ErrorMessage = "Customer ID is required")]
            public Guid CustomerId { get; set; }

            [Required(ErrorMessage = "Booking ID is required")]
            public Guid BookingId { get; set; }

            [Required(ErrorMessage = "Staff ID is required")]
            public Guid StaffId { get; set; }

            [Required(ErrorMessage = "Service date is required")]
            public DateTime ServiceDate { get; set; }

            [Required(ErrorMessage = "Odometer reading is required")]
            [Range(0, int.MaxValue, ErrorMessage = "Odometer must be positive")]
            public int Odometer { get; set; }

            [Required(ErrorMessage = "Service type is required")]
            public MaintenanceServiceTypeEnum ServiceType { get; set; }

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Cost is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
            public decimal Cost { get; set; }

            public List<MaintenanceInventoryItemDto> InventoryItems { get; set; } = new List<MaintenanceInventoryItemDto>();

        }

        public class UpdateMaintenanceDto
        {
            public Guid? StaffId { get; set; }
            public DateTime? ServiceDate { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Odometer must be positive")]
            public int? Odometer { get; set; }

            public MaintenanceServiceTypeEnum? ServiceType { get; set; }

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
            public decimal? Cost { get; set; }

            public MaintenanceEnums? Status { get; set; }

            public List<MaintenanceInventoryItemDto>? InventoryItems { get; set; }

        }

        public class MaintenanceResponseDto
        {
            public Guid MaintenanceId { get; set; }
            public Guid VehicleId { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BookingId { get; set; }
            public Guid StaffId { get; set; }
            public DateTime ServiceDate { get; set; }
            public int Odometer { get; set; }
            public MaintenanceServiceTypeEnum ServiceType { get; set; }
            public string ServiceTypeName { get; set; } = string.Empty; // For display
            public string? Description { get; set; }
            public decimal Cost { get; set; }
            public MaintenanceEnums Status { get; set; }
            public string StatusName { get; set; } = string.Empty; // For display
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }

            public List<MaintenanceInventoryItemDto> InventoryItems { get; set; } = new List<MaintenanceInventoryItemDto>();

        }

        public class MaintenanceListDto
        {
            public Guid MaintenanceId { get; set; }
            public Guid VehicleId { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BookingId { get; set; }
            public DateTime ServiceDate { get; set; }
            public MaintenanceServiceTypeEnum ServiceType { get; set; }
            public decimal Cost { get; set; }
            public MaintenanceEnums Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }


        public class UpdateStatusDto
        {
            [Required(ErrorMessage = "Status is required")]
            public MaintenanceEnums Status { get; set; }
        }

        public class AssignStaffDto
        {
            [Required(ErrorMessage = "Staff ID is required")]
            public Guid StaffId { get; set; }
        }

        public class MaintenanceQueryDto
        {
            public Guid? CustomerId { get; set; }
            public Guid? VehicleId { get; set; }
            public Guid? BookingId { get; set; }
            public Guid? StaffId { get; set; }
            public MaintenanceEnums? Status { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }

            [Range(1, int.MaxValue)]
            public int PageNumber { get; set; } = 1;

            [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
            public int PageSize { get; set; } = 10;
        }

    }
}
