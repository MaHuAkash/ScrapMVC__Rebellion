using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IBlogRepository _blogRepository;
		private readonly IConfiguration _configuration;

		public HomeController(ILogger<HomeController> logger, IBlogRepository blogRepository, IConfiguration configuration)
		{
			_logger = logger;
			_blogRepository = blogRepository;
			_configuration = configuration;
		}

		public async Task<IActionResult> Index()
		{
			var lastThreeBlogs = await _blogRepository.GetAllBlogViewModels();
			var viewModelList = lastThreeBlogs.ToList();

			return View(viewModelList);
		}

		[HttpGet]
		public IActionResult Contact()
		{
			var model = new ContactViewModel();
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Contact(ContactViewModel model)
		{
			if (ModelState.IsValid)
			{
				var fromEmail = model.Email;
                var toEmail = "akashlamsal.com.np@gmail.com"; 

				var smtpHost = "smtp.gmail.com";
				var smtpPort = 587;
                var smtpUsername = _configuration["Gmail:Username"];
                var smtpPassword = _configuration["Gmail:Password"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
				{
					client.EnableSsl = true;

					client.Credentials = new NetworkCredential(smtpUsername, smtpPassword );

					var mailMessage = new MailMessage();
					mailMessage.From = new MailAddress(fromEmail);
					mailMessage.ReplyToList.Add(new MailAddress(fromEmail, model.Name));
					mailMessage.To.Add(toEmail);
					mailMessage.Subject = model.Subject;
					mailMessage.Body = model.Message; 
					mailMessage.IsBodyHtml = true;

					try
					{
						await client.SendMailAsync(mailMessage);

						TempData["SuccessMessage"] = "Email sent successfully";
						return RedirectToAction("Contact");
					}
					catch (Exception ex)
					{

						TempData["ErrorMessage"] = "Failed to send email.";
						return RedirectToAction("Contact");
					}
				}
			}
			return View(model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
