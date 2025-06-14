using System.Collections.Specialized;

namespace MAS.API.Models.Entities
{
    public class DoctorSchedule
    {
        public string Id { get; set; }
        public int DoctorId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }

        public virtual Doctor Doctor { get; set; }

    }
}
