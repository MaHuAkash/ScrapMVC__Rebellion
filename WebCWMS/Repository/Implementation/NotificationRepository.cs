//using Microsoft.AspNetCore.Identity;
//using WebCWMS.Models;
//using WebCWMS.Repository.Interface;

//namespace WebCWMS.Repository.Implementation
//{
//    public class NotificationRepository : INotificationRepository
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public NotificationRepository(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task NotifySuperAdminAsync(string message)
//        {
//            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
//            foreach (var superAdmin in superAdmins)
//            {
//            }
//        }
//    }
//}
