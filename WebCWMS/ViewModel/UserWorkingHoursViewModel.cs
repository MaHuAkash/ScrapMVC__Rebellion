using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
    public class UserWorkingHoursViewModel
    {
        public ApplicationUser User { get; set; }
        public List<TimeRecordViewModel> TimeRecords { get; set; }
        public double TotalWorkedHours { get; set; }
    }
}
