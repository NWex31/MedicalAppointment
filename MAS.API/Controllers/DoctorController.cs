using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MAS.API.Data;
using MAS.API.Models.Entities;


namespace MAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase

    {

        private readonly AppDbContext _context;


        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list")]

        public async Task<IActionResult> GetDoctorList()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new
                {
                    d.Id,
                    Name = d.User.FirstName + " " + d.User.LastName,
                    d.Specialization,
                    Email = d.User.Email,


                })
                .ToListAsync();
            return Ok(doctors);

        }

        [HttpGet("{doctorId}/schedule")]

        public async Task<IActionResult> GetDoctorSchedule(int doctorId, DateTime date)
        {
            var schedule = await _context.Appointments
                 .Include(a => a.Patient)
                 .Where(a => a.DoctorId == doctorId
                 && a.AppointmentDateTime == date.Date)
                 .OrderBy(a => a.AppointmentDateTime)
                 .Select(a => new
                 {
                     a.Id,
                     Time = a.AppointmentDateTime.ToString("HH:mm"),
                     PacientName = a.Patient.FirstName + " " + a.Patient.LastName,
                     a.Status,
                     a.Notes


                 })
                 .ToListAsync();
            return Ok(schedule);



        }

        [HttpPut("{doctorId}/confirm-appointment/{appointmentId}")]


        public async Task<IActionResult> ConfirmAppointment(int doctorId, int appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorId == doctorId
                && a.Id == appointmentId);

            if (appointment == null)
                return NotFound("Nie znaleziono wizyty");


            appointment.Status = "Confirmed";
            await _context.SaveChangesAsync();






            return Ok(new { message = "Wizyta potwierdzona" });
        }

        [HttpPost("{doctorId}/set-avaliability")]
        public async Task<IActionResult> SetDoctorsAvaliability(int doctorId, SetAvaliabilityDto dto)
        {
            var schedule = new DoctorSchedule
            {
                DoctorId = doctorId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SlotDurationMinutes = 30
            };


            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dostępność ustawiona" });


        }





        public class SetAvaliabilityDto
        {

            public DayOfWeek DayOfWeek { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }






        }


    }
}

