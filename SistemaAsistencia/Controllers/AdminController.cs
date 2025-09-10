using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var usersQuery = from u in _context.Users
                            select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.Name.Contains(searchString) || u.Email.Contains(searchString));
            }

            // Contar el total de registros
            var totalCount = await usersQuery.CountAsync();

            // Aplicar paginación
            var users = await usersQuery
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(pagedResult);
        }

     

        // GET: Admin/LateArrivalsReport
        public async Task<IActionResult> LateArrivalsReport(int pageNumber = 1, int pageSize = 20)
        {
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var lateTime = new TimeSpan(9, 30, 0);
            var lateArrivalsQuery = _context.AttendanceRecords
                .Include(r => r.User) 
                .Where(r => r.RecordType == "Entrada" && r.Timestamp.TimeOfDay > lateTime);

            // Contar el total de registros
            var totalCount = await lateArrivalsQuery.CountAsync();

            // Aplicar paginación
            var lateArrivals = await lateArrivalsQuery
                .OrderByDescending(r => r.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<AttendanceRecord>
            {
                Items = lateArrivals,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(pagedResult);
        }

        // GET: Admin/EarlyDeparturesReport
        public async Task<IActionResult> EarlyDeparturesReport(int pageNumber = 1, int pageSize = 20)
        {
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var earlyTime = new TimeSpan(17, 30, 0);
            var earlyDeparturesQuery = _context.AttendanceRecords
                .Include(r => r.User) 
                .Where(r => r.RecordType == "Salida" && r.Timestamp.TimeOfDay < earlyTime);

            // Contar el total de registros
            var totalCount = await earlyDeparturesQuery.CountAsync();

            // Aplicar paginación
            var earlyDepartures = await earlyDeparturesQuery
                .OrderByDescending(r => r.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<AttendanceRecord>
            {
                Items = earlyDepartures,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(pagedResult);
        }

        // GET: Admin/AbsenceReport
        public async Task<IActionResult> AbsenceReport(DateTime? reportDate, int pageNumber = 1, int pageSize = 20)
        {
            // Si no se especifica una fecha, usamos el día de hoy
            var date = reportDate ?? DateTime.Today;
            ViewData["ReportDate"] = date.ToString("yyyy-MM-dd");
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            // Obtenemos los IDs de los usuarios que SÍ registraron asistencia en esa fecha
            var usersWithAttendance = await _context.AttendanceRecords
                .Where(r => r.Timestamp.Date == date.Date)
                .Select(r => r.UserId)
                .Distinct()
                .ToListAsync();

            // Buscamos a todos los usuarios que NO están en la lista anterior
            var absentUsersQuery = _context.Users
                .Where(u => !u.IsAdmin && !usersWithAttendance.Contains(u.Id));

            // Contar el total de registros
            var totalCount = await absentUsersQuery.CountAsync();

            // Aplicar paginación
            var absentUsers = await absentUsersQuery
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<User>
            {
                Items = absentUsers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(pagedResult);
        }

      
        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        // GET: Admin/Create
        public IActionResult Create() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,Name,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FindAsync(id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,Name,IsAdmin")] User user)
        {
            if (id != user.Id) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) { _context.Users.Remove(user); }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(int id) { return _context.Users.Any(e => e.Id == id); }
       

        // GET: Admin/ManualRecord
        public async Task<IActionResult> ManualRecord()
        {
            var viewModel = new ManualRecordViewModel
            {
             
                Users = new SelectList(await _context.Users.Where(u => !u.IsAdmin).ToListAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: Admin/ManualRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualRecord(ManualRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                
                viewModel.Users = new SelectList(await _context.Users.Where(u => !u.IsAdmin).ToListAsync(), "Id", "Name", viewModel.UserId);
                return View(viewModel);
            }

            var existingRecords = await _context.AttendanceRecords
                .Where(r => r.UserId == viewModel.UserId && r.Timestamp.Date == viewModel.Date.Date)
                .ToListAsync();

            if (existingRecords.Any())
            {
                _context.AttendanceRecords.RemoveRange(existingRecords);
            }

          
            if (viewModel.ClockInTime.HasValue)
            {
                var clockInDateTime = viewModel.Date.Date.Add(viewModel.ClockInTime.Value);
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    UserId = viewModel.UserId,
                    Timestamp = clockInDateTime,
                    RecordType = "Entrada"
                });
            }
          
            if (viewModel.ClockOutTime.HasValue)
            {
                var clockOutDateTime = viewModel.Date.Date.Add(viewModel.ClockOutTime.Value);
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    UserId = viewModel.UserId,
                    Timestamp = clockOutDateTime,
                    RecordType = "Salida"
                });
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "El registro de asistencia se ha actualizado correctamente.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MonthlyReport(int? month, int? year)
        {
            // Usar el mes y año actual si no se especifican
            int reportMonth = month ?? DateTime.Now.Month;
            int reportYear = year ?? DateTime.Now.Year;

            var startDate = new DateTime(reportYear, reportMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // 1. Obtener todos los datos necesarios en pocas consultas a la BD
            var users = await _context.Users.Where(u => !u.IsAdmin).ToListAsync();
            var recordsForMonth = await _context.AttendanceRecords
                .Where(r => r.Timestamp.Date >= startDate.Date && r.Timestamp.Date <= endDate.Date)
                .ToListAsync();

            var viewModel = new MonthlyReportViewModel
            {
                Month = reportMonth,
                Year = reportYear
            };

            
            for (int i = 1; i <= endDate.Day; i++)
            {
                viewModel.DayHeaders.Add(i.ToString());
            }

            // 3. Procesar los datos para cada empleado
            foreach (var user in users)
            {
                var employeeStatus = new EmployeeMonthlyStatus
                {
                    UserId = user.Id,
                    UserName = user.Name
                };

                for (int day = 1; day <= endDate.Day; day++)
                {
                    var currentDate = new DateTime(reportYear, reportMonth, day);
                    var recordsForDay = recordsForMonth
                        .Where(r => r.UserId == user.Id && r.Timestamp.Date == currentDate.Date)
                        .ToList();

                    var clockInRecord = recordsForDay.FirstOrDefault(r => r.RecordType == "Entrada");
                    var clockOutRecord = recordsForDay.FirstOrDefault(r => r.RecordType == "Salida");

                    var dailyStatus = new DailyStatusViewModel();

                    if (clockInRecord == null && clockOutRecord == null)
                    {
                        dailyStatus.Status = AttendanceStatus.Absent;
                    }
                    else if (clockInRecord == null || clockOutRecord == null)
                    {
                        dailyStatus.Status = AttendanceStatus.Incomplete;
                    }
                    else
                    {
                        dailyStatus.ClockInTime = clockInRecord.Timestamp.TimeOfDay;
                        dailyStatus.ClockOutTime = clockOutRecord.Timestamp.TimeOfDay;

                        bool isLate = dailyStatus.ClockInTime > new TimeSpan(9, 30, 0);
                        bool isEarly = dailyStatus.ClockOutTime < new TimeSpan(17, 30, 0);

                        if (isLate) dailyStatus.Status = AttendanceStatus.LateArrival;
                        else if (isEarly) dailyStatus.Status = AttendanceStatus.EarlyDeparture;
                        else dailyStatus.Status = AttendanceStatus.Attended;
                    }
                    employeeStatus.DailyStatuses.Add(day, dailyStatus);
                }
                viewModel.EmployeeStatuses.Add(employeeStatus);
            }

            return View(viewModel);
        }
    }


}