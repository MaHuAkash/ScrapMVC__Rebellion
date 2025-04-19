using System;
using System.ComponentModel.DataAnnotations;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
    public class TimeRecordViewModel
    {
        public int TimeRecordId { get; set; }
        public string UserId { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }

        // Convert ClockInTime to Vancouver time zone
        public DateTime ClockInTimeVancouver
        {
            get
            {
                TimeZoneInfo vancouverZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
                return TimeZoneInfo.ConvertTimeFromUtc(ClockInTime, vancouverZone);
            }
        }

        // Convert ClockOutTime to Vancouver time zone
        public DateTime? ClockOutTimeVancouver
        {
            get
            {
                if (ClockOutTime.HasValue)
                {
                    TimeZoneInfo vancouverZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
                    return TimeZoneInfo.ConvertTimeFromUtc(ClockOutTime.Value, vancouverZone);
                }
                return null;
            }
        }

        public string FormattedClockOutTime => ClockOutTimeVancouver.HasValue ? ClockOutTimeVancouver.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Not Available";
        public TimeSpan TimeSinceClockIn => DateTime.UtcNow - ClockInTime; // Assuming ClockInTime is in UTC

        public List<TimeRecord> TimeRecords { get; set; }
        public object TimeRecord { get; internal set; }
    }
} 
