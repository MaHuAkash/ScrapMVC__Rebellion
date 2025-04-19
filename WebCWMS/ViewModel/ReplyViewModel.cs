using System.ComponentModel.DataAnnotations.Schema;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class ReplyViewModel
	{


        public int? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CommentId { get; set; }
        public string? ImageName { get; set; }

		[NotMapped]
		public IFormFile ImageFile { get; set; }


	}
}
