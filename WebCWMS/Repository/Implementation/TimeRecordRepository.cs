using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;

namespace WebCWMS.Repository.Implementation
{
    public class TimeRecordRepository : ITimeRecordRepository
    {
        private readonly AppDbContext _context;

        public TimeRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ClockInAsync(string userId)
        {
            var record = new TimeRecord { UserId = userId, ClockInTime = DateTime.UtcNow };
            _context.TimeRecords.Add(record);
            await _context.SaveChangesAsync();
            return record.TimeRecordId; 
        }

        public async Task<bool> ClockOutAsync(int timeRecordId)
        {
            var record = await _context.TimeRecords.FindAsync(timeRecordId);
            if (record == null || record.ClockOutTime.HasValue)
            {
                return false;
            }

            record.ClockOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TimeRecord>> GetUserTimeRecordsAsync(string userEmail)
        {
            // Assuming you're using Entity Framework or similar ORM
            return await _context.TimeRecords
                                 .Where(record => record.UserId == userEmail)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<TimeRecord>> GetAllTimeRecordsAsync()
        {
            return await _context.TimeRecords.ToListAsync();
        }

        public async Task<bool> IsUserCurrentlyClockedIn(string userId)
        {
            return await _context.TimeRecords.AnyAsync(tr => tr.UserId == userId && !tr.ClockOutTime.HasValue);
        }
        public async Task<TimeRecord> GetTimeRecordByIdAsync(int recordId)
        {
            return await _context.TimeRecords.FirstOrDefaultAsync(tr => tr.TimeRecordId == recordId);
        }

        public async Task<int?> GetOngoingSessionId(string userId)
        {
            // Retrieve the ongoing session ID for the user based on certain criteria
            // For example, you can retrieve the session with a null ClockOutTime, indicating it's ongoing
            var ongoingSession = await _context.TimeRecords
                .Where(tr => tr.UserId == userId && tr.ClockOutTime == null)
                .FirstOrDefaultAsync();

            // If an ongoing session is found, return its TimeRecordId; otherwise, return null
            return ongoingSession?.TimeRecordId;
        }

    }
}
