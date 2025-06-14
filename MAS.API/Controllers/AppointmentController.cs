using MAS.API.Data;
using MAS.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAS.API.Controllers
{

    [ApiController]
    [Route("controller")]
    public class AppointmentController : ControllerBase

    {
        private readonly AppDbContext _context;
     public AppointmentController(AppDbContext context)
     {
            _context = context;
     }




        [HttpGet("avaliable-slots")]

        public async Task<IActionResult> GetAvaliableSlots (int doctorId, DateTime date)
        {

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                && a.AppointmentDateTime.Date == date.Date
                && a.Status != "Canceled")
                .Select(a => a.AppointmentDateTime)
                .ToListAsync();


            var avaliableSlots = new List<DateTime>();
            var startTime = date.Date.AddHours(8);
            var endTime = date.Date.AddHours(16);

            while (startTime < endTime)
            {
                if (!bookedSlots.Contains(startTime))
                {
                    
                avaliableSlots.Add(startTime);
                }
                startTime = startTime.AddMinutes(30);


            }

            return Ok(avaliableSlots);


        }

        [HttpPost("book")]

        public async Task<IActionResult> BookAppointment(BookAppointmentDdto dto)
        {

            var ExistingAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == dto.DoctorId
                && a.AppointmentDateTime == dto.AppointmentDateTime
                && a.Status != "Canceled");


            if (ExistingAppointment)
            {
                return BadRequest("Ten termin jest już zajęty");
            }


            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId, 
                AppointmentDateTime = dto.AppointmentDateTime,
                Status = "Scheduled",
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };


            return Ok(new
            {
                message = "Wizyta została zarejestrowana",
                appointmentId = appointment.Id
            });
         
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment (int id)
        {

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) 
                return NotFound("Wzyta nie została odnaleziona");

            if (appointment.AppointmentDateTime < DateTime.Now.AddHours(24))
                return BadRequest("Przykro nam ale nie można anulować wizyty do której zostało mniej niż 24 godziny");

            appointment.Status = "Canceled";

            return Ok(new { message = "Wizyta została anulowana" });

        }

        [HttpGet("my-appointments")]

        public async Task <IActionResult> MyAppointments (int patientId)
        {

            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDateTime)
                .Select(a => new
                {
                    a.Id,
                    a.PatientId,
                    a.AppointmentDateTime,
                    a.Status,
                    a.Notes,
                    DoctorName = a.Doctor.User.FirstName + " " + a.Doctor.User.LastName,
                    Specialization = a.Doctor.Specialization
                })

                .ToListAsync();
            return Ok(appointments);
        }


        public class BookAppointmentDdto
        {
            public int DoctorId { get; set; }
            public int PatientId { get; set; }
            public DateTime AppointmentDateTime { get; set; }
            
            public string? Notes { get; set; }



        }
        






    }
}
