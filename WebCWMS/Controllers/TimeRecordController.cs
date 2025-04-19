using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Controllers
{
    public class TimeRecordController : Controller
    {
        private readonly ITimeRecordRepository _repository;

        public TimeRecordController(ITimeRecordRepository repository, AppDbContext context)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> ClockIn()
        {
            var userId = User.Identity.Name; // Get the user ID

            // Check if the user is already clocked in
            if (await _repository.IsUserCurrentlyClockedIn(userId))
            {
                // User is already clocked in
                TempData["ClockedInMessage"] = "You are already clocked in.";

                // Retrieve the ID of the ongoing session, if available
                int? ongoingSessionId = await _repository.GetOngoingSessionId(userId);

                // Redirect to the "CurrentOngoingSession" action with the ongoing session ID
                return RedirectToAction("CurrentSession", new { recordId = ongoingSessionId });
            }

            // If the user is not already clocked in, proceed with creating a new time record
            int recordId = await _repository.ClockInAsync(userId);

            // Redirect to an action that displays the current clock-in record
            return RedirectToAction("CurrentSession", new { recordId = recordId });
        }

        [HttpPost]
        public async Task<IActionResult> ClockOut(int recordId)
        {
            var userId = User.Identity.Name; // Get the user ID

            // Validate if the record exists and belongs to the current user
            var timeRecord = await _repository.GetTimeRecordByIdAsync(recordId);
            if (timeRecord == null)
            {
                return BadRequest("Time record not found.");
            }

            if (timeRecord.UserId != userId)
            {
                return Unauthorized("You do not have permission to clock out this record.");
            }

            if (timeRecord.ClockOutTime.HasValue)
            {
                return BadRequest("This session has already been clocked out.");
            }

            // Proceed to clock out
            bool success = await _repository.ClockOutAsync(recordId);
            if (success)
            {
                return RedirectToAction("UserDashboard", "Admin");
            }
            else
            {
                return BadRequest("Clock out operation failed.");
            }
        }


        public async Task<IActionResult> CurrentSession(int recordId)
        {
            var currentRecord = await _repository.GetTimeRecordByIdAsync(recordId);
            if (currentRecord == null)
            {
                // Handle the case when the record is not found
                return NotFound("Clock-in record not found.");
            }

            var viewModel = new TimeRecordViewModel
            {
                TimeRecordId = currentRecord.TimeRecordId,
                UserId = currentRecord.UserId,
                ClockInTime = currentRecord.ClockInTime,
                ClockOutTime = currentRecord.ClockOutTime
            };

            return View(viewModel); // Pass the TimeRecordViewModel to the view
        }




    }
}
