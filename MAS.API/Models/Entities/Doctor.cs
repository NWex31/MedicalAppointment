namespace MAS.API.Models.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Specialization { get; set; }
        public string? LicenseNumber { get; set; }

        public virtual User User { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
