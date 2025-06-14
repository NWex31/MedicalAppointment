using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MAS.API.Data;

namespace MAS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;


    public AdminController (AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("statistics")]

        public async Task<IActionResult> GetStatistics ()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TodayAppointmensts = await _context.Appointments.CountAsync(
                    a => a.AppointmentDateTime.Date == DateTime.Today),
                CanceledAppointments = await _context.Appointments.CountAsync(
                    a => a.Status == "Canceled")
            };

            return Ok(stats);

        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers ()
        {

            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.CreatedAt,
                    u.IsEmailVerified
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("user{userId}/toggle-status")]
        public async Task<IActionResult> ToggleStatus (int userId)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Użytkownik nie znaleziony");
            
            user.IsEmailVerified = !user.IsEmailVerified;
            await _context.SaveChangesAsync();
            return Ok(new {message = "Status zmieniony", isActive = user.IsEmailVerified });

        }





    }
}
