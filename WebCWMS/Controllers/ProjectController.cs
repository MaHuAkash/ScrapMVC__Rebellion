using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCWMS.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebCWMS.Helpers;
using SendGrid.Helpers.Mail;

namespace WebCWMS.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _appDbContext;

        public ProjectController(IProjectRepository projectRepository, IWebHostEnvironment webHostEnvironment, AppDbContext appDbContext)
        {
            _projectRepository = projectRepository;
            _webHostEnvironment = webHostEnvironment;
            _appDbContext = appDbContext;
        }
        [Authorize]
        // Action to display all projects
        public async Task<IActionResult> Index()
        {
            var projects = await _projectRepository.GetAllProjectViewModels();
            return View(projects);
        }


            public async Task<IActionResult> Project(int? page)
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;

                var projectsViewModels = await _projectRepository.GetAllProjectViewModels();

                // Get the paginated blog posts using the PaginationHelper.
                var pagedBlogViewModels = PaginationHelper.GetPagedList(projectsViewModels, pageNumber, pageSize);

                ViewBag.PagedBlogViewModel = pagedBlogViewModels;

                return View(pagedBlogViewModels);
            }
		[Authorize(Roles = "Superadmin")]
		[HttpGet]
        public async Task<IActionResult> Create()
        {
            var province = await _projectRepository.GetAllProvince();
			var category = await _projectRepository.GetAllCategory();
			var cities = Enumerable.Empty<City>();
            var avenues = Enumerable.Empty<Avenue>();

            ViewBag.Provinces = new SelectList(province, "Id", "ProvinceName");
			ViewBag.Categories = new SelectList(category, "Id", "CategoryName");
			ViewBag.Cities = new SelectList(cities, "Id", "CityName");
            ViewBag.Avenues = new SelectList(avenues, "Id", "AvenueName");
            return View(new Project());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Project model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;

                        // Initialize list to store image filenames
                        var imageFileNames = new List<string>();

                        foreach (var file in model.ImageFiles)
                        {
                            if (file.Length > 0)
                            {
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                string uniqueFileName = Guid.NewGuid().ToString() + extension;
                                imageFileNames.Add(uniqueFileName);
                                string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);
                                  // Resize the image before saving
                                ResizeImageAsync(file.OpenReadStream(), path, 650, 550); // Set the desired width and height

                                //using (var fileStream = new FileStream(path, FileMode.Create))
                                //{
                                //    await file.CopyToAsync(fileStream);
                                //}
                            }
                        }

                        // Convert the list of filenames to a comma-separated string
                        model.ImageFileNames = string.Join(",", imageFileNames);
                    }


                    await _projectRepository.Add(model);
                    TempData["SuccessMessage"] = "Project created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Model state is invalid";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var project = await _projectRepository.Details(id);

                if (project == null)
                {
                    return NotFound(); // If project with the specified id is not found, return a "Not Found" view.
                }

                return View(project); // Assuming you have a corresponding Detail.cshtml view in the "Views/Project" folder.
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


		// Helper method to resize the image
		private async Task ResizeImageAsync(Stream imageStream, string outputPath, int width, int height)
		{
			using (Image image = Image.FromStream(imageStream))
			{
				using (Bitmap resizedImage = new Bitmap(width, height))
				{
					using (Graphics graphics = Graphics.FromImage(resizedImage))
					{
						graphics.CompositingQuality = CompositingQuality.HighQuality;
						graphics.SmoothingMode = SmoothingMode.HighQuality;
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
						graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

						graphics.DrawImage(image, 0, 0, width, height);

						using (var memoryStream = new MemoryStream())
						{
							resizedImage.Save(memoryStream, ImageFormat.Jpeg);
							memoryStream.Position = 0;

							using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
							{
								await memoryStream.CopyToAsync(fileStream);
								await fileStream.FlushAsync(); // Ensure all bytes are written to the disk
							}
						}
					}
				}
			}
		}


		[HttpGet]
		public async Task<IActionResult> GetProjectsByCategory(int categoryId)
		{
			var projects = await _appDbContext.Projects
				.Where(p => categoryId == 0 || p.CategoryId == categoryId)
				.ToListAsync();

			return Json(projects);
		}


		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var project = await _projectRepository.GetProjectById(id);
				if (project == null)
				{
					// Project not found
					return Json(new { success = false, message = "Project not found." });
				}

				// Use repository to delete the project
				await _projectRepository.Delete(id);

				// Return a success response
				return Json(new { success = true, message = "Project deleted successfully." });
			}
			catch (Exception ex)
			{
				// Handle any exceptions and return an error response
				return Json(new { success = false, message = ex.Message });
			}
		}



	}
}
