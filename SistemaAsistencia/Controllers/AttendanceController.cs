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

            // Validar que el tipo de registro sea válido
            if (string.IsNullOrEmpty(recordType) || (recordType != "Entrada" && recordType != "Salida"))
            {
                TempData["Error"] = "Tipo de registro no válido.";
                return RedirectToAction("Index");
            }

            // Buscar el último registro del usuario para el día de hoy
            var lastRecordToday = await _context.AttendanceRecords
                .Where(r => r.UserId == userId.Value && r.Timestamp.Date == DateTime.Today)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            // Validaciones de negocio
            if (recordType == "Entrada")
            {
                // No puede marcar entrada si ya marcó entrada hoy
                if (lastRecordToday != null && lastRecordToday.RecordType == "Entrada")
                {
                    TempData["Error"] = "Ya ha marcado su entrada hoy. Debe marcar salida primero.";
                    return RedirectToAction("Index");
                }
            }
            else if (recordType == "Salida")
            {
                // No puede marcar salida si no ha marcado entrada o ya marcó salida
                if (lastRecordToday == null || lastRecordToday.RecordType == "Salida")
                {
                    TempData["Error"] = "Debe marcar su entrada antes de marcar salida.";
                    return RedirectToAction("Index");
                }
            }

            // Crear el nuevo registro
            var record = new AttendanceRecord
            {
                UserId = userId.Value,
                Timestamp = DateTime.Now,
                RecordType = recordType
            };

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            // Mensaje de confirmación más detallado
            var timeString = record.Timestamp.ToString("HH:mm:ss");
            var dateString = record.Timestamp.ToString("dd/MM/yyyy");
            TempData["Message"] = $"✅ {recordType} registrada exitosamente el {dateString} a las {timeString}";

            return RedirectToAction("Index");
        }
    }
}