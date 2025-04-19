using WebCWMS.Models;

namespace WebCWMS.Repository.Interface
{
    public interface ITimeRecordRepository
    {
        Task<int> ClockInAsync(string userId);
        Task<bool> ClockOutAsync(int timeRecordId);
        Task<IEnumerable<TimeRecord>> GetUserTimeRecordsAsync(string userEmail);
        Task<IEnumerable<TimeRecord>> GetAllTimeRecordsAsync();
        Task<bool> IsUserCurrentlyClockedIn(string userId);
        Task<TimeRecord> GetTimeRecordByIdAsync(int recordId);
        Task<int?> GetOngoingSessionId(string userId);
    }
}
