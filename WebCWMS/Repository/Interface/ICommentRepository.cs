using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
	public interface ICommentRepository
	{
		Task<IEnumerable<Comment>> GetAll();
		Task<Comment> GetCommentById(int id);
		Task<IEnumerable<CommentViewModel>> GetAllCommentViewModels();
		Task<IEnumerable<Comment>> GetCommentsByBlogId(int blogId);
		Task Add(Comment model);
		Task Delete(int commentId);
	}
}
