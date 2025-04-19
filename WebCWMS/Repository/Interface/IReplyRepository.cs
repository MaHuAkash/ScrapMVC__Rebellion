using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
    public interface IReplyRepository
    {
        Task<IEnumerable<Reply>> GetAll();
        Task<IEnumerable<ReplyViewModel>> GetAllReplyViewModels();
		Task<IEnumerable<Reply>> GetRepliesByCommentId(int commentId);
		Task Add(Reply model);
        Task Delete(int id);
	}
}
