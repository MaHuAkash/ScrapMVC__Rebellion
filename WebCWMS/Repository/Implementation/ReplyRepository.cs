namespace WebCWMS.Repository.Implementation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using global::WebCWMS.Models;
	using global::WebCWMS.Repository.Interface;
	using global::WebCWMS.ViewModel;
	using Microsoft.EntityFrameworkCore;
    using SendGrid.Helpers.Mail;

    namespace WebCWMS.Repository
	{
		public class ReplyRepository : IReplyRepository
        { 
            private readonly AppDbContext _context;

            public ReplyRepository(AppDbContext context)
            {
                _context = context;
            }
            public async Task Add(Reply model)
            {
                await _context.Replys.AddAsync(model);
                await _context.SaveChangesAsync();
            }

            public async Task Delete(int id)
            {
                var replies = await _context.Replys.FirstOrDefaultAsync(c => c.Id == id);

                if (replies != null)
                {
                    _context.Replys.Remove(replies);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task<IEnumerable<Reply>> GetAll()
            {
                var replies = await _context.Replys.ToListAsync();
                return replies;
            }

            public async Task<IEnumerable<ReplyViewModel>> GetAllReplyViewModels()
            {
                var replies = await _context.Replys.ToListAsync();

                var replyViewModels = new List<ReplyViewModel>();

                foreach (var reply in replies)
                {
                    var comment = await _context.Comments.FindAsync(reply.CommentId);

                    var replyViewModel = new ReplyViewModel
                    {
                        Id = comment.Id,
                        Name = comment.Name,
                        Email = comment.Email,
                        CreatedAt = comment.CreatedAt,
                        ImageName = comment.ImageName,


                    };

                    replyViewModels.Add(replyViewModel);
                }

                return replyViewModels;
            }

			public async Task<IEnumerable<Reply>> GetRepliesByCommentId(int commentId)
			{
				return await _context.Replys
					.Where(r => r.CommentId == commentId)
					.ToListAsync();
			}
		}
	}
}
