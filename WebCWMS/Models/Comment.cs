using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCWMS.Models
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int BlogId { get; set; }
        public string? ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload Picture")]
        public IFormFile ImageFile { get; set; }
    }
}
