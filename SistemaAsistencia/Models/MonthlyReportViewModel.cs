using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class MonthlyReportViewModel
    {
        [Display(Name = "Mes")]
        public int Month { get; set; }

        [Display(Name = "Año")]
        public int Year { get; set; }

        public List<string> DayHeaders { get; set; } = new List<string>();
        public List<EmployeeMonthlyStatus> EmployeeStatuses { get; set; } = new List<EmployeeMonthlyStatus>();
    }

    // Representa la fila de un solo empleado en el reporte
    public class EmployeeMonthlyStatus
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Dictionary<int, DailyStatusViewModel> DailyStatuses { get; set; } = new Dictionary<int, DailyStatusViewModel>();
    }

 
    public class DailyStatusViewModel
    {
        public AttendanceStatus Status { get; set; }
        public TimeSpan? ClockInTime { get; set; }
        public TimeSpan? ClockOutTime { get; set; }
    }


    public enum AttendanceStatus
    {
        Attended,
        Absent,
        LateArrival,
        EarlyDeparture,
        Incomplete 
    }
}