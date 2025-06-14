using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MAS.API.Data;

namespace MAS.API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]


    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SearchController(AppDbContext context) {

            _context = context;
        }
        [HttpGet("doctors")]
        public async Task<IActionResult> SearchDoctorsSafe(string specialization)
        {
            
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.Specialization.Contains(specialization))
                .Select(d => new
                {
                    d.Id,
                    Name = d.User.FirstName + " " + d.User.LastName,
                    d.Specialization
                })
                .ToListAsync();

            return Ok(doctors);

        }
        [HttpGet("appointments-wuth-notes")]
        public async Task<IActionResult> SearchAppointmentsWithNotes(string searchTerm)
        {
            var appointments = await _context.Appointments
                .Where(a => a.Notes.Contains(searchTerm))
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDateTime,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName
                })
                .ToListAsync();


            return Ok(appointments);

        }




    }
}
