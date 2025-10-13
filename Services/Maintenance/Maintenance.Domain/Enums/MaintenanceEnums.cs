using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Enums
{
    public enum MaintenanceEnums
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum MaintenanceServiceTypeEnum
    {
        PeriodicMaintenance = 0,    // Bảo dưỡng định kỳ
        BatteryCheck = 1,            // Kiểm tra pin
        BrakeService = 2,            // Bảo dưỡng phanh
        TireReplacement = 3,         // Thay lốp
        SuspensionRepair = 4,        // Sửa chữa hệ thống treo
        ElectricalRepair = 5,        // Sửa chữa hệ thống điện
        ChargingSystemRepair = 6,    // Sửa chữa hệ thống sạc
        GeneralRepair = 7,           // Sửa chữa chung
        Inspection = 8,              // Kiểm tra tổng quát
        Emergency = 9                // Khẩn cấp
    }

}
