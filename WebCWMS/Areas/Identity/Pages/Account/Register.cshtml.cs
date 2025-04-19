using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebCWMS.Models; // Ensure this is the correct namespace for ApplicationUser

namespace WebCWMS.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		private readonly RoleManager<IdentityRole> _roleManager; // Add this

		public RegisterModel(
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender,
			RoleManager<IdentityRole> roleManager) // Add this parameter
		{
			_userManager = userManager;
			_userStore = userStore;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			_roleManager = roleManager; // Assign the role manager
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }
            public IFormFile ProfilePicture { get; set; }
        }

		public async Task<IActionResult> OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			return Page();
		}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, IsApproved = false };

                if (Input.ProfilePicture != null)
                {
                    using var memoryStream = new MemoryStream();
                    await Input.ProfilePicture.CopyToAsync(memoryStream);
                    user.ProfilePicture = memoryStream.ToArray();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User registered but pending approval.");

                    // Check if the 'User' role exists, create if not
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    // Assign the new user to the 'User' role
                    await _userManager.AddToRoleAsync(user, "User");

                    // Send notification to Superadmins for user approval
                    var superAdmins = await _userManager.GetUsersInRoleAsync("Superadmin");
                    foreach (var admin in superAdmins)
                    {
                        var approvalLink = Url.Action("ApproveUser", "Admin", new { userId = user.Id }, protocol: Request.Scheme);
                        await _emailSender.SendEmailAsync(
                            admin.Email,
                            "New User Approval Needed",
                            $"A new user has registered and requires approval. Email: {user.Email}. Click here to approve: <a href='{approvalLink}'>Approve User</a>.");
                    }

                    // Redirect to the RegistrationConfirmation page
                    return RedirectToPage("RegisterConfirmation", new { email = user.Email });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


	}
}
