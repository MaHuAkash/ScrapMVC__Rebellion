using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Controllers
{
	public class CommentController : Controller
	{
		private readonly ICommentRepository _commentRepository;
		private readonly IWebHostEnvironment _webHostEnviroment;
		private readonly IBlogRepository _blogRepository;
        private readonly IReplyRepository _replyRepository;
		private readonly AppDbContext _appDbContext;


		public CommentController(ICommentRepository commentRepository,IReplyRepository replyRepository, IWebHostEnvironment webHostEnviroment, IBlogRepository blogRepository, AppDbContext appDbContext)
		{
			_commentRepository = commentRepository;
			_webHostEnviroment = webHostEnviroment;
			_blogRepository = blogRepository;
            _replyRepository = replyRepository;
			_appDbContext= appDbContext;
		}

		// Other actions and methods...

        [HttpGet]
        public async Task<IActionResult> AddComment(int blogId)
        {
            var blog = await _blogRepository.GetBlogById(blogId);
            if (blog == null)
            {
                // Blog not found, handle the error (e.g., show an error message or redirect to a 404 page)
                return NotFound();
            }

            // Pass the blogId to the view to be used in the form
            ViewBag.BlogId = blogId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(Comment model)
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

            try
            {
                if (ModelState.IsValid)
                {

                    await _commentRepository.Add(model);
                    TempData["SuccessMessage"] = "Commented successfully!";
                    return RedirectToAction("Details", "Blog", new { id = model.BlogId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid comment data.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View(model);
        }



		[HttpGet]
		public async Task<IActionResult> AddReply(int commentId, int blogId)
		{
			var comment = await _commentRepository.GetCommentsByBlogId(blogId);
			if (comment == null)
			{
				// Blog not found, handle the error (e.g., show an error message or redirect to a 404 page)
				return NotFound();
			}

			// Pass the blogId to the view to be used in the form
			ViewBag.CommentId = commentId;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddReply(ReplyViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					//int commentId = model.CommentId;
					// Convert the ReplyViewModel to the Reply entity
					var reply = new Reply
					{
						Name = model.Name,
						Email = model.Email,
						Content = model.Content,
						CreatedAt = DateTime.Now,
						CommentId = model.CommentId 
					};

					// Handle image upload if an image is provided
					if (model.ImageFile != null)
					{
						string wwwRootPath = _webHostEnviroment.WebRootPath;
						string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
						string extension = Path.GetExtension(model.ImageFile.FileName);
						string uniqueFileName = Guid.NewGuid().ToString() + extension;
						reply.ImageName = uniqueFileName;
						string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);
						using (var fileStream = new FileStream(path, FileMode.Create))
						{
							await model.ImageFile.CopyToAsync(fileStream);
						}
					}

					await _replyRepository.Add(reply);
					TempData["SuccessMessage"] = "Replied successfully!";
					// Fetch the comment with its blog ID from the repository
					var comment = await _commentRepository.GetCommentById(model.CommentId);

					// Redirect back to the "Blog" view (Details action) with the existing blog ID and comment ID
					return RedirectToAction("Details", "Blog", new { id = comment.BlogId, commentId = model.CommentId });
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = ex.Message;
				}
			}
			else
			{
				TempData["ErrorMessage"] = "Invalid reply data.";
			}

			return View(model);
		}


		 //Json for retriving comments
		[Authorize]
		[HttpGet]
		[Route("api/comment/{id}")]
		public async Task<IActionResult> GetCommentJson(int id)
		{
			var comment = await _commentRepository.GetCommentById(id);

			if (comment == null)
			{
				return NotFound();
			}

			return Json(new { content = comment.Content, name = comment.Name, createdat = comment.CreatedAt, imageName= comment.ImageName, email = comment .Email, });
		}




		[HttpPost]
		[Authorize]
		public async Task<IActionResult> DeleteCommentConfirmed(int id)
		{
			try
			{
				await _commentRepository.Delete(id);
				TempData["SuccessMessage"] = "Comment deleted successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error deleting the blog: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}


		[HttpGet]
		[Route("Reply/GetRepliesByCommentId/{commentId}/{page}")]
		public async Task<IActionResult> GetRepliesByCommentId(int commentId, int page = 1)
		{
			int repliesPerPage = 3; // Number of replies to display per page

			var replies = await _replyRepository.GetRepliesByCommentId(commentId);
			var paginatedReplies = replies.Skip((page - 1) * repliesPerPage).Take(repliesPerPage).ToList();

			// Return the paginated replies as JSON
			return Json(paginatedReplies);
		}


		[HttpGet]
		[Route("Reply/GetCommentByBlogId/{blogId}/{page}")]
		public async Task<IActionResult> GetCommentByBlogId(int blogId, int page = 1)
		{
			int commentPerPage = 3; // Number of replies to display per page

			var comment = await _commentRepository.GetCommentsByBlogId(blogId);
			var paginatedComment = comment.Skip((page - 1) * commentPerPage).Take(commentPerPage).ToList();

			// Return the paginated replies as JSON
			return Json(paginatedComment);
		}





	}
}

