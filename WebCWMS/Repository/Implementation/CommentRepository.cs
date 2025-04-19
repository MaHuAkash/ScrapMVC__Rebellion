using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Implementation
{
	public class CommentRepository : ICommentRepository
	{
		private readonly AppDbContext _context;

		public CommentRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Comment>> GetAll()
		{
			var comments = await _context.Comments.ToListAsync();
			return comments;
		}


		public async Task Add(Comment model)
		{
			await _context.Comments.AddAsync(model);
			await _context.SaveChangesAsync();
		}


		public async Task Delete(int commentId)
		{
			var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
			if (comment != null)
			{
				_context.Comments.Remove(comment);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<IEnumerable<CommentViewModel>> GetAllCommentViewModels()
		{
			var comments = await _context.Comments.ToListAsync();

			var commentViewModels = new List<CommentViewModel>();

			foreach (var comment in comments)
			{
				var blog = await _context.Blogs.FindAsync(comment.BlogId);

				var commentViewModel = new CommentViewModel
				{
					Id = comment.Id,
					Name = comment.Name,
					Email = comment.Email,
					CreatedAt = comment.CreatedAt,
					ImageName = comment.ImageName,
				};

				commentViewModels.Add(commentViewModel);
			}

			return commentViewModels;
		}



		public async Task<IEnumerable<Comment>> GetCommentsByBlogId(int blogId)
		{
			return await _context.Comments.Where(c => c.BlogId == blogId).ToListAsync();
		}

		public async Task<Comment> GetCommentById(int id)
		{
			return await _context.Comments.FindAsync(id);
		}
	}
}
