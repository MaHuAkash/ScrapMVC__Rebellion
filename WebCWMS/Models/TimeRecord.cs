using System.ComponentModel.DataAnnotations;

namespace WebCWMS.Models
{
    public class TimeRecord
    {
        [Key()]
        public int TimeRecordId { get; set; }
        public string UserId { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; } // Nullable for ongoing work periods
    }
}
