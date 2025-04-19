using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class CommentViewModel
	{

		public CommentViewModel()
		{
			blogs = new List<Blog>();
			Replies = new List<ReplyViewModel>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int BlogId { get; set; }
		public string? ImageName { get; set; }

		[NotMapped]
		public IFormFile ImageFile { get; set; }
		public List<Blog> blogs { get; set; }
		public List<ReplyViewModel> Replies { get; set; }



	}
}
