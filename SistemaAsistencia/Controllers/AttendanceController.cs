using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAsistencia.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Buscar el último registro del usuario para el día de hoy
            var lastRecordToday = await _context.AttendanceRecords
                .Where(r => r.UserId == userId.Value && r.Timestamp.Date == DateTime.Today)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            string attendanceStatus;
            if (lastRecordToday == null)
            {
                // No ha marcado nada hoy
                attendanceStatus = "CanClockIn";
            }
            else if (lastRecordToday.RecordType == "Entrada")
            {
                // Su último registro fue una entrada, así que puede marcar salida
                attendanceStatus = "CanClockOut";
            }
            else
            {
                // Ya marcó su salida por hoy
                attendanceStatus = "CompletedDay";
            }

            ViewBag.AttendanceStatus = attendanceStatus;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(string recordType)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Aquí se añade una validación extra en el servidor para mayor seguridad
            var lastRecordToday = await _context.AttendanceRecords
                .Where(r => r.UserId == userId.Value && r.Timestamp.Date == DateTime.Today)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            // Prevenir marcar entrada si ya lo hizo, o salida si no ha entrado
            if ((recordType == "Entrada" && lastRecordToday != null) ||
                (recordType == "Salida" && (lastRecordToday == null || lastRecordToday.RecordType == "Salida")))
            {
                TempData["Error"] = "Acción no válida.";
                return RedirectToAction("Index");
            }

            var record = new AttendanceRecord
            {
                UserId = userId.Value,
                Timestamp = DateTime.Now,
                RecordType = recordType
            };
            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Se ha registrado su '{recordType}' a las {record.Timestamp:HH:mm:ss}";
            return RedirectToAction("Index");
        }
    }
}