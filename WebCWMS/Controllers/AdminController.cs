using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Linq;
using System.Threading.Tasks;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeRecordRepository _repository; // Repository for time records
		private readonly IBlogRepository _blogRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly ICategoryRepository _categoryRepository;

        public AdminController(UserManager<ApplicationUser> userManager, ITimeRecordRepository repository,IBlogRepository blogRepository, IProjectRepository projectRepository, ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _repository = repository; // Initialize the repository
			_blogRepository = blogRepository;
			_categoryRepository = categoryRepository;	
			_projectRepository = projectRepository;
        }



		[Authorize(Roles = "Superadmin")]
		public async Task<IActionResult> UserList()
		{
			var users = await _userManager.Users.ToListAsync(); 
			ViewBag.TotalUsers = users.Count; 
			return View(users);
		}

		[Authorize(Roles = "Superadmin")]
		public async Task<IActionResult> ApproveUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId) as ApplicationUser;
			if (user != null && !user.IsApproved)
			{
				user.IsApproved = true;
				await _userManager.UpdateAsync(user);
			}
			return RedirectToAction("UserList");
		}

        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> SuperAdminDashboard()
        {
            var users = await _userManager.Users.ToListAsync();

            ViewBag.TotalProjects = await _projectRepository.GetTotalProjectsAsync();
            ViewBag.TotalBlog = await _blogRepository.GetTotalBlogAsync();
            ViewBag.TotalCategory = await _categoryRepository.GetTotalCategoryAsync();
            ViewBag.TotalApplicationUsers = users.Count;

            // Ensure that a valid model is passed to the view
            return View(users);  // users is passed as the model to the view
        }


        [Authorize(Roles ="User")]
        public async Task<IActionResult> UserDashboard()
        {
            var userEmail = User.Identity.Name; // Gets the email of the logged-in user

            // Fetch the time records for the logged-in user using their email
            var timeRecords = await _repository.GetUserTimeRecordsAsync(userEmail);

            var model = timeRecords.Select(record => new TimeRecordViewModel
            {
                TimeRecordId = record.TimeRecordId,
                ClockInTime = record.ClockInTime,
                ClockOutTime = record.ClockOutTime
            }).ToList();

            return View(model); // Pass the list of TimeRecordViewModel to the view
        }

        [Authorize]
		public async Task<IActionResult> RedirectToDashboard()
		{
			// Retrieve the ApplicationUser from the database
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				// Handle the case when the user is not found
				return RedirectToAction("Index", "Home");
			}

			if (await _userManager.IsInRoleAsync(user, "Superadmin"))
			{
				return RedirectToAction("SuperadminDashboard", "Admin");
			}
			else if (await _userManager.IsInRoleAsync(user, "User"))
			{
				return RedirectToAction("UserDashboard", "Admin");
			}
			else
			{
				return RedirectToAction("Index", "Home"); // Default redirect
			}
		}


		public async Task<IActionResult> SearchUser(string searchQuery)
		{
			try
			{
				// Search for users whose name starts with the search query first, and then those containing the query
				var users = string.IsNullOrEmpty(searchQuery)
					? await _userManager.Users
						.OrderBy(u => u.UserName) // If no searchQuery, return all users ordered alphabetically
						.ToListAsync()
					: await _userManager.Users
						.Where(u => u.UserName.StartsWith(searchQuery)) // Names starting with the searchQuery
						.Union(_userManager.Users.Where(u => u.UserName.Contains(searchQuery) && !u.UserName.StartsWith(searchQuery))) // Names containing the query but not starting with it
						.OrderBy(u => u.UserName) // Order the results alphabetically
						.ToListAsync();

				return PartialView("_UserListPartial", users); // Return the partial view with filtered users
			}
			catch (Exception ex)
			{
				// Log the exception or set a breakpoint here to examine 'ex'
				return StatusCode(500, "Server error: " + ex.Message);
			}
		}


		[Authorize(Roles = "Superadmin")] // Ensure only authorized roles can access this method
		[HttpPost]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var result = await _userManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return Json(new { success = true, message = "User has been deleted." });
				}
				else
				{
					return Json(new { success = false, message = "Error while deleting the user." });
				}
			}

			return Json(new { success = false, message = "User not found." });
		}

        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> ViewUserWorkingHours(string userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Fetch the time records for the user
            var timeRecords = await _repository.GetUserTimeRecordsAsync(user.Email);

            // Check if time records exist for the user
            if (timeRecords == null || !timeRecords.Any())
            {
                ViewBag.Message = "No time records found for this user.";
            }

            // Calculate the total working hours
            var totalWorkedHours = timeRecords
                .Where(tr => tr.ClockOutTime.HasValue)
                .Sum(tr => (tr.ClockOutTime.Value - tr.ClockInTime).TotalHours);

            // Create ViewModel with the necessary data
            var model = new UserWorkingHoursViewModel
            {
                User = user,
                TimeRecords = timeRecords.Select(tr => new TimeRecordViewModel
                {
                    TimeRecordId = tr.TimeRecordId,
                    ClockInTime = tr.ClockInTime,
                    ClockOutTime = tr.ClockOutTime
                }).ToList(),
                TotalWorkedHours = totalWorkedHours
            };

            return View(model);  // Pass the model to the view
        }


    }
}
