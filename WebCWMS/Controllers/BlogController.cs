using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.ComponentModel.Design;
using WebCWMS.Helpers;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;
using X.PagedList;

namespace WebCWMS.Controllers
{
	public class BlogController : Controller
	{
		private readonly IBlogRepository _blogRepository;
		private readonly ICommentRepository _commentRepository;
		private readonly IReplyRepository _replyRepository;
		private readonly IWebHostEnvironment _webHostEnviroment;



		public BlogController(IBlogRepository blogRepository, IWebHostEnvironment webHostEnviroment, ICommentRepository commentRepository, IReplyRepository replyRepository)
		{
			_blogRepository = blogRepository;
			this._webHostEnviroment = webHostEnviroment;
			_commentRepository = commentRepository;
			_replyRepository = replyRepository;
		}
		public async Task<IActionResult> Blog(int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 5;

			var blogViewModels = await _blogRepository.GetAllBlogViewModels();

			// Get the paginated blog posts using the PaginationHelper.
			var pagedBlogViewModels = PaginationHelper.GetPagedList(blogViewModels, pageNumber, pageSize);

			// Get the data for the "popularPosts" section (3 newest blog posts)
			var popularBlogViewModels = blogViewModels.OrderByDescending(blog => blog.Date).Take(5).ToList();

			// Get the data for the "recentPosts" section (3 oldest blog posts)
			var recentBlogViewModels = blogViewModels.OrderBy(blog => blog.Date).Take(5).ToList();

			// Pass both models to the view
			ViewBag.PagedBlogViewModel = pagedBlogViewModels;
			ViewBag.PopularBlogViewModel = popularBlogViewModels;
			ViewBag.RecentBlogViewModel = recentBlogViewModels;

			return View(pagedBlogViewModels);
		}


        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> Index(int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 5;

			var blogViewModels = await _blogRepository.GetAllBlogViewModels();

			// Get the paginated blog posts using the PaginationHelper.
			var pagedBlogViewModels = PaginationHelper.GetPagedList(blogViewModels, pageNumber, pageSize);

			// Pass the paged model to the view using ViewBag.
			ViewBag.BlogViewModel = pagedBlogViewModels;

			return View(pagedBlogViewModels);
		}

        [Authorize(Roles = "Superadmin")]
        [HttpGet]
		public async Task<IActionResult> CreateOrEdit(int id = 0)
		{

			if (id == 0)
			{
				return View(new Blog());
			}
			else
			{
				var blog = await _blogRepository.GetBlogById(id);
				if (blog != null)
				{

					return View(blog);
				}

				TempData["errorMessage"] = $"Blog details not found with Id! : {id}";
				return RedirectToAction(nameof(Index));
			}
		}




        [Authorize(Roles = "Superadmin")]
        [HttpPost]
		public async Task<IActionResult> CreateOrEdit(Blog model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (model.ImageFile != null)
					{
						string wwwRootPath = _webHostEnviroment.WebRootPath;
						string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
						string extension = Path.GetExtension(model.ImageFile.FileName);
						string uniqueFileName = Guid.NewGuid().ToString() + extension;
						model.ImageName = uniqueFileName;
						string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);
						using (var fileStream = new FileStream(path, FileMode.Create))
						{
							await model.ImageFile.CopyToAsync(fileStream);
						}
					}

					if (model.Id == 0)
					{

						await _blogRepository.Add(model);
						TempData["SuccessMessage"] = "Blog created successfully!";
					}
					else
					{
						var existingBlog = await _blogRepository.GetBlogById(model.Id);
						existingBlog.Date = model.Date;
						existingBlog.Title = model.Title;
						existingBlog.Description = model.Description;
						existingBlog.ImageName = model.ImageName;
						await _blogRepository.Update(existingBlog);
						TempData["SuccessMessage"] = "Blog updated successfully!";
					}

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
        [Authorize(Roles = "Superadmin")]
        [HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var blog = await _blogRepository.GetBlogById(id);
			if (blog == null)
			{
				TempData["ErrorMessage"] = "Blog not found.";
				return RedirectToAction(nameof(Index));
			}

			return View(blog);
		}

        [Authorize(Roles = "Superadmin")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			try
			{
				await _blogRepository.Delete(id);
				TempData["SuccessMessage"] = "Blog deleted successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error deleting the blog: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}

        public async Task<IActionResult> Popup(int id)
        {
            var blog = await _blogRepository.GetBlogById(id);

            if (blog == null)
            {
                // Handle the case when the blog is not found, you can return a not found view or redirect, etc.
                return NotFound();
            }

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // If it's an AJAX request, return the blog details in JSON format
                return Json(new { title = blog.Title, description = blog.Description, imageName = blog.ImageName });
            }
            else
            {
                // For regular HTTP request, return the full view with blog details
                return View(blog);
            }
        }



		public async Task<IActionResult> Details(int id, BlogViewModel blogViewModel)
		{
			var blog = await _blogRepository.GetBlogById(id);

			if (blog == null)
			{
				return NotFound();
			}

			var comments = await _commentRepository.GetCommentsByBlogId(blog.Id);

			var commentViewModels = comments.Select(c => new CommentViewModel
			{
				Id = c.Id,
				Name = c.Name,
				Email = c.Email,
				CreatedAt = c.CreatedAt,
				ImageName = c.ImageName,
				Content = c.Content,
				Replies = null
			}).ToList();

			foreach (var comment in comments)
			{
				var replies = await _replyRepository.GetRepliesByCommentId(comment.Id);

				var replyViewModels = replies.Select(r => new ReplyViewModel
				{
					Id = r.Id,
					Name = r.Name,
					Email = r.Email,
					CreatedAt = r.CreatedAt,
					ImageName = r.ImageName,
					Content = r.Content,
				}).ToList();

				var commentViewModel = commentViewModels.FirstOrDefault(c => c.Id == comment.Id);
				if (commentViewModel != null)
				{
					commentViewModel.Replies = replyViewModels;
				}
			}

			blogViewModel.Comments = commentViewModels;

			return View(blogViewModel);
		}

	}
}






