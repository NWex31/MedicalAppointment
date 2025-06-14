namespace MAS.API.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public int PatientId { get; set; }
        public string Status { get; set; }
        public string? Notes   {get; set;}

        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual User Patient { get; set; }
    }
}
